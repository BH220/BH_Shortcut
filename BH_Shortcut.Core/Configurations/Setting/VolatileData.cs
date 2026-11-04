using Newtonsoft.Json;

namespace BH_Shortcut.Core.Configurations.Setting
{
    /// <summary>
    /// 휘발성 데이터. 로그인 화면의 "아이디 저장" 처럼 잃어도 되는 값을 담는다.
    /// 애플리케이션 시작 경로의 VolatileData.json 에 저장된다 (설정 파일과 달리 ProgramData 가 아니다).
    /// </summary>
    public class VolatileData : IConfig
    {
        /// <summary>파일 경로: (exe 폴더)\VolatileData.json</summary>
        public static string FilePath => Path.Combine(AppContext.BaseDirectory, nameof(VolatileData) + ".json");

        /// <summary>아이디 저장 여부</summary>
        public bool IsSaveUserId { get; set; }

        /// <summary>저장된 로그인 아이디. IsSaveUserId 가 false 면 빈 값.</summary>
        public string SavedUserId { get; set; } = "";

        public void Load()
        {
            try
            {
                if (File.Exists(FilePath) == false)
                    return;

                var loaded = JsonConvert.DeserializeObject<VolatileData>(File.ReadAllText(FilePath));
                if (loaded == null)
                    return;

                IsSaveUserId = loaded.IsSaveUserId;
                SavedUserId = loaded.IsSaveUserId ? (loaded.SavedUserId ?? "") : "";
            }
            catch (Exception ex)
            {
                //손상된 파일은 무시하고 빈 값으로 시작한다
                Console.WriteLine($"손상된 {nameof(VolatileData)} 파일 :: {ex.Message}");
                IsSaveUserId = false;
                SavedUserId = "";
            }
        }

        public void Save()
        {
            try
            {
                if (IsSaveUserId == false)
                    SavedUserId = "";

                File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(VolatileData)} 저장 실패 :: {ex.Message}");
            }
        }
    }
}
