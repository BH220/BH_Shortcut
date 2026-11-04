using BH_Shortcut.Api.Interface;
using BH_Shortcut.Api.Model.Response;
using BH_Shortcut.Core.Helper;
using BH_Shortcut.Core.Session;

namespace BH_Shortcut.Api.Manager
{
    public class AuthManager : IAuthManager
    {
        public async Task<LoginResult> LoginAsync(string userId, string password, bool force = false)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
                return new LoginResult { Message = "로그인 정보를 확인 할 수 없습니다.\r\n아이디와 패스워드를 정확하게 입력하세요" };

            ResLogin res = force
                ? await ScApi.Instance.GetTokenForceAsync(userId, password)
                : await ScApi.Instance.GetTokenAsync(userId, password);

            if (res.result == 1 && res.user_session_data != null)
            {
                // 세션은 CmApi 가 서버 응답(user_session_data)으로 이미 생성했다. (uuid 가 이후 API 의 Bearer 토큰)
                return new LoginResult { Success = true };
            }

            if (res.is_alive == 0)
                return new LoginResult { ServerUnavailable = true, Message = "서버에 연결할 수 없습니다.\r\n우하단 설정(⚙)에서 API 서버 주소를 확인하세요." };

            if (res.IsDuplicateSession)
                return new LoginResult { IsDuplicateSession = true, ExistingSession = res.existingSession, Message = "이미 다른 곳에서 로그인되어 있습니다." };

            return new LoginResult { Message = string.IsNullOrEmpty(res.msg) ? "로그인에 실패했습니다." : res.msg };
        }

        public async Task AbortLoginAsync(string userId)
        {
            try
            {
                await ScApi.Instance.SetLoginAbortAsync(userId);
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "AbortLogin failed");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                if (SessionManager.Instance.IsLive)
                    await ScApi.Instance.PostResponseAsync<ResBase>("/api/auth/logout");
            }
            catch (Exception ex)
            {
                Log.Exception(ex, "Logout API failed");
            }
            finally
            {
                SessionManager.Clear();
            }
        }
    }
}
