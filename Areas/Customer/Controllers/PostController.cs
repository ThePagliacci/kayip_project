using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
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

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            List<Post> postObj = _unitOfWork.Post.GetAll(includeProperties: "ApplicationUser")
                .Where(post => post.ApplicationUserId == userId)
                .ToList();

            return View(postObj);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Post post = new Post();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //create
             if(id == null || id == 0) 
             {
               post.ApplicationUserId = userId;
                return View(post);
             }
             else
             {
                //update
                post = _unitOfWork.Post.Get(u => u.Id == id && u.ApplicationUserId == userId);
                return View(post);
             }
        }
    
        [HttpPost]
        public IActionResult Upsert(Post post, IFormFile? file)
        {
            if (file == null && post.Image == null)
            {
                ModelState.AddModelError("Image", "Bu alanın doldurulması zorunludur");
            }
            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string postPath = Path.Combine(wwwRootPath, "images", "post");

                    if (!string.IsNullOrEmpty(post.Image))
                    {
                        string sanitizedImagePath = Regex.Replace(post.Image, @"\.\.[\\/]", string.Empty);
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
                    post.Image = Path.Combine("images", "post", fileName).Replace("\\", "/");
                }
                post.Latitude = post.Latitude; 
                post.Longitude = post.Longitude; 
                if (post.Id == 0)
                {
                    _unitOfWork.Post.Add(post);
                }
                else
                {
                    _unitOfWork.Post.Update(post);
                }

                _unitOfWork.Save();
                return RedirectToAction("Index", "Home");
            }
            return View(post);
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