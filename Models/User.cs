using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public String Login { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        public Role Role { get; set; }
    }
}