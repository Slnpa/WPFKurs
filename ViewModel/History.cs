using System;
using System.Collections.Generic;
using System.Linq;
using static WpfApp2.Registration;
using WpfApp2.View;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using WpfApp2.Model;
using WpfApp2.Repositories;
using System.Windows.Media.Imaging;

namespace WpfApp2.ViewModel
{
    internal class History
    {
        public HistoryWindow window;
        public static int UserI;
        public ICommand MoveToCatalog { get; }
        public History(HistoryWindow wind, int use)
        {
            UserI = use;
            window = wind;
            PopulateMobileList();
            MoveToCatalog = new RelayCommand(MoveCatalog);
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        public void MoveCatalog(object parameter)
        {
            BasketWindow cat = new BasketWindow(UserI);
            cat.Show();
            window.Close();
        }
        //public class MobileDbContext : DbContext
        //{
        //    public DbSet<Ordert> Mobiles { get; set; }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseNpgsql("Server=localhost;User Id=postgres;Password=123;Database=kursach;");
        //    }
        //}

        public class MobileRepository
        {
            public List<Ordert> GetMobiles()
            {
                using (var context = new MobileDbContext())
                {
                    if(UserI == 34)
                    {
                        var mobiles = context.Orders.FromSqlRaw("select id_ordert, UserAccount.username, TO_CHAR(date_order,'YYYY-MM-DD-HH24:MI') as date_order, information_order, all_count, all_price from Ordert join UserAccount on Ordert.user_id = UserAccount.user_id").ToList();
                        return mobiles;
                    }
                    else
                    {
                        var mobiles = context.Orders.FromSqlRaw("select id_ordert, UserAccount.username, TO_CHAR(date_order,'YYYY-MM-DD-HH24:MI') as date_order, information_order, all_count, all_price from Ordert join UserAccount on Ordert.user_id = UserAccount.user_id where UserAccount.user_id = {0}", UserI).ToList();
                        return mobiles;
                    }
                }
            }
        }
        private void PopulateMobileList()
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.historyListView.ItemsSource = mobiles;
        }
    }
}
