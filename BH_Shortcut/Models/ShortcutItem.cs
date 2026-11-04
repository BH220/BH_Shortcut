using CommunityToolkit.Mvvm.ComponentModel;

namespace BH_Shortcut.Models
{
    public enum ShortcutKind
    {
        Remote,     //원격연결
        Program,    //프로그램
    }

    //단축 명령 한 건. 왼쪽 목록(그리드)과 오른쪽 상세 편집에서 같이 쓴다.
    public partial class ShortcutItem : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(KindText))]
        [NotifyPropertyChangedFor(nameof(IsRemote))]
        [NotifyPropertyChangedFor(nameof(IsProgram))]
        private ShortcutKind kind = ShortcutKind.Remote;

        [ObservableProperty]
        private string shortName = string.Empty;

        [ObservableProperty]
        private string programName = string.Empty;

        //구분 = 원격연결
        [ObservableProperty]
        private string address = string.Empty;

        [ObservableProperty]
        private int port = 3389;

        [ObservableProperty]
        private int width = 1920;

        [ObservableProperty]
        private int height = 1080;

        //구분 = 프로그램
        [ObservableProperty]
        private string filePath = string.Empty;

        //그리드 "구분" 컬럼 표시용
        public string KindText => Kind switch
        {
            ShortcutKind.Remote => "원격연결",
            ShortcutKind.Program => "프로그램",
            _ => string.Empty,
        };

        //라디오 버튼 바인딩용. 체크 해제(false)는 무시하고 체크(true)만 Kind 에 반영한다.
        public bool IsRemote
        {
            get => Kind == ShortcutKind.Remote;
            set { if (value) Kind = ShortcutKind.Remote; }
        }

        public bool IsProgram
        {
            get => Kind == ShortcutKind.Program;
            set { if (value) Kind = ShortcutKind.Program; }
        }

        public ShortcutItem Clone() => new ShortcutItem().CopyFrom(this);

        public ShortcutItem CopyFrom(ShortcutItem src)
        {
            Kind = src.Kind;
            ShortName = src.ShortName;
            ProgramName = src.ProgramName;
            Address = src.Address;
            Port = src.Port;
            Width = src.Width;
            Height = src.Height;
            FilePath = src.FilePath;
            return this;
        }
    }
}
