using BH_Shortcut.Models;

namespace BH_Shortcut.Services
{
    //뷰모델에서 메시지 상자·로그인 창 같은 모달 UI 를 띄울 때 쓴다.
    public interface IDialogService
    {
        //확인 버튼 하나짜리 메시지 상자
        Task ShowMessageAsync(string title, string message);

        //로그인 창을 모달로 띄운다. 로그인에 성공하면 사용자 정보, 취소하면 null
        LoginInfo? ShowLogin();
    }
}
