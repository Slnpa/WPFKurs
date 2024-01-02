using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using WpfApp2.View;
using static WpfApp2.Registration;
using WpfApp2.Model;
using WpfApp2.Repositories;
using System.Windows.Media.Imaging;

namespace WpfApp2.ViewModel
{
    internal class Favourites
    {
        FavouritesWindow window;
        private static int UserI;
        public ICommand MoveToCatalog { get; }
        public ICommand DeleteFromFav { get; }
        
        public Favourites(FavouritesWindow wind, int UserId)
        {
            UserI = UserId;
            window = wind;
            PopulateMobileList();
            MoveToCatalog = new RelayCommand(MoveCatalog);
            DeleteFromFav = new RelayCommand(DeleteFav);
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        private void DeleteFav(object paremeter)
        {
            var item = paremeter as MobileInFavorites;
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("CALL DELETE_MOBILE_FROM_FAVOURITES(@id_mobile, @id_user)", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile);
                    command.Parameters.AddWithValue("@id_user", NpgsqlDbType.Integer, UserI);
                    command.ExecuteScalar();
                }
            }
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
        }
        public void MoveCatalog(object parameter)
        {
            CatalogWindow cat = new CatalogWindow(UserI);
            cat.Show();
            window.Close();
        }
        //public class MobileDbContext : DbContext
        //{
        //    public DbSet<MobileInFavorites> Mobiles { get; set; }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseNpgsql("Server=localhost;User Id=postgres;Password=123;Database=kursach;");
        //    }
        //}

        public class MobileRepository
        {
            public List<MobileInFavorites> GetMobiles()
            {
                using (var context = new MobileDbContext())
                {
                    var mobiles = context.Favorites.FromSqlRaw("select Mobile.id_mobile, Mobile.mobile_name, Mobile.mobile_price, Mobile.mobile_brend,Mobile.mobile_os, Mobile.mobile_date, Mobile.mobile_image_url from Favorites join Mobile on Favorites.id_mobile = Mobile.id_mobile where user_id = {0}", UserI).ToList();
                    return mobiles;
                }
            }
        }
        private void PopulateMobileList()
        {

            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
        }
    }
}
