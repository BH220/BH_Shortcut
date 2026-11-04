using Newtonsoft.Json;

namespace BH_Shortcut.Core.Session
{
    /// <summary>
    /// 중복 로그인(409 DUPLICATE_SESSION) 응답의 existing_session 정보
    /// </summary>
    public class ExistingSessionData
    {
        public string ip { get; set; } = "";

        [JsonProperty("user_agent")]
        public string userAgent { get; set; } = "";

        [JsonProperty("login_at")]
        public DateTime? loginAt { get; set; }

        [JsonProperty("last_activity_at")]
        public DateTime? lastActivityAt { get; set; }
    }
}
