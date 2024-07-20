using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using kayip_project.Models;
using kayip_project.Repository.IRepository;
using kayip_project.Utility;
using kayip_project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace kayip_project.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PostController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostController(ILogger<PostController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string? id)
        {
            List<Post> postObj = _unitOfWork.Post.GetAll().Where(post => post.ApplicationUser != null && post.ApplicationUser.Id == id).ToList();
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
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string postPath = Path.Combine(wwwRootPath, "images", "post");

                    if (!string.IsNullOrEmpty(postVM.Post.Image))
                    {
                        string sanitizedImagePath = Regex.Replace(postVM.Post.Image, @"\.\.[\\/]", string.Empty);
                        string oldImagePath = Path.Combine(wwwRootPath, sanitizedImagePath);

                        if (Path.GetFullPath(oldImagePath).StartsWith(wwwRootPath, StringComparison.OrdinalIgnoreCase))
                        {
                            var fileInfo = new System.IO.FileInfo(oldImagePath);
                            if (fileInfo.Exists)
                            {
                                fileInfo.Delete();
                            }
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(postPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    postVM.Post.Image = Path.Combine("images", "post", fileName).Replace("\\", "/");
                }
                if (postVM.Post.Id == 0)
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
            return View(postVM);
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