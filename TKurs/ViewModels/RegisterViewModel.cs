using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TKurs.Context;
using TKurs.Model;

namespace TKurs.ViewModels
{
    public partial class RegisterViewModel : ObservableObject, ICloseWindows
    {
        private readonly OptimDbContext context;

        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [ObservableProperty]
        private string comfirmPassword;

        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [ObservableProperty]
        private string email;

        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [ObservableProperty]
        private string firstName;

        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [ObservableProperty]
        private string lastName;

        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [ObservableProperty]
        private string login;

        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        [ObservableProperty]
        private string password;


        public RegisterViewModel()
        {
            context = new OptimDbContext();
        }


        public Action Close { get; set; }

        public bool CanClose()
        {
            return true;
        }

        private bool CanRegister()
        {
            return  !string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(ComfirmPassword);
        }

        [RelayCommand(CanExecute = nameof(CanRegister))]
        public void Register()
        {
            if (Password != ComfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (context.Users.FirstOrDefault(u => u.Login == Login) != null)
            {
                MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }


            context.Users.Add(new User
            {
                Login = Login,
                IsAdmin = false,
                Password = password,
               
            });
            context.SaveChanges();
            MessageBox.Show("Регистрация прошла успешно", "()", MessageBoxButton.OK, MessageBoxImage.Information);
            Close.Invoke();
        }
    }
}
