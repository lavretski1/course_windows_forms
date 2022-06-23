using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationProject.DAL.Models
{
    public class RentAccounting
    {
        public int Id { get; set; }
        public Tenant Tenant { get; set; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }
        public Room Room { get; set; }
        public bool BillCreated { get; set; }
        public bool Cancelled { get; set; }
    }
}
