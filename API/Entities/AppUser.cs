using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace API.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}