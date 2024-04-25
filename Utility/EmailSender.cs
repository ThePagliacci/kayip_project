using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace kayip_project.Utility
{
    public class EmailSender: IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string body)
        {
            //logic to send email
            return Task.CompletedTask;
        }
    }
}