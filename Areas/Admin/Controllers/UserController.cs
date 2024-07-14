using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Data;
using kayip_project.Models;
using kayip_project.Repository.IRepository;
using kayip_project.Utility;
using kayip_project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace kayip_project.Areas.Admin.Controllers.Admin
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin_Role)]
    public class UserController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            UserVM userVM = new()
            {
                ApplicationUser = (ApplicationUser)user,
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id, // Assuming i.Id is already a string
                    Selected = i.Name == userRole // Set Selected based on user's current role name
                }),
            };

            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserVM userVM)
        {
            string oldRole = (await _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userVM.ApplicationUser.Id))).FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userVM.ApplicationUser.Id);
            if (applicationUser != null)
            {
                applicationUser.FName = userVM.ApplicationUser.FName;
                applicationUser.LName = userVM.ApplicationUser.LName;
                applicationUser.City = userVM.ApplicationUser.City;
                applicationUser.District = userVM.ApplicationUser.District;
                applicationUser.Email = userVM.ApplicationUser.Email;

                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();
            }

            // Ensure role ID is valid
            string roleId = userVM.ApplicationUser.Role; // Assuming userVM.ApplicationUser.Role is the role ID

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role != null && role.Name != oldRole)
            {
                // Role exists and is different from the old role
                if (!string.IsNullOrEmpty(oldRole))
                {
                    await _userManager.RemoveFromRoleAsync(applicationUser, oldRole);
                }

                // Add the user to the new role by ID
                await _userManager.AddToRoleAsync(applicationUser, role.Name);

                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll().ToList();

            foreach(var user in userList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
            }

            return Json(new {data = userList});
        }

        [HttpDelete]
        public IActionResult Delete(string? id)
        {
            var userToBeDeleted = _userManager.FindByIdAsync(id).GetAwaiter().GetResult();
            if(userToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting post"});
            }
            // Retrieve user roles and remove them
            var roles = _userManager.GetRolesAsync(userToBeDeleted).GetAwaiter().GetResult();
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    var result = _userManager.RemoveFromRoleAsync(userToBeDeleted, role).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                    {
                        return Json(new { success = false, message = "Error while removing user roles" });
                    }
                }
            }
                // Delete the user
                var deleteResult = _userManager.DeleteAsync(userToBeDeleted).GetAwaiter().GetResult();
                if (!deleteResult.Succeeded)
                {
                    return Json(new { success = false, message = "Error while deleting user" });
                }
    return Json(new { success = true, message = "Deleted Successfully" });

        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(u=> u.Id == id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking"});
            }

            if(objFromDb.LockoutEnd !=null && objFromDb.LockoutEnd > DateTime.Now)
            {
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }
            _unitOfWork.ApplicationUser.Update(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Locked successfully"});
        }
        #endregion

    }
}