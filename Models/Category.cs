using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public String Name { get; set; }
    }
}