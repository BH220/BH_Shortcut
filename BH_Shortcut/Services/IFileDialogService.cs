namespace BH_Shortcut.Services
{
    //뷰모델에서 파일 열기/저장 대화상자를 띄울 때 쓴다.
    public interface IFileDialogService
    {
        //파일 열기. 취소하면 null
        string? OpenFile(string filter, string? title = null);

        //파일 저장. 취소하면 null
        string? SaveFile(string filter, string? title = null, string? defaultFileName = null);
    }
}
