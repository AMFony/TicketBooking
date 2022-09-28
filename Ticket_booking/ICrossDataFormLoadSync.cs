using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket_booking
{
    public interface ICrossDataFormLoadSync
    {
        void ReloadData(List<Flight> flight);
        void UpdateFlight(Flight flight);
        void RemoveFlight(int id);
        void ReloadPassenger(List<Passenger> passenger);
        void UpdatePassenger(Passenger p);
        void RemovePassenger(int id);
    }
}
