namespace BH_Shortcut.Models
{
    //로그인에 성공한 사용자 정보. 화면에는 [아이디(이름)] 꼴로 보여준다.
    public record LoginInfo(string UserId, string UserName);
}
