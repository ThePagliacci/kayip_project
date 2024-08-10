//login
$(document).ready(function () {
    $('#login-submit').on('click', function (e) {
        e.preventDefault();
        var recaptcha = grecaptcha.getResponse();
        var errorMessage = $('#recaptcha-error');

        if(recaptcha === '')
        {
            errorMessage.text('reCAPTCHA doğrulamasında bir hata oluştu.').show();
        }
        else
        {
            $('#account').submit();
        }
    });
});

$(document).ready(function () {
    //regestration
    $('#registerSubmit').on('click', function (e) {
        e.preventDefault();
        var recaptcha = grecaptcha.getResponse();
        var errorMessage = $('#recaptcha-er');

        if(recaptcha === '')
        {
            errorMessage.text('reCAPTCHA doğrulamasında bir hata oluştu.').show();
        }
        else
        {
            $('#registerForm').submit();
        }
    });

});
