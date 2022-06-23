using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationProject.DAL.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LegalAddress { get; set; }
        public string BankName { get; set; }
        public string Head { get; set; }
        public string Characteristic { get; set; }
        public List<Bill> Bills { get; set; }
        public List<RentAccounting> Rents { get; set; }
        public string Username { get; set; }
        public bool HasAccess { get; set; }
    }
}
