using BH_Shortcut.Api.Model;
using BH_Shortcut.Api.Model.Response;
using BH_Shortcut.Core.Common;
using BH_Shortcut.Core.Configurations;
using BH_Shortcut.Core.Configurations.Setting;
using BH_Shortcut.Core.Helper;
using BH_Shortcut.Core.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BH_Shortcut.Api
{ 
    public class ScApi
    {
        private static ScApi _instance = null;

        public static ScApi Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScApi();
                    _instance.SetUrl(Config.ServerSetting.Address);
                }
                return _instance;
            }
        }

        private string _baseUrl;

        /// <summary>
        /// 서버 호출용 HttpClient. 호출마다 새로 만들면 연결·TLS 핸드셰이크를 반복하므로 하나를 공유한다.
        /// 주소가 바뀌면 (<see cref="SetUrl"/>) 다음 호출 때 다시 만든다. 인증 헤더는 요청 단위로 붙인다.
        /// </summary>
        private HttpClient? _client;
        private string? _clientBaseUrl;

        /// <summary>/health 확인용 (3초 제한). 인증서는 정상 검증하고, localhost 개발 서버의 자체 서명 인증서만 허용한다.</summary>
        private static readonly HttpClient HealthHttp = new(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                errors == System.Net.Security.SslPolicyErrors.None || (request.RequestUri?.IsLoopback ?? false)
        })
        { Timeout = TimeSpan.FromSeconds(3) };

        public bool IsAlive() => IsAlive(Config.ServerSetting.Address);

        /// <summary>지정한 주소(스킴 포함, 예: https://api.example.com)의 서버가 응답하는지 확인한다. (설정 저장 전 연결 테스트용, 동기)</summary>
        public bool IsAlive(string address) => IsAliveAsync(address).GetAwaiter().GetResult();

        public Task<bool> IsAliveAsync() => IsAliveAsync(Config.ServerSetting.Address);

        /// <summary>지정한 주소의 서버가 응답하는지 확인한다. UI 스레드를 막지 않는다.</summary>
        public async Task<bool> IsAliveAsync(string address)
        {
            address = ServerSetting.Normalize(address);
            if (string.IsNullOrEmpty(address))
                return false;
            try
            {
                Uri url = new Uri($"{address}/health");
                using var response = await HealthHttp.GetAsync(url).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "IsAlive check failed");
            }
            return false;
        }

        public ScApi()
        {
            _baseUrl = "https://localhost:5001";
        }

        /// <summary>공유 HttpClient. 호출하는 쪽에서 Dispose 하지 않는다.</summary>
        private HttpClient HttpClientEx()
        {
            if (_client is null || _clientBaseUrl != _baseUrl)
            {
                HttpClientHandler handler = new HttpClientHandler();
                if (_baseUrl.Contains("://localhost"))
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                _client = new HttpClient(handler);
                _clientBaseUrl = _baseUrl;
            }
            return _client;
        }

        /// <summary>로그인 세션이 있으면 Bearer 헤더를 붙인 요청을 만든다.</summary>
        private static HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, url) { Content = content };
            if (string.IsNullOrEmpty(SessionManager.Instance.UUID) == false)
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SessionManager.Instance.UUID);
            return request;
        }

        /// <summary>API 기본 주소를 설정한다. 스킴 포함 (예: https://api.example.com). 스킴이 없으면 https:// 로 간주한다.</summary>
        public void SetUrl(string url)
        {
            string address = ServerSetting.Normalize(url);
            _baseUrl = address;
            Config.ServerSetting.Address = address;
        }

        public async Task<T> PostResAsync<T>(string endpoint, Dictionary<string, string> dictionary) where T : ResBase, new()
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(dictionary);
            return await PostResponseAsync<T>(endpoint, content);
        }

        public async Task<T> PostResponseAsync<T>(string endpoint, HttpContent content = null) where T : ResBase, new()
        {
            // 인증 필요 여부는 클라이언트가 판단하지 않는다. (서버 컨트롤러의 [Authorize]가 판단)
            // 익명 API(/api/threshold, /api/update 등)는 로그인 여부와 무관하게 정상 호출되어야 한다.
            bool wasLive = SessionManager.Instance.IsLive;
            try
            {
                {
                    var client = HttpClientEx();
                    string apiUrl = $"{_baseUrl}{endpoint}";

                    using var request = CreateRequest(HttpMethod.Post, apiUrl, content);
                    using HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        SessionManager.Clear();
                        if (wasLive)
                        {
                            // 사용 중 서버에서 세션이 만료/종료됨 → 세션 정리 후, 사용자에게 명확한 메시지를 전달한다.
                            Log.ErrorLog($"인증 실패(Unauthorized) - EndPoint: {endpoint}");
                            return new T { result = 0, unauthorized = true, msg = "Session expired. Please login again." };
                        }
                        // 로그아웃/미로그인 상태에서 보낸 요청의 401 은 조용히 실패 처리한다.
                        // (백그라운드 폴링 등 여러 요청이 한꺼번에 401 을 받아 오류 팝업이 겹쳐 뜨는 것을 방지)
                        return new T { result = 0, unauthorized = true, msg = "" };
                    }
                    // 서버는 입력값 문제를 400 + {result:0, msg:"..."} 로 알려준다.
                    // 그 msg 가 사용자가 고칠 수 있는 유일한 단서이므로 예외로 버리지 않고 그대로 돌려준다.
                    string body = await response.Content.ReadAsStringAsync();
                    string errMsg = $"Post 요청 실패: {response.StatusCode} {Environment.NewLine} EndPoint: {endpoint} {Environment.NewLine} Body: {body}";
                    Log.ErrorLog(errMsg);

                    if (string.IsNullOrWhiteSpace(body) == false)
                    {
                        try
                        {
                            T? failed = JsonConvert.DeserializeObject<T>(body);
                            if (failed != null && string.IsNullOrWhiteSpace(failed.msg) == false)
                            {
                                failed.result = 0;
                                return failed;
                            }
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            // JSON 이 아니면 아래 예외로 넘어간다.
                        }
                    }

                    throw new Exception($"Post 요청 실패: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return new T { msg = ex.Message };
            }
        }


        /// <summary>
        /// 인증이 필요한 바이너리(첨부 이미지 원본 등)를 GET 으로 받는다.
        /// endpoint 는 "/api/security/image/1" 같은 상대 경로 또는 절대 URL. (서버 라우트: GET /api/security/image/{image_num})
        /// 성공이면 Content-Type 이 image/* 인 바이너리, 실패면 JSON({result:0, msg}) 이 오므로 msg 를 그대로 전달한다.
        /// </summary>
        public async Task<(bool Success, string Message, byte[]? Data)> GetBytesAsync(string endpoint)
        {
            bool wasLive = SessionManager.Instance.IsLive;
            try
            {
                var client = HttpClientEx();
                bool isAbsolute = endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                               || endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
                string apiUrl = isAbsolute ? endpoint : $"{_baseUrl}{(endpoint.StartsWith("/") ? "" : "/")}{endpoint}";

                using var request = CreateRequest(HttpMethod.Get, apiUrl);
                using HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    SessionManager.Clear();
                    if (wasLive)
                        Log.ErrorLog($"인증 실패(Unauthorized) - EndPoint: {endpoint}");
                    return (false, wasLive ? "Session expired. Please login again." : "", null);
                }

                // 오류는 JSON 으로 온다.
                string mediaType = response.Content.Headers.ContentType?.MediaType ?? "";
                if (mediaType.Contains("json", StringComparison.OrdinalIgnoreCase))
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResBase>(json);
                    string msg = string.IsNullOrEmpty(res?.msg) ? $"요청 실패: {response.StatusCode}" : res!.msg;
                    Log.ErrorLog($"GET 요청 실패: {response.StatusCode} {msg} EndPoint: {endpoint}");
                    return (false, msg, null);
                }

                if (response.IsSuccessStatusCode == false)
                {
                    Log.ErrorLog($"GET 요청 실패: {response.StatusCode} EndPoint: {endpoint}");
                    return (false, $"요청 실패: {response.StatusCode}", null);
                }

                byte[] data = await response.Content.ReadAsByteArrayAsync();
                return (true, "", data);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, $"GetBytesAsync failed: {endpoint}");
                return (false, ex.Message, null);
            }
        }

        /// <summary>
        /// 파일 업로드 (multipart/form-data, Bearer 인증). fields 는 일반 폼 값, 파일은 fileField 이름(이미지 등록 API 는 "image_data")으로 붙인다.
        /// 서버(PHP)는 $_FILES[fileField] 로 받으므로 파트에 filename 이 반드시 있어야 한다.
        /// </summary>
        public async Task<T> PostMultipartAsync<T>(string endpoint, IDictionary<string, string> fields, string fileField, string fileName, byte[] fileData, string? contentType = null) where T : ResBase, new()
        {
            using var content = new MultipartFormDataContent();
            foreach (var field in fields)
                content.Add(new StringContent(field.Value ?? ""), field.Key);

            var file = new ByteArrayContent(fileData);
            file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType ?? GuessImageContentType(fileName));
            content.Add(file, fileField, fileName);

            return await PostResponseAsync<T>(endpoint, content);
        }

        /// <summary>파일명 확장자로 이미지 Content-Type 을 정한다. 모르면 application/octet-stream.</summary>
        private static string GuessImageContentType(string fileName)
        {
            string ext = Path.GetExtension(fileName ?? "").TrimStart('.').ToLowerInvariant();
            return ext switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                "webp" => "image/webp",
                _ => "application/octet-stream",
            };
        }

        /// <summary>로그인 요청 시 서버에 전달하는 프로그램 번호</summary>
        public const string ProgramNum = "7";

        /// <summary>로그인. 다른 곳에 세션이 있으면 409 / msg = DUPLICATE_SESSION 으로 응답한다. (<see cref="ResLogin.IsDuplicateSession"/>)</summary>
        public Task<ResLogin> GetTokenAsync(string userId, string password, bool requestClient = true)
            => LoginInternalAsync("/api/auth/login", userId, password);

        /// <summary>기존 세션을 끊고 로그인한다.</summary>
        public Task<ResLogin> GetTokenForceAsync(string userId, string password, bool requestClient = true)
            => LoginInternalAsync("/api/auth/login/force", userId, password);

        private async Task<ResLogin> LoginInternalAsync(string endpoint, string userId, string password)
        {
            ResLogin result = new ResLogin();
            if (await IsAliveAsync() == false)
            {
                result.is_alive = 0;
                result.msg = "서버에 연결할 수 없습니다.";
                return result;
            }
            try
            {
                var client = HttpClientEx();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("program_num", ProgramNum),
                    new KeyValuePair<string, string>("id", userId),
                    new KeyValuePair<string, string>("pw", password),
                });

                var response = await client.PostAsync($"{_baseUrl}{endpoint}", content);
                result.is_alive = 1;

                string body = await response.Content.ReadAsStringAsync();
                JObject? json = null;
                try { json = string.IsNullOrWhiteSpace(body) ? null : JObject.Parse(body); } catch { /* JSON 이 아닌 응답 */ }

                if (response.IsSuccessStatusCode)
                {
                    var session = json?["user_session_data"]?.ToObject<UserSessionData>();
                    if (session == null)
                    {
                        result.msg = "서버 응답에 세션 정보가 없습니다.";
                        Log.ErrorLog($"로그인 응답 형식 오류 - EndPoint: {endpoint}, Body: {body}");
                        return result;
                    }
                    result.result = 1;
                    result.user_session_data = session;
                    SessionManager.MakeSession(session);
                }
                else
                {
                    result.msg = json?["msg"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(result.msg))
                        result.msg = $"로그인 실패 ({(int)response.StatusCode} {response.StatusCode})";
                    result.unauthorized = response.StatusCode == HttpStatusCode.Unauthorized;

                    var existing = json?["existing_session"] ?? json?["existingSession"];
                    if (existing != null)
                    {
                        try { result.existingSession = existing.ToObject<ExistingSessionData>(); } catch { }
                    }
                    Log.Warn($"로그인 실패 - EndPoint: {endpoint}, Status: {(int)response.StatusCode}, Msg: {result.msg}");
                }
            }
            catch (HttpRequestException ex)
            {
                result.is_alive = 0;
                result.msg = ex.Message;
                Log.Exception(ex, $"로그인 요청 실패 - EndPoint: {endpoint}");
            }
            catch (Exception ex)
            {
                result.msg = ex.Message;
                Log.Exception(ex, $"로그인 처리 오류 - EndPoint: {endpoint}");
            }
            return result;
        }

        public async Task<ResBase> SetLoginAbortAsync(string userId)
        {
            ResBase result = new ResBase();
            if (await IsAliveAsync() == false)
            {
                //result.is_alive = 0;
                return result;
            }
            try
            {
                var requestUrl = $"{_baseUrl}/api/auth/login/abort";

                var client = HttpClientEx();
                {
                    var keyValues = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("program_num", ProgramNum),
                            new KeyValuePair<string, string>("id", userId)
                        };

                    var content = new FormUrlEncodedContent(keyValues);

                    var response = await client.PostAsync(requestUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        result.result = 1;
                    }
                    else
                    {
                        result.msg = response.StatusCode.ToString();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                //result.is_alive = 0;
                result.msg = ex.Message;
            }
            catch (Exception ex)
            {
                result.msg = ex.Message;
            }
            return result;
        }
         

    }
}
