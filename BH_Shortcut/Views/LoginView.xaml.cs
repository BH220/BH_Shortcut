using BH_Shortcut.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace BH_Shortcut.Views
{
    public partial class LoginView : FluentWindow
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            SystemThemeWatcher.Watch(this);

            //뷰모델이 닫아 달라고 하면 DialogResult 를 정해 창을 닫는다 (ShowDialog 로 열린 창은 DialogResult 를 넣으면 닫힌다)
            viewModel.CloseRequested += ok => DialogResult = ok;
        }
    }
}
