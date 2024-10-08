using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
namespace kayip_project.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Bu alanın doldurulması zorunludur" )]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Bu alanın doldurulması zorunludur" )]
        public string Description { get; set; }
        [Required(ErrorMessage = "Bu alanın doldurulması zorunludur" )]
        [Display(Name = "Contact Info")]
        public string ContactInfo { get; set; }
        [ValidateNever]
        [Required(ErrorMessage = "Bu alanın doldurulması zorunludur" )]
        public string Image { get; set; }

        [Required(ErrorMessage = "Bu alanın doldurulması zorunludur" )]
        public double Latitude { get; set; }
        [Required(ErrorMessage = "Bu alanın doldurulması zorunludur" )]
        public double Longitude { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        
    }
}
