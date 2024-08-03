// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;


namespace kayip_project.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
        }
        public string RecaptchaSiteKey => _configuration["reCAPTCHA:SiteKey"];
        public string RecaptchaSecreteKey => _configuration["reCAPTCHA:SecretKey"];



        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public string? RecaptchaToken { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required (ErrorMessage = "Şifre alanı zorunludur." )]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Beni Hatırlat?")]
            public bool RememberMe { get; set; }
        }
 
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }
 
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    int accessFailedCount = await _userManager.GetAccessFailedCountAsync(user);
                    int maxFailedAccessAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                    if(result.Succeeded)
                    {
                        _logger.LogInformation("Kullanıcı giriş yapmış.");
                        return LocalRedirect(returnUrl);
                    }
                    if(accessFailedCount + 1 >= maxFailedAccessAttempts)
                    {
                        ModelState.AddModelError(string.Empty, "Hesabınız kilitlenmeden önce son bir deneme hakkınız var.");
                        return Page();
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("Kullanıcı hesabı kilitlenmiş.");
                        return RedirectToPage("./Lockout");
                    }
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "E-posta adresiniz doğrulanmamış. Lütfen e-postanızı kontrol edin ve doğrulama işlemini gerçekleştirin.");
                        return Page();
                    }
                    else 
                    {
                        ModelState.AddModelError(string.Empty, "Parola yanlış.");
                        return Page();
                    }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}