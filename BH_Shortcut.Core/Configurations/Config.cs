using BH_Shortcut.Core.Configurations.Setting;

namespace BH_Shortcut.Core.Configurations
{
    /// <summary>
    /// 프로그램 설정값을 관리하는 클래스
    /// </summary>
    public class Config
    {
        private static ServerSetting? _serverSetting;

        /// <summary>API 서버 설정. 최초 접근 시 JSON 파일에서 로드된다. (<see cref="ServerSetting.FilePath"/>)</summary>
        public static ServerSetting ServerSetting
        {
            get
            {
                if (_serverSetting == null)
                {
                    _serverSetting = new ServerSetting();
                    _serverSetting.Load();
                }
                return _serverSetting;
            }
        }

        private static VolatileData? _volatileData;

        /// <summary>휘발성 데이터(아이디 저장 등). 최초 접근 시 exe 폴더의 VolatileData.json 에서 로드된다.</summary>
        public static VolatileData VolatileData
        {
            get
            {
                if (_volatileData == null)
                {
                    _volatileData = new VolatileData();
                    _volatileData.Load();
                }
                return _volatileData;
            }
        }

        public static Mutex? RunMutex { get; set; }
        public static bool IsExitPossible { get; set; }
    }
}
