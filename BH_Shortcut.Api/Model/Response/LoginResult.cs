using BH_Shortcut.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH_Shortcut.Api.Model.Response
{
    public class LoginResult
    {
        public bool Success { get; init; }
        public string Message { get; init; } = "";

        /// <summary>서버에 연결할 수 없음 (주소 오류, 서버 다운)</summary>
        public bool ServerUnavailable { get; init; }

        /// <summary>다른 곳에 로그인 세션이 있음. 사용자에게 강제 로그인 여부를 물어야 한다.</summary>
        public bool IsDuplicateSession { get; init; }
        public ExistingSessionData? ExistingSession { get; init; }
    }
}
