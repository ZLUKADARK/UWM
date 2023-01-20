﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using UWM.BLL.Interfaces;
using UWM.Domain.DTO.Authentication;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UWM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthorizationServices _authorizationServices;
        private readonly IConfiguration _configuration;

        public AuthorizationController(SignInManager<IdentityUser> signInManager, IAuthorizationServices authorizationServices, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _authorizationServices = authorizationServices;
            _configuration = configuration;
        }

        // POST api/Authorization/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (ModelState.IsValid)
            {
                var result = await _authorizationServices.Login(login);
                if (result.Token != null)
                {
                    return Ok(new { Token = result.Token });
                }
                else if (result.Code != null)
                {
                    SendMailToConfirm(login.Email, result.Code);
                    return Content("Ваша регистрация не завршена. Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        private async void SendMailToConfirm(string email, string code)
        {

            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Authorization",
                new { userEmail = email, code = code },
                protocol: HttpContext.Request.Scheme);

            await _authorizationServices.SendEmailAsync(email, "Подтверждение аккаунта",
                $"Подтвердите сброс пароля, перейдя по ссылке: <a href='{callbackUrl}'>Подтвердить</a>");
        }

        // POST api/Authorization/Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        // POST api/Authorization/Logout
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(UserEmail email)
        {
            var code = await _authorizationServices.ForgotPassword(email);

            await _authorizationServices.SendEmailAsync(email.Email, "Ключ для сброса пароля", $"<h4>Ваш ключ: <code>{code}</code></h4> <br/> <h5>Вставьте данный ключ в соотвествующее поле в меню сброса пароля</h5>");
            return Ok("Вам на почту был выслан КЛЮЧ для сброса пароля");
        }

        // POST api/Authorization/Logout
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetUserPassword model)
        {
            if(!ModelState.IsValid)
                return BadRequest("Модель не правильно заполнина");
            return Ok(await _authorizationServices.ResetPassword(model));
        }

        // POST api/Authorization/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Registeration(Registration registration)
        {
            if (ModelState.IsValid)
            {
                var user = await _authorizationServices.Registration(registration);
                if (user != null)
                {
                    SendMailToConfirm(user.Email, user.Code);
                    return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<RedirectResult> ConfirmEmail(string userEmail, string code)
        {
            var url = _configuration.GetSection("CORS").Value.Split(",")[0];
            var result = await _authorizationServices.ConfirmEmail(userEmail, code);
            if (result == true)
            {
                await _authorizationServices.SendEmailAsync(userEmail, "Ваша аккаунт подтвержден", "Поздравляем вы успешно подтвердили свою учетную запись");
                return Redirect(url);
            }
            return Redirect(url+"/BadRequest");
        }
    }
}