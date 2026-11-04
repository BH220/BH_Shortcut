using Newtonsoft.Json;

namespace BH_Shortcut.Core.Session
{
    /// <summary>
    /// 로그인 응답의 user_session_data. (BHS_Api AuthService::createSession 과 필드명 일치)
    /// </summary>
    public class UserSessionData
    {
        public string uuid { get; set; } = "";

        /// <summary>서버 필드명은 user_num. 기존 코드 호환을 위해 user_no 로도 노출한다.</summary>
        [JsonProperty("user_num")]
        public ulong user_no { get; set; }

        public string user_id { get; set; } = "";
        public string user_name { get; set; } = "";
        public int program_num { get; set; }
        public StatusTypes status { get; set; }

        /// <summary>세션 유지 시간(분)</summary>
        public int timeout { get; set; }
        public UserType user_type { get; set; }
        public DateTime? expired_at { get; set; }
    }
}
