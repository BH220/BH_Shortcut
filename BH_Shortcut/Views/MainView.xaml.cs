using BH_Shortcut.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace BH_Shortcut.Views
{
    public partial class MainView : FluentWindow
    {
        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;

            //OS 라이트/다크 테마를 따라가고, 바뀌면 같이 바뀐다
            SystemThemeWatcher.Watch(this);
        }
    }
}
