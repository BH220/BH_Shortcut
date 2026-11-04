using System.Windows;
using BH_Shortcut.Models;
using BH_Shortcut.ViewModels;
using BH_Shortcut.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BH_Shortcut.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _services;

        public DialogService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task ShowMessageAsync(string title, string message)
        {
            var box = new Wpf.Ui.Controls.MessageBox
            {
                Title = title,
                Content = message,
                CloseButtonText = "확인",
            };
            await box.ShowDialogAsync();
        }

        public LoginInfo? ShowLogin()
        {
            //매번 새 창·새 뷰모델 (Transient)
            var view = _services.GetRequiredService<LoginView>();
            view.Owner = Application.Current?.MainWindow;
            if (view.ShowDialog() != true)
                return null;

            return ((LoginViewModel)view.DataContext).Result;
        }
    }
}
