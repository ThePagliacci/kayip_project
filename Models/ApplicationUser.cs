using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;

namespace kayip_project.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string LName { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        [NotMapped]
        public string Role { get; set; }

    }
}