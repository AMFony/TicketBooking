using System;

namespace Ticket_booking
{
    public class Flight
    {
        public int fID { get; set; }
        public string airline_name { get; set; }
        public DateTime flight_date { get; set; }
        public string leave_from { get; set; }
        public string going_to { get; set; }
        public decimal ticket_price { get; set; }
        public bool seat_available { get; set; }
        public string airline_pic { get; set; }
    }
}