using Microsoft.Win32;

namespace BH_Shortcut.Services
{
    public class FileDialogService : IFileDialogService
    {
        public string? OpenFile(string filter, string? title = null)
        {
            var dlg = new OpenFileDialog
            {
                Filter = filter,
                Title = title ?? "파일 선택",
                CheckFileExists = true,
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public string? SaveFile(string filter, string? title = null, string? defaultFileName = null)
        {
            var dlg = new SaveFileDialog
            {
                Filter = filter,
                Title = title ?? "파일 저장",
                FileName = defaultFileName ?? string.Empty,
                AddExtension = true,
                OverwritePrompt = true,
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }
}
