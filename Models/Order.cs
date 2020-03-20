using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"[0-9]+")]
        public String Number { get; set; }
        [Required]
        public String Address { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public int Total { get; set; }
    }
}