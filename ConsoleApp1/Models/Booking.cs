using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime DateBooking { get; set; }
        public DateTime DateDelivery { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product Product { get; set; }


    }
}
