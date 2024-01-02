using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    public class Review
    {
        [Key]
        public int id_review { get; set; }
        public int id_mobile { get; set; }
        public int client_id { get; set; }
        public string description { get; set; }
        public string username { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
