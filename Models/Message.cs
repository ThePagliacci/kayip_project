using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kayip_project.Models
{
    public class Message
    { 
        [Key]
        public int Id { get; set; }
        [Required (ErrorMessage = "E posta alanı zorunludur." )]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Konu alanı zorunludur." )]

        public string Subject { get; set; }
        [Required (ErrorMessage = "Mesaj alanı zorunludur." )]
        public string Body { get; set; }
    }
}