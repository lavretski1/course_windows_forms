using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationProject.DAL.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public float Area { get; set; }
        public string Address { get; set; }
        public RoomType Type { get; set; }
        public bool Active { get; set; }
    }
}
