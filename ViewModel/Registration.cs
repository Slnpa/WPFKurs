using Npgsql;
using NpgsqlTypes;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp2.View;

namespace WpfApp2
{
    public class Registration : INotifyPropertyChanged
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
                OnPropertyChanged(nameof(Password));
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
        public RegistrationWindow window;
        public ICommand RegisterCommand { get; }
        public ICommand MoveToLoginCommand { get; }

        public Registration(RegistrationWindow wind)
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
            MoveToLoginCommand = new RelayCommand(MoveToLog);
            window = wind;
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        public void MoveToLog(object parameter)
        {
            AutorisationWindow AutoWind = new AutorisationWindow();
            AutoWind.Show();
            window.Close();
        }
        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public event EventHandler CanExecuteChanged;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

            public void Execute(object parameter) => _execute(parameter);

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        private bool CanRegister(object parameter)
        {
            // Implement your logic to determine if registration is allowed
            return true;
        }
        public int UserId;
        private void Register(object parameter)
        {
            Password = window.txtPassword.Password;

            if (string.IsNullOrEmpty(Username) || Username.Length < 5 || Username.Length > 15)
            {
                window.txtPassword.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFABADB3");
                ErrorMessage = "Длина логина должна быть в пределах 5-14 символов";
                window.txtUsername.BorderBrush = Brushes.Red;
            }
            else if (!char.IsLetter(Username[0]))
            {
                window.txtPassword.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFABADB3");
                window.txtUsername.BorderBrush = Brushes.Red;
                ErrorMessage = "Логин должен начинаться с буквы";
            }
            else if (!Username.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')))
            {
                window.txtPassword.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFABADB3");
                window.txtUsername.BorderBrush = Brushes.Red;
                ErrorMessage = "Логин должен содержать только латинские буквы или цифры";
            }
            else if (Password is null)
            {
                window.txtUsername.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFABADB3");
                window.txtPassword.BorderBrush = Brushes.Red;
                ErrorMessage = "Пароль должен содержать заглавные и строчные символы, а также число";
            }
            else if (!IsStrongPassword1(Password))
            {
                window.txtUsername.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFABADB3");
                window.txtPassword.BorderBrush = Brushes.Red;
                ErrorMessage = "Пароль должен содержать не менее 8 символов";
            }
            else if (!IsStrongPassword(Password))
            {
                window.txtUsername.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFABADB3");
                window.txtPassword.BorderBrush = Brushes.Red;
                ErrorMessage = "Пароль должен содержать заглавные и строчные символы латинского алфавита, а также число";
            }
            else if (ValidateUser(Username, Password))
            {
                window.txtUsername.BorderBrush = Brushes.Red;
                window.txtPassword.BorderBrush = Brushes.Red;
                ErrorMessage = "Пользователь уже создан";
            }
            else
            {
                RegisterUser(Username, HashPassword(Password));
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
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private static bool IsStrongPassword1(string password)
        {
            return password.Length >= 8;
        }
        private static bool IsStrongPassword(string password)
        {
            return password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit) && password.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'));
        }

        public static bool ValidateUser(string username, string password)
        {
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                int count = 0;
                string hashedPassword = HashPassword(password); // Хешируем введенный пароль

                // Проверяем, есть ли пользователь с введенным именем и паролем в базе данных

                using (var command = new NpgsqlCommand("SELECT count(*) FROM UserAccount WHERE username = @Username", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@Username", NpgsqlDbType.Varchar, username);
                    count = Convert.ToInt32(command.ExecuteScalar());
                }
                return count > 0;
            }
        }

        private void RegisterUser(string username, string password)
        {
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("Call register_user (@username_arg, @password_arg, @role_arg)", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@username_arg", NpgsqlDbType.Varchar, username);
                    command.Parameters.AddWithValue("@password_arg", NpgsqlDbType.Varchar, password);
                    command.Parameters.AddWithValue("@role_arg", NpgsqlDbType.Varchar, "1");
                    command.ExecuteNonQuery();
                }
            }
        }
        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Хеширование пароля с использованием SHA-256
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Преобразование хеша в строку для сохранения в базе данных
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
