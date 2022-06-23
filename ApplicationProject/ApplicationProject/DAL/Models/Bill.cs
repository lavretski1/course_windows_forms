using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationProject.DAL.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public float Ammount { get; set; }
        public float Tax { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public float FinePerDay { get; set; }
        public DateTime? PaymentDate { get; set; }
        public RentAccounting Rent { get; set; }
    }
}
