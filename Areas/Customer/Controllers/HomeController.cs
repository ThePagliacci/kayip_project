using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using kayip_project.Models;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using kayip_project.Repository.IRepository;

namespace kayip_projectA.Areas.Customer.Controllers;
    [Area("Customer")]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ContactUs()
    {
        return View(); 
    }

    [HttpPost]
    public IActionResult ContactUs(Message message)
    {
        if(ModelState.IsValid)
        {
            MailMessage mail = new MailMessage("boshra.khaled@outlook.sa", "boshra.khaled@outlook.sa");
            mail.Subject = message.Subject;
            mail.Body = "User Email: " +  message.Name +"<br>User Massage: "+ message.Body;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.office365.com";
            smtp.Port= 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("boshra.khaled@outlook.sa", "boshraROKAYA");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mail);

            _unitOfWork.Message.Add(message);
            _unitOfWork.Save();

            ViewBag.Message = "Mail sent successfully";
            ModelState.Clear();
            return View();
        }
        return View("index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
