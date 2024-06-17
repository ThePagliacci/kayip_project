using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Data;
using kayip_project.Models;
using kayip_project.Repository.IRepository;
using kayip_project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace kayip_project.Areas.Admin.Controllers.Admin
{
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //access the root folder
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Post> postObj = _unitOfWork.Post.GetAll(includeProperties: "ApplicationUser").ToList();
            return View(postObj);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            PostVM postVM = new()
            {
                UserList = _unitOfWork.ApplicationUser.GetAll().Select(u=> new SelectListItem
                {
                    Text = u.FName,
                    Value = u.Id.ToString()
                }),
                Post = new Post()
            };
            //create
             if(id == null || id == 0) return View(postVM);
             else
             {
                //update
                postVM.Post = _unitOfWork.Post.Get(u => u.Id == id);
                return View(postVM);

             }
        }

        [HttpPost]
        public IActionResult Upsert(PostVM postVM, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file !=null)
                {
                    string fileName = Guid.NewGuid().ToString()+ Path.GetExtension(file.FileName);
                    string postPath = Path.Combine(wwwRootPath, @"images\post");

                    if(!string.IsNullOrEmpty(postVM.Post.Image))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, postVM.Post.Image.TrimStart('\\'));
                        string fullPath = Path.GetFullPath(oldImagePath);
                        string pattern = "^" + Regex.Escape(wwwRootPath) + @"(?:\\|/|$)";

                        if (Regex.IsMatch(fullPath, pattern, RegexOptions.IgnoreCase) && System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using(var fileStream = new FileStream(Path.Combine(postPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    postVM.Post.Image = @"\images\post\" + fileName;
                }
                if(postVM.Post.Id == 0)
                {
                    _unitOfWork.Post.Add(postVM.Post);
                }
                else
                {
                    _unitOfWork.Post.Update(postVM.Post);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }
        
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Post> postObj = _unitOfWork.Post.GetAll(includeProperties: "ApplicationUser").ToList();
            return Json(new {data = postObj });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var postToBeDeleted = _unitOfWork.Post.Get(u=>u.Id == id);
            if(postToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting post"});
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, postToBeDeleted.Image.TrimStart('\\'));

            if(System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Post.Remove(postToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully"});
        }
        
        #endregion
    }
}