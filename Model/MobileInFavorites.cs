using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    public class MobileInFavorites
    {
        [Key]
        public int id_mobile { get; set; }
        public string mobile_name { get; set; }
        public int mobile_price { get; set; }
        public string mobile_brend { get; set; }
        public string mobile_os { get; set; }
        public int mobile_date { get; set; }
        public string mobile_image_url { get; set; }
    }
}
