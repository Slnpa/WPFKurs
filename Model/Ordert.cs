using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    public class Ordert
    {
        [Key]
        public int id_ordert { get; set; }
        public string username { get; set; }
        public string date_order { get; set; }
        public string information_order { get; set; }
        public int all_count { get; set; }
        public int all_price { get; set; }
    }
}
