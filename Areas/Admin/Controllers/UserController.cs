using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Data;
using kayip_project.Models;
using kayip_project.Repository.IRepository;
using kayip_project.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace kayip_project.Areas.Admin.Controllers.Admin
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit(string? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            string RoleID = _db.UserRoles.FirstOrDefault(u=>u.UserId == id).RoleId;
            UserVM userVM = new()
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u =>u.Id == id),
                RoleList = _db.Roles.Select(x=>x.Name).Select(i=> new SelectListItem{
                    Text = i,
                    Value = i
                }),
            };
            userVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u=> u.Id == RoleID).Name;
            return View(userVM);
        }

         [HttpPost]
       public IActionResult Edit(UserVM userVM)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(u=> u.UserId == userVM.ApplicationUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u=>u.Id == RoleID).Name;
            ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u=> u.Id == userVM.ApplicationUser.Id);
            _db.ApplicationUsers.Update(applicationUser);
            if(applicationUser != null)
            {
                applicationUser.FName = userVM.ApplicationUser.FName;
                applicationUser.LName = userVM.ApplicationUser.LName;
                applicationUser.City = userVM.ApplicationUser.City;
                applicationUser.District = userVM.ApplicationUser.District;
                applicationUser.Email = userVM.ApplicationUser.Email;
            }
            _db.SaveChanges();

            if(!(userVM.ApplicationUser.Role == oldRole))
            {
                //a role was updated
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, userVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }

            return RedirectToAction("Index");
        }
    
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _db.ApplicationUsers.ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach(var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u=>u.UserId == user.Id).RoleId;

                user.Role = roles.FirstOrDefault(u=>u.Id == roleId).Name;
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

            //removing the user's role
            string RoleID = _db.UserRoles.FirstOrDefault(u=> u.UserId == id).RoleId;
            string Role = _db.Roles.FirstOrDefault(u=>u.Id == RoleID).Name;
            _userManager.RemoveFromRoleAsync(userToBeDeleted, Role).GetAwaiter().GetResult();

             //removing the user
            _userManager.DeleteAsync(userToBeDeleted).GetAwaiter().GetResult();

            return Json(new { success = true, message = "Deleted Successfully"});
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u=> u.Id == id);
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
            _db.SaveChanges();

            return Json(new { success = true, message = "Locked successfully"});
        }
        #endregion

    }
}