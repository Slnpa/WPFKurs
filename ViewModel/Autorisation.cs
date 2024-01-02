using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static WpfApp2.Registration;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using WpfApp2.View;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp2.ViewModel
{
    internal class Autorisation : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private string _errorMessage;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(window.txtPassword));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged(nameof(_visibility));
                }
            }
        }
        public AutorisationWindow window;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand LoginCommand { get; }
        public ICommand MoveToRegistrationCommand { get; }
        public Autorisation(AutorisationWindow wind)
        {
            LoginCommand = new RelayCommand(Login);
            MoveToRegistrationCommand = new RelayCommand(MoveToReg);
            window = wind;
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        public void MoveToReg(object parameter)
        {
            RegistrationWindow regwind = new RegistrationWindow();
            regwind.Show();
            window.Close();
        }
        public int UserId;
        private void Login(object parameter)
        {
            Password = window.txtPassword.Password;
            // Вызов процедуры регистрации пользователя
            if (Username == "" || Password == "")
            {
                window.txtUsername.BorderBrush = Brushes.Red;
                window.txtPassword.BorderBrush = Brushes.Red;
                Username = "";
                Password = "";
                ErrorMessage = "Неверный логин или пароль";
            }
            else if (ValidateUser(Username, Password))
            {
                if (Username == "admin")
                {
                    using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
                    {
                        connection.Open();
                        using (var command = new NpgsqlCommand("SELECT user_id FROM UserAccount WHERE username = @Username AND password = @Password", connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@Username", NpgsqlDbType.Varchar, Username);
                            command.Parameters.AddWithValue("@Password", NpgsqlDbType.Varchar, Password);
                            UserId = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    CatalogWindow mainWindow = new CatalogWindow(UserId);
                    mainWindow.Show();
                    window.Close();
                }
                else
                {
                    using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
                    {
                        connection.Open();
                        using (var command = new NpgsqlCommand("SELECT user_id FROM UserAccount WHERE username = @Username AND password = @Password", connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@Username", NpgsqlDbType.Varchar, Username);
                            command.Parameters.AddWithValue("@Password", NpgsqlDbType.Varchar, HashPassword(Password));
                            UserId = Convert.ToInt32(command.ExecuteScalar());
                        }
                    }
                    CatalogWindow mainWindow = new CatalogWindow(UserId);
                    mainWindow.Show();
                    window.Close();
                }
            }
            else
            {
                window.txtUsername.BorderBrush = Brushes.Red;
                window.txtPassword.BorderBrush = Brushes.Red;
                Username = "";
                Password = "";
                ErrorMessage = "Такого пользователя не существует";
            }
        }
        private static bool ValidateUser(string username, string password)
        {
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                int count = 0;
                string hashedPassword;
                if (username != "admin")
                {
                    hashedPassword = HashPassword(password);
                }
                else
                {
                    hashedPassword = password;
                }
                using (var command = new NpgsqlCommand("SELECT count(*) FROM UserAccount WHERE username = @Username AND password = @Password", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Username", NpgsqlDbType.Varchar, username);
                    command.Parameters.AddWithValue("@Password", NpgsqlDbType.Varchar, hashedPassword);
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
                return count > 0;
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
