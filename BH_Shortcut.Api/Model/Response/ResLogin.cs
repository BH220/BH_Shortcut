using BH_Shortcut.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH_Shortcut.Api.Model.Response
{
    public class ResLogin : ResBase
    {
        public UserSessionData? user_session_data { get; set; }

        /// <summary>중복 로그인(409) 시 서버가 돌려주는 기존 세션 정보</summary>
        public ExistingSessionData? existingSession { get; set; }

        /// <summary>서버가 msg = "DUPLICATE_SESSION" 으로 중복 로그인을 알린 경우</summary>
        public bool IsDuplicateSession => result == 0 && string.Equals(msg, "DUPLICATE_SESSION", StringComparison.OrdinalIgnoreCase);
    }
}
