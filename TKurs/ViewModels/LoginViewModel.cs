using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TKurs.Context;
using TKurs.Views;

namespace TKurs.ViewModels
{
    public partial class LoginViewModel : ObservableObject, ICloseWindows
    {

        private readonly OptimDbContext context;

        [NotifyCanExecuteChangedFor(nameof(TryLoginCommand))]
        [ObservableProperty]
        private string login = string.Empty;

        [NotifyCanExecuteChangedFor(nameof(TryLoginCommand))]
        [ObservableProperty]
        private string password = string.Empty;

        public LoginViewModel()
        {
            context = new OptimDbContext();
        }

       

        public Action Close { get; set; }

        public bool CanClose()
        {
            return true;
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
            ;
        }


        [RelayCommand(CanExecute = nameof(CanLogin))]
        public void TryLogin()
        {
            var users = context.Users.ToList();
            var user = context.Users.FirstOrDefault(x => x.Login == Login && x.Password == Password);
            if (user != null)
            {

                var window = new MainWindow();
                window.Show();
                Close.Invoke();
            }
            else
            {
                MessageBox.Show("Непрвильный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        public void Register()
        {
            var window = new RegisterWindow();
            window.ShowDialog();
        }
    }
}
