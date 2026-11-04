using System.ComponentModel;
using BH_Shortcut.Core.Common;

namespace BH_Shortcut.Core.Configurations.Setting
{
    /// <summary>
    /// API 서버 접속 설정. JSON 파일(<see cref="FilePath"/>)로 저장된다.
    /// </summary>
    public class ServerSetting : ConfigBase, IConfig
    {
        /// <summary>설정 파일 경로: %ProgramData%\BH Soft\settings\ServerSetting.json</summary>
        public static string FilePath => Path.Combine(FilePathHelper.SettingRoot, nameof(ServerSetting) + ".json");

        /// <summary>API 서버 주소 (스킴 포함). 예: https://api.example.com, http://192.168.1.100:5001</summary>
        [DefaultValue("")]
        public string Address { get; set; } = "";

        public void Load()
        {
            base.LoadConfig<ServerSetting>(this);
            Address = Normalize(Address);
        }

        public void Save()
        {
            Address = Normalize(Address);
            base.SaveConfig<ServerSetting>(this);
        }

        /// <summary>
        /// 입력값을 정리한다. 공백 제거, 끝 슬래시 제거, 스킴이 없으면 https:// 를 붙인다.
        /// </summary>
        public static string Normalize(string? address)
        {
            string value = (address ?? "").Trim().TrimEnd('/');
            if (value.Length == 0)
                return "";
            if (value.Contains("://") == false)
                value = "https://" + value;
            return value;
        }

        /// <summary>http/https 절대 주소인지 검사한다.</summary>
        public static bool IsValid(string? address, out string error)
        {
            error = "";
            string value = Normalize(address);
            if (value.Length == 0)
            {
                error = "API 서버 주소를 입력하세요.";
                return false;
            }
            if (Uri.TryCreate(value, UriKind.Absolute, out var uri) == false ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                error = "주소 형식이 올바르지 않습니다. 예) https://api.example.com, http://192.168.1.100:5001";
                return false;
            }
            if (uri.PathAndQuery != "/" || string.IsNullOrEmpty(uri.Fragment) == false)
            {
                error = "주소에는 경로를 포함하지 마세요. 예) https://api.example.com";
                return false;
            }
            return true;
        }
    }
}
