using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket_booking
{
    public static class ConnectionHelper
    {
        public static string ConString
        {
            get
            {
                string dbPath = Path.Combine(Path.GetFullPath(@"..\..\"), "TBookingDB.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={dbPath};Initial Catalog=TeacherDb;Trusted_Connection=True";
            }
        }
    }
}
