using System.IO;
using System.Collections.ObjectModel;
using BH_Shortcut.Models;
using BH_Shortcut.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BH_Shortcut.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private const string JsonFilter = "JSON 파일 (*.json)|*.json|모든 파일 (*.*)|*.*";
        private const string TodoMessage = "이런저런 로직 구현 필요!";

        private readonly IFileDialogService _fileDialog;
        private readonly IDialogService _dialog;

        //왼쪽 그리드 목록
        public ObservableCollection<ShortcutItem> Items { get; } = new();

        //그리드에서 선택한 항목. null 이면 새 명령 작성 중.
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
        private ShortcutItem? selectedItem;

        //오른쪽 상세 패널이 편집하는 사본. 저장 버튼을 눌러야 목록에 반영된다.
        [ObservableProperty]
        private ShortcutItem editing = new();

        //로그인한 사용자. null 이면 로그아웃 상태.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLoggedIn))]
        [NotifyPropertyChangedFor(nameof(LoginText))]
        private LoginInfo? login;

        public bool IsLoggedIn => Login is not null;

        //다운로드 버튼 옆에 보여줄 [아이디(이름)]
        public string LoginText => Login is null ? string.Empty : $"[{Login.UserId}({Login.UserName})]";

        public MainViewModel(IFileDialogService fileDialog, IDialogService dialog)
        {
            _fileDialog = fileDialog;
            _dialog = dialog;

            //화면 확인용 예시 데이터
            Items.Add(new ShortcutItem
            {
                Kind = ShortcutKind.Remote,
                ShortName = "dev",
                ProgramName = "개발 서버",
                Address = "192.168.0.10",
                Port = 3389,
                Width = 1920,
                Height = 1080,
            });
            Items.Add(new ShortcutItem
            {
                Kind = ShortcutKind.Program,
                ShortName = "np",
                ProgramName = "메모장",
                FilePath = @"C:\Windows\notepad.exe",
            });
        }

        partial void OnSelectedItemChanged(ShortcutItem? value)
        {
            Editing = value?.Clone() ?? new ShortcutItem();
        }

        //이미 로그인되어 있으면 그대로, 아니면 로그인 창을 띄운다. 로그인 상태면 true
        private bool EnsureLoggedIn()
        {
            if (Login is not null)
                return true;

            Login = _dialog.ShowLogin();
            return Login is not null;
        }

        //로그아웃 (하이퍼링크)
        [RelayCommand]
        private void Logout()
        {
            //TODO: IAuthManager.LogoutAsync 연결
            Login = null;
        }

        //새 명령
        [RelayCommand]
        private void NewItem()
        {
            SelectedItem = null;
            Editing = new ShortcutItem();
        }

        //업로드 - 온라인: 로그인한 뒤 서버로 올린다
        [RelayCommand]
        private async Task UploadOnline()
        {
            if (!EnsureLoggedIn())
                return;

            await _dialog.ShowMessageAsync("온라인 업로드", "로그인: " + LoginText + "\n\n" + TodoMessage);
        }

        //업로드 - 오프라인: JSON 파일을 골라 들여온다
        [RelayCommand]
        private async Task UploadOffline()
        {
            string? path = _fileDialog.OpenFile(JsonFilter, "업로드할 JSON 파일 선택");
            if (path is null)
                return;

            await _dialog.ShowMessageAsync("오프라인 업로드", "선택한 파일:\n" + path + "\n\n" + TodoMessage);
        }

        //다운로드 - 온라인: 로그인한 뒤 서버에서 가져온다
        [RelayCommand]
        private async Task DownloadOnline()
        {
            if (!EnsureLoggedIn())
                return;

            await _dialog.ShowMessageAsync("온라인 다운로드", "로그인: " + LoginText + "\n\n" + TodoMessage);
        }

        //다운로드 - 오프라인: JSON 파일로 내보낸다
        [RelayCommand]
        private async Task DownloadOffline()
        {
            string? path = _fileDialog.SaveFile(JsonFilter, "JSON 파일로 내보내기", "shortcuts.json");
            if (path is null)
                return;

            await _dialog.ShowMessageAsync("오프라인 다운로드", "저장할 파일:\n" + path + "\n\n" + TodoMessage);
        }

        //상세: 파일 선택 (구분 = 프로그램)
        [RelayCommand]
        private void SelectFile()
        {
            string? path = _fileDialog.OpenFile("실행 파일 (*.exe)|*.exe|모든 파일 (*.*)|*.*", "프로그램 선택");
            if (path is null)
                return;

            Editing.FilePath = path;
            if (string.IsNullOrWhiteSpace(Editing.ProgramName))
                Editing.ProgramName = Path.GetFileNameWithoutExtension(path);
        }

        //상세: 저장. 새 명령이면 목록에 추가, 기존 항목이면 덮어쓴다.
        [RelayCommand]
        private void Save()
        {
            if (SelectedItem is null)
            {
                var item = Editing.Clone();
                Items.Add(item);
                SelectedItem = item;
            }
            else
            {
                SelectedItem.CopyFrom(Editing);
            }
            Console.WriteLine("[Save] " + Editing.KindText + " / " + Editing.ShortName + " / " + Editing.ProgramName);
        }

        //상세: 삭제. 그리드에서 고른 항목을 목록에서 뺀다. 새 명령 작성 중에는 비활성.
        [RelayCommand(CanExecute = nameof(CanDelete))]
        private void Delete()
        {
            var item = SelectedItem;
            if (item is null)
                return;

            SelectedItem = null;
            Items.Remove(item);
            Console.WriteLine("[Delete] " + item.KindText + " / " + item.ShortName + " / " + item.ProgramName);
        }

        private bool CanDelete() => SelectedItem is not null;
    }
}
