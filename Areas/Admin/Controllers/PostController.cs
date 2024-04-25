using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Data;
using kayip_project.Models;
using kayip_project.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

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
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> UsersList = _unitOfWork.ApplicationUser.GetAll().Select(u=> new SelectListItem
            {
                Text = u.FName,
                Value = u.Id.ToString()
            });

            ViewBag.UserList = UsersList;
            return View();
        }

       [HttpPost]
        public IActionResult Create(Post obj, IFormFile? file)
        {
           if(ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file !=null)
                {
                    string fileName = Guid.NewGuid().ToString()+ Path.GetExtension(file.FileName);
                    string postPath = Path.Combine(wwwRootPath, @"images\post");
                    using(var fileStream = new FileStream(Path.Combine(postPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Image = @"\images\post\" + fileName;
                }
                _unitOfWork.Post.Add(obj);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0) return NotFound();

            Post? postFromDb = _unitOfWork.Post.Get(u => u.Id == id);

            if(postFromDb == null) return NotFound();
            return View(postFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Post post, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file !=null)
                {
                    string fileName = Guid.NewGuid().ToString()+ Path.GetExtension(file.FileName);
                    string postPath = Path.Combine(wwwRootPath, @"images\post");

                    if(!string.IsNullOrEmpty(post.Image))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, post.Image.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using(var fileStream = new FileStream(Path.Combine(postPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    post.Image = @"\images\post\" + fileName;
                }
                _unitOfWork.Post.Update(post);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }  
            return View();          
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Post> postObj = _unitOfWork.Post.GetAll().ToList();
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