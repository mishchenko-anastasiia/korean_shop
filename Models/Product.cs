using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public Category Category { get; set; }
        [Required]
        public Brand Brand { get; set; }
        [Required]
        public int Price { get; set; }
        public String ImagePath { get; set; }
    }
}