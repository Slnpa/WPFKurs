using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    public class Mobile
    {
        [Key]
        public int id_mobile_arg { get; set; }
        public string mobile_name_arg { get; set; }
        public int mobile_price_arg { get; set; }
        public string mobile_brend_arg { get; set; }
        public string mobile_os_arg { get; set; }
        public int mobile_date_arg { get; set; }
        public string mobile_image_url_arg { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool IsFavorite { get; set; }
    }
}
