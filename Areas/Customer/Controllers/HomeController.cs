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
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _configuration = configuration;
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
        public async Task<ActionResult> ContactUs(Message message)
        {
            var emailUsername = _configuration["Email:Username"];
            var emailPassword = _configuration["Email:Password"];
            if(ModelState.IsValid)
            {
                MailMessage mail = new MailMessage(message.Name, emailUsername);
                mail.Subject = "Contact Us Message!!";
                mail.Body ="User Email: " +  message.Name +"<br>User Massage: "+ message.Subject +"<br>" + message.Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host =  "smtp.gmail.com";
                smtp.Port= 587;
                smtp.EnableSsl = true;

                NetworkCredential nc = new NetworkCredential(emailUsername, emailPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

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
}