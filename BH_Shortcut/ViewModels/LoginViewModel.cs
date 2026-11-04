using BH_Shortcut.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BH_Shortcut.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userId = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage = string.Empty;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        //로그인에 성공한 사용자 정보. 성공 전·취소 시에는 null
        public LoginInfo? Result { get; private set; }

        //창을 닫아 달라는 요청. true = 로그인 성공, false = 취소
        public event Action<bool>? CloseRequested;

        [RelayCommand]
        private void Login()
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                ErrorMessage = "아이디를 입력하세요.";
                return;
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "비밀번호를 입력하세요.";
                return;
            }

            ErrorMessage = string.Empty;

            //TODO: BH_Shortcut.Api 의 IAuthManager.LoginAsync 연결. 지금은 입력만 확인하고 성공으로 본다.
            //      이름은 서버 응답(user_session_data.user_name)으로 바꿔야 하며, 지금은 아이디를 그대로 쓴다.
            string id = UserId.Trim();
            Result = new LoginInfo(id, id);
            CloseRequested?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel() => CloseRequested?.Invoke(false);
    }
}
