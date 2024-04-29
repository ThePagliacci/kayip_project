using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace kayip_project.ViewModels
{
    public class UserVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}