using BH_Shortcut.Services;
using BH_Shortcut.ViewModels;
using BH_Shortcut.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace BH_Shortcut
{
    public static class DiService
    {
        public static ServiceProvider ServicesRegister()
        {
            var services = new ServiceCollection();

            // 서비스
            services.AddSingleton<IMessenger>(new WeakReferenceMessenger());
            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<IDialogService, DialogService>();

            // ViewModel
            services.AddSingleton<MainViewModel>();
            services.AddTransient<LoginViewModel>();

            // View
            services.AddSingleton<MainView>();
            services.AddTransient<LoginView>();

            return services.BuildServiceProvider();
        }
    }
}
