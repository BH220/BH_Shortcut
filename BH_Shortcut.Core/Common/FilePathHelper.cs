using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH_Shortcut.Core.Common
{
    public class FilePathHelper
    {
        private static string GetPath(params string[] paths)
        {
            string path = Path.Combine(paths);
            // 확장자 있으면 파일로 간주 → 부모 폴더만 생성
            string? dir = Path.HasExtension(path)
                ? Path.GetDirectoryName(path)
                : path;

            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return path;
        }

        /// <summary>
        /// 기준이 되는 데이터 폴더. 윈도우는 %ProgramData%.
        /// 안드로이드/리눅스에서는 CommonApplicationData 가 "/usr/share"(쓰기 불가)로 잡히고 c:\ 도 쓸 수 없어서
        /// 앱 전용 저장소로 대체한다. (윈도우 동작은 그대로다.)
        /// </summary>
        private static string PlatformDataRoot()
        {
            if (OperatingSystem.IsWindows())
            {
                string common = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if (string.IsNullOrEmpty(common) == false)
                    return common;
            }
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }


        #region 기준 경로
        public static string ProgramDataRoot { get { return GetPath(PlatformDataRoot(), "BH Soft", BhShortcutMakerDisplay); } }
        public static string Root { get { return OperatingSystem.IsWindows() ? GetPath(@"c:\Bh Soft") : GetPath(PlatformDataRoot(), "Bh Soft"); } }
        public static string AppRoot { get { return GetPath(Root, BhShortcutMakerDisplay); } }
        public static string SettingRoot { get { return GetPath(ProgramDataRoot, BhShortcutMakerDisplay, "settings"); } }
        public static string LogRoot { get { return GetPath(Root, "logs", BhShortcutMakerDisplay); } }

        #endregion

        #region 프로그램 표시명
        public static string BhShortcutMakerDisplay { get { return "BH Shortcut"; } }
        #endregion
    }
}
