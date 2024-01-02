using System;
using System.Collections.Generic;
using System.Linq;
using WpfApp2.View;
using static WpfApp2.Registration;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WpfApp2.Model;
using WpfApp2.Repositories;
using System.Windows.Media;

namespace WpfApp2.ViewModel
{
    internal class MobileInfo
    {
        public MobileInfoWindow window;
        public ICommand Return { get; }
        public ICommand PubComm { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ClosePop { get; }


        public Mobile mmm;
        public static int mobid;
        public static int Useri;
        public MobileInfo(MobileInfoWindow wind, Mobile selectedMobile, int userI)
        {
            Useri = userI;
            mmm= selectedMobile;
            window = wind;
            mobid = selectedMobile.id_mobile_arg;
            window.mobileNameTextBlock.Text = selectedMobile.mobile_name_arg;
            window.mobilePriceTextBlock.Text = selectedMobile.mobile_price_arg.ToString()+" рублей";
            // Преобразование строки URL в ImageSource
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(selectedMobile.mobile_image_url_arg);
            bitmap.EndInit();
            window.mobileImg.Source = bitmap;
            window.mobileDate.Text = "Дата выхода: " + selectedMobile.mobile_date_arg.ToString();
            window.mobileOs.Text = "Операционная система: " + selectedMobile.mobile_os_arg;
            window.mobileBrend.Text = "Производитель: " + selectedMobile.mobile_brend_arg;
            PopulateMobileList();
            Return = new RelayCommand(Back);
            PubComm = new RelayCommand(Publish);
            DeleteCommand = new RelayCommand(DeleteComment);
            UpdateCommand = new RelayCommand(UpdateComment);
            ClosePop = new RelayCommand(ClosePopUp);
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        public MobileInfo(MobileInfoWindow wind)
        {
            window = wind;
            Return = new RelayCommand(Back);
            PubComm = new RelayCommand(Publish);
            DeleteCommand = new RelayCommand(DeleteComment);
            UpdateCommand = new RelayCommand(UpdateComment);
            ClosePop = new RelayCommand(ClosePopUp);
        }
        private string enteredText;
        private void ClosePopUp(object parameter)
        {
            window.MyPopup.IsOpen = false;
            enteredText = window.MyTextBox.Text;
            Uod(ppp);
            enteredText = "";
        }
        private object ppp;
        private void Uod(object parameter)
        {
            var item = parameter as Review;
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("update Reviews set description = @description where id_review=@id_review", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@description", NpgsqlDbType.Varchar, enteredText);
                    command.Parameters.AddWithValue("@id_review", NpgsqlDbType.Integer, item.id_review);
                    command.ExecuteScalar();
                }
            }
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.reviewListView.ItemsSource = mobiles;
        }
        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        private void UpdateComment(object parameter)
        {
            var item = parameter as Review;
            var button = FindChild<TextBlock>(window.reviewListView.ItemContainerGenerator.ContainerFromItem(item), "CommentButton");
            window.MyPopup.PlacementTarget = button;
            window.MyPopup.IsOpen = true;
            window.MyTextBox.Text = item.description;
            ppp = parameter;
        }
        public void Back(object parameter)
        {
            CatalogWindow windof = new CatalogWindow(Useri);
            windof.Show(); 
            window.Close(); 
        }
        //public class MobileDbContext : DbContext
        //{
        //    public DbSet<Review> Mobiles { get; set; }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseNpgsql("Server=localhost;User Id=postgres;Password=123;Database=kursach;");
        //    }
        //}

        public class MobileRepository
        {
            public List<Review> GetMobiles()
            {
                using (var context = new MobileDbContext())
                {
                    if (Useri == 34)
                    {
                        var mobiles = context.Reviews.FromSqlRaw("select *, true as IsCurrentUser from reviews join useraccount on reviews.client_id = useraccount.user_id where id_mobile={0}", mobid).ToList();
                        return mobiles;
                    }
                    else
                    {
                        var mobiles = context.Reviews.FromSqlRaw("select *, client_id = {1} as IsCurrentUser from reviews join useraccount on reviews.client_id = useraccount.user_id where id_mobile={0}", mobid, Useri).ToList();
                        return mobiles;
                    }
                }
            }
        }
        private void PopulateMobileList()
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.reviewListView.ItemsSource = mobiles;
           
        }

        private void Publish(object parameter)
        {
            if(window.Comment.Text!="")
            {
                using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("call add_review_to_mobile(@idmobile,@clientid,@description)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@idmobile", NpgsqlDbType.Integer, mobid);
                        command.Parameters.AddWithValue("@clientid", NpgsqlDbType.Integer, Useri);
                        command.Parameters.AddWithValue("@description", NpgsqlDbType.Varchar, window.Comment.Text);
                        command.ExecuteScalar();
                    }
                }
            }
            window.Comment.Text = "";
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.reviewListView.ItemsSource = mobiles;
        }
        private void DeleteComment(object parameter)
        {
            var item = parameter as Review;
            if (item != null)
            {
                using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("delete from Reviews where id_review = @id_review", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id_review", NpgsqlDbType.Integer, item.id_review);
                        command.ExecuteScalar();
                    }
                }
            }
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.reviewListView.ItemsSource = mobiles;
        }
        //public class BooleanToVisibilityConverter : IValueConverter
        //{
        //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        if (value is bool && (bool)value)
        //        {
        //            return Visibility.Visible;
        //        }
        //        return Visibility.Collapsed;
        //    }

        //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
