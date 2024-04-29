using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using kayip_project.Models;
using kayip_project.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.Logging;

namespace kayip_project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MessageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public MessageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        { 
            return View();
        }

        public IActionResult Edit(int?id)
        {
            if(id==null || id == 0) return NotFound();
            Message message = _unitOfWork.Message.Get(u=>u.Id == id);
            if(message  == null) return NotFound();
            return View(message);
        }

        [HttpPost]
        public IActionResult Edit(Message message)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Message.Update(message);
                _unitOfWork.Save();
                return RedirectToAction("Index");

            }
            return View();
        }

        #region API CALLS

        [HttpGet]        
        public IActionResult GetAll()
        {
            List<Message> messageObj = _unitOfWork.Message.GetAll().ToList();
            return Json(new {data = messageObj });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var messageToBeDeleted = _unitOfWork.Message.Get(u=>u.Id == id);
            if(messageToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting Message"});
            }

            _unitOfWork.Message.Remove(messageToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully"});
        }
        
        #endregion

    }

}