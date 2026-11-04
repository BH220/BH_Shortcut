using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH_Shortcut.Api.Model.Response
{
    public class ResBase
    {
        /// <summary>
        /// 서버 자체가 죽어버렸는지 여부를 반환
        /// 1: 서버가 살아 있음
        /// 0: 서버가 죽어 있거나 찾지 못함
        /// </summary>
        public int is_alive { get; set; } = 0;
        /// <summary>
        /// API 요청이 성공적으로 처리되었는지 반환
        /// 1: API 요청이 성공적으로 처리됨
        /// 0: API 요청이 실패했거나 처리 중 오류 발생
        /// </summary>
        public int result { get; set; } = 0;
        public string msg { get; set; } = "";
        /// <summary>
        /// 인증 실패(401) 여부. 로그아웃/세션 만료로 서버가 요청을 거부한 경우 true.
        /// true 인 경우 사용자에게 오류 팝업을 띄우지 않는다.
        /// </summary>
        public bool unauthorized { get; set; } = false;
    }
}
