// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading; 
using System.Threading.Tasks;
using AspNetCore.ReCaptcha;
using kayip_project.Models;
using kayip_project.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace kayip_project.Areas.Identity.Pages.Account
{
    [ValidateReCaptcha("Register", ErrorMessage ="reCAPTCHA doğrulaması başarısız oldu.")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _configuration = configuration;
        }
        public string RecaptchaSiteKey => _configuration["reCAPTCHA:SiteKey"];
        public string RecaptchaSecreteKey => _configuration["reCAPTCHA:SecretKey"];
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "E-posta alanı zorunludur.")]
            [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
            [Display(Name = "E posta")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required (ErrorMessage = "Şifre alanı zorunludur." )]
            [StringLength(100, ErrorMessage = "{0} en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Required (ErrorMessage = "Şifre onaylama alanı zorunludur." )]
            [Display(Name = "Şifre Onaylama")]
            [Compare("Password", ErrorMessage = "Şifre ve onay şifresi eşleşmiyor.")]
            public string ConfirmPassword { get; set; }

            public string? Role { get; set; }
            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

            [Required (ErrorMessage = "Adı alanı zorunludur." )]
            public string FName { get; set; }
            [Required (ErrorMessage = "Soyadı alanı zorunludur." )]
            public string LName { get; set; }
            public string? City { get; set; }
            public string? District { get; set; }

            [Required(ErrorMessage = "Lütfen Şartlar ve Hizmetleri kabul edin.")]
            public bool TermsAccepted { get; set; }

            [Required(ErrorMessage = "Lütfen Gizlilik Politikası Şartlarını kabul edin.")]
            public bool PolicyAccepted { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            //creating user roles
            if(!_roleManager.RoleExistsAsync(SD.User_Role).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.User_Role)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Admin_Role)).GetAwaiter().GetResult();
            }

            Input = new()
            {
                RoleList = _roleManager.Roles.Select(x=>x.Name).Select(i=> new SelectListItem{
                    Text = i,
                    Value = i
                })
            };
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!Input.TermsAccepted)
            {
               ModelState.AddModelError(string.Empty, "Şartlar ve Hizmetleri kabul etmelisiniz.");
            }
            if (!Input.PolicyAccepted)
            {
              ModelState.AddModelError(string.Empty, "Gizlilik Politikası Şartlarını kabul etmelisiniz.");
            }     
            else if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.FName = Input.FName;
                user.LName = Input.LName;
                user.City = Input.City;
                user.District = Input.District;
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı şifre ile yeni bir hesap oluşturmuştur.");

                    if(!String.IsNullOrEmpty(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.User_Role);
                    }
 
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await SendEmailAsync(Input.Email, "E-postanızı onaylayın",
                        $"<!DOCTYPE html><html><body style='text-align: center;'><h1>Yeni Kullanıcıyı Hoş Geldiniz</h1><p>Lütfen aşağidaki̇ bağlantiya tiklayarak e-posta adresi̇ni̇zi̇ onaylayin, böylece web si̇tesi̇ne rahatça gi̇ri̇ş yapabi̇li̇r ve en son yayinlar i̇çi̇n taki̇pte kalabi̇li̇rsi̇ni̇z.</p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Buraya Tıklayın</a></body></html>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError(string.Empty, "Bu e-posta adresi zaten kullanılmaktadır.");
                        break; // Exit the loop after adding the specific error message
                    }
                    else if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError(string.Empty, "Parola yeterince güçlü değil.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                    }

                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        {
            var emailUsername = _configuration["Email:Username"];
            var emailPassword = _configuration["Email:Password"];
            try
            {
 
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                message.From = new MailAddress(emailUsername);
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = confirmLink;
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port= 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                NetworkCredential nc = new NetworkCredential(emailUsername, emailPassword);
                smtpClient.Credentials = nc;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);

                return true;
            }
            catch(Exception)
            {
                return false;
            }

        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"'{nameof(IdentityUser)}' örneği oluşturulamıyor. " + $"{nameof(IdentityUser)}' öğesinin soyut bir sınıf olmadığından ve parametresiz bir kurucuya sahip olduğundan emin olun veya alternatif olarak " + $"/Areas/Identity/Pages/Account/Register.cshtml içindeki kayıt sayfasını geçersiz kılın");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Standart kullanıcı arayüzü e-posta destekli bir kullanıcı belleği gerektirir.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

    }
}
