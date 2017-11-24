using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Lab1_CRUD
{
    public class Director
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public DateTime Birthday { get; set; }

        public string DirectorInfo
        {
            get
            {
                return $"{Fullname} (ID: {Id})";
            }
        }
    }
}
