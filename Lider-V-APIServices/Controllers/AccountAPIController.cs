using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services;
using Lider_V_APIServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Encodings.Web;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Lider_V_APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountAPIController : Controller
    {
        private readonly UserManager<User> _userManager;
        private IAccountRepository _accountRepository;
        private readonly ApplicationDbContext _cotnext;
        protected ResponseDto _response;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public AccountAPIController(UserManager<User> userManager, IAccountRepository accountRepository, ApplicationDbContext cotnext, ApplicationDbContext context, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
            this._response = new ResponseDto();
            _cotnext = cotnext;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpPost("Login")]
        public async Task<object> Login(Login login)
        {
            try
            {
                var user = await _userManager
                .FindByNameAsync(login.LoginName) ?? await _userManager.FindByEmailAsync(login.LoginEmail);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Не правильный логин или пароль";
                    return StatusCode(400, _response);
                }
                else
                {
                    var isPasswordValid = await _userManager.CheckPasswordAsync(user, login.LoginPassword);

                    if (!isPasswordValid)
                    {
                        _response.IsSuccess = false;
                        _response.Result = "Не правильный логин или пароль";
                        return StatusCode(400, _response);
                    }

                    var token = await _accountRepository.GenerateJwtTokenByUser(user);
                    user.LastLoginDate = DateTime.UtcNow;
                    await _cotnext.SaveChangesAsync();
                    _response.Result = token;
                    return StatusCode(200, _response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("Register")]
        public async Task<object> Register(Register register)
        {
            try
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(register.RegisterEmail);

                if (existingUserByEmail != null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данный email уже зарегистрирован";
                    return StatusCode(406, _response);

                }

                var existingUserByName = await _userManager.FindByNameAsync(register.RegisterLogin);

                if (existingUserByName != null)
                {
                    if (existingUserByEmail != null)
                    {
                        _response.IsSuccess = false;
                        _response.Result = "Данный логин уже зарегистрирован";
                        return StatusCode(406, _response);
                    }
                }

                //if (string.IsNullOrWhiteSpace(register.RegisterPassword) ||
                //    register.RegisterPassword.Length < 4 ||
                //    !register.RegisterPassword.Any(char.IsUpper) ||
                //    !register.RegisterPassword.Any(char.IsDigit) ||
                //    !register.RegisterPassword.Any(char.IsSymbol))
                //{
                //    _response.IsSuccess = false;
                //    _response.Result = "Пароль должен содержать как минимум 4 символа, хотя бы одну заглавную букву, одну цифру и один специальный символ.";
                //    return StatusCode(400, _response);
                //}


                var user = new User
                {
                    Email = register.RegisterEmail,
                    UserName = register.RegisterLogin,
                    UserFirstName = register.RegisterFirstName,
                    UserLastName = register.RegisterLastName,
                    RegistrationDate = DateTime.UtcNow, // Устанавливаем дату регистрации
                };

                var result = await _userManager.CreateAsync(user, register.RegisterPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.UserRoleName);
                    //user.LastLoginDate = DateTime.UtcNow;
                    await _cotnext.SaveChangesAsync();
                    _response.Result = "Регистрация прошла успешно.";
                }

                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }



        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не авторизован или не найден";
                    return StatusCode(401, _response);
                }

                if (await _userManager.IsInRoleAsync(user, Constants.AdminRoleName))
                {
                    var users = _userManager.Users.ToList();

                    var userDtoList = users.Select(user => new
                    {
                        user.Id,
                        user.UserName,
                        user.UserFirstName,
                        user.UserLastName,
                        user.Email,
                        user.RegistrationDate,
                        user.LastLoginDate
                    }).ToList();

                    _response.Result = userDtoList;
                    return StatusCode(200, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данная функция доступна только для администратора";
                    return StatusCode(403, _response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUser")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не найден";
                    return StatusCode(404, _response);
                }

                var activeOrders = _cotnext.Orders
                    .Where(o => o.UserId == user.Id && o.Status != OrderStatus.Delivered)
                    .ToList();

                UserInfoDto userInfo = new UserInfoDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserFirstName = user.UserFirstName,
                    UserLastName = user.UserLastName,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    Orders = activeOrders
                };

                _response.Result = userInfo;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUserById/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не найден";
                    return StatusCode(404, _response);
                }

                var activeOrders = _cotnext.Orders
                    .Where(o => o.UserId == userId && o.Status != OrderStatus.Delivered)
                    .ToList();

                UserInfoDto userInfo = new UserInfoDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserFirstName = user.UserFirstName,
                    UserLastName = user.UserLastName,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    Orders = activeOrders
                };

                _response.Result = userInfo;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUserByEmail/{email}")]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не найден";
                    return StatusCode(404, _response);
                }

                var activeOrders = _cotnext.Orders
                    .Where(o => o.UserId == user.Id && o.Status != OrderStatus.Delivered)
                    .ToList();

                UserInfoDto userInfo = new UserInfoDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserFirstName = user.UserFirstName,
                    UserLastName = user.UserLastName,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    Orders = activeOrders
                };

                _response.Result = userInfo;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("GetUserByUserName/{userName}")]
        [Authorize]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пользователь не найден";
                    return StatusCode(404, _response);
                }

                var activeOrders = _cotnext.Orders
                    .Where(o => o.UserId == user.Id && o.Status != OrderStatus.Delivered)
                    .ToList();

                UserInfoDto userInfo = new UserInfoDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserFirstName = user.UserFirstName,
                    UserLastName = user.UserLastName,
                    Email = user.Email,
                    RegistrationDate = user.RegistrationDate,
                    LastLoginDate = user.LastLoginDate,
                    Orders = activeOrders
                };

                _response.Result = userInfo;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("ChangeUserLogin")]
        [Authorize]
        public async Task<IActionResult> ChangeUserLogin(string newUserLogin)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данного аккаунта нет в системе";

                    return StatusCode(404, _response);
                }

                var existingUserWithEmail = await _userManager.FindByNameAsync(newUserLogin);

                if (existingUserWithEmail != null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Ошибка при смене почты: указанный логин уже используется другим аккаунтом";
                    return StatusCode(400, _response);
                }

                if (newUserLogin == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Ошибка при смене логина";

                    return StatusCode(400, _response);
                }
                else
                {
                    user.UserName = newUserLogin;
                    await _userManager.UpdateAsync(user);
                    await _signInManager.RefreshSignInAsync(user);
                    await _context.SaveChangesAsync();
                    _response.Result = newUserLogin;

                    return StatusCode(200, _response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("ChangeUserEmail")]
        [Authorize]
        public async Task<IActionResult> ChangeUserEmail(string newEmail)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данного аккаунта нет в системе";
                    return StatusCode(404, _response);
                }

                if (string.IsNullOrEmpty(newEmail))
                {
                    _response.IsSuccess = false;
                    _response.Result = "Ошибка при смене почты: новый email не может быть пустым";
                    return StatusCode(400, _response);
                }

                var existingUserWithEmail = await _userManager.FindByEmailAsync(newEmail);

                if (existingUserWithEmail != null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Ошибка при смене почты: указанный email уже используется другим аккаунтом";
                    return StatusCode(400, _response);
                }

                var code = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
                var callBackUrl = $"https://lider-filament.ru/confirmemail/?&newEmail={newEmail}&code={code}";
                //var callbackUrl = Url.Action("ConfirmEmailChange", "Account", new { email = newEmail, code }, protocol: HttpContext.Request.Scheme);

                EmailSender emailSender = new EmailSender();

                await emailSender.SendEmailAsync(newEmail, "Подтверждение смены почты",
                    $"Для завершения смены почты перейдите по ссылке: <a href='{HtmlEncoder.Default.Encode(callBackUrl)}'>подтвердить email</a>");

                _response.Result = "На новый email было отправлено письмо для подтверждения смены почты";

                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpGet("ConfirmEmailChanged")]
        public async Task<IActionResult> ConfirmEmailChanged(string email, string code)
        {
            if (email == null || code == null)
            {
                _response.IsSuccess = false;
                _response.Result = "Неверные параметры для подтверждения email";

                return StatusCode(400, _response);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _response.IsSuccess = false;
                _response.Result = "Пользователь не найден";

                return StatusCode(404, _response);
            }

            var result = await _userManager.ChangeEmailAsync(user, email, code);
            var confirmEmail = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                _response.Result = "Почта успешно подтверждена";

                return StatusCode(200, _response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = "Ошибка при подтверждении почты";

                return StatusCode(500, _response);
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email, string code)
        {
            if (email == null || code == null)
            {
                _response.IsSuccess = false;
                _response.Result = "Неверные параметры для подтверждения email";

                return StatusCode(400, _response);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _response.IsSuccess = false;
                _response.Result = "Пользователь не найден";

                return StatusCode(404, _response);
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                _response.Result = "Почта успешно подтверждена";

                return StatusCode(200, _response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = "Ошибка при подтверждении почты";

                return StatusCode(500, _response);
            }
        }

        [HttpPost("ChangeUserName")]
        [Authorize]
        public async Task<IActionResult> ChangeUserName(string newFirstName, string newLastName)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данного аккаунта нет в системе";
                    return StatusCode(404, _response);
                }

                if (newFirstName == null || newLastName == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Ошибка при смене логина";
                    return StatusCode(400, _response);
                }
                else
                {
                    user.UserFirstName = newFirstName;
                    user.UserLastName = newLastName;
                    _response.Result = newFirstName + " " + newLastName;
                    await _context.SaveChangesAsync();
                    return StatusCode(200, _response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данного аккаунта нет в системе";
                    return StatusCode(404, _response);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                EmailSender emailSender = new EmailSender();

                //var callBackUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/AccountAPI/ResetPasswordConfirm?userEmail={user.Email}&code={encodedToken}";
                var callBackUrl = $"https://lider-filament.ru/resetpassword?&userEmail={user.Email}&code={encodedToken}";


                //var callBackUrl = Url.Action(
                //    "ResetPassword",
                //    "Account",
                //    new
                //    {
                //        userId = user.Id,
                //        code = encodedToken,
                //    },
                //    protocol: HttpContext.Request.Scheme);

                await emailSender.SendEmailAsync(
                    model.Email,
                    "Сброс пароля",
                    $"Это сообщение сгенерировано автоматически и на него не нужно отвечать. Если вы получили это сообщение по ошибке, удалите его. " +
                    $"Перейдите по ссылке чтобы сбросить свой пароль: {callBackUrl}");

                _response.Result = "Ссылка на сброс пароля отправлена на вашу почту.";
                return StatusCode(200, _response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = "Некорректный запрос";
                return StatusCode(400, _response);
            }
        }

        [HttpPost("ResetPasswordConfirm")]
        public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Данного аккаунта нет в системе";
                    return StatusCode(400, _response);
                }

                var decodedToken = WebEncoders.Base64UrlDecode(model.Code);
                var token = Encoding.UTF8.GetString(decodedToken);

                // Проверка, совпадают ли новый пароль и подтверждение пароля
                if (model.Password != model.ConfirmPassword)
                {
                    _response.IsSuccess = false;
                    _response.Result = "Новый пароль и подтверждение пароля не совпадают.";
                    return StatusCode(400, _response);
                }

                var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

                if (result.Succeeded)
                {
                    _response.Result = "Пароль успешно изменен";
                    return StatusCode(200, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Result = "Ошибка при изенении пароля";
                    return StatusCode(500, _response);
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = "Некорректный запрос";
                return StatusCode(400, _response);
            }
        }
    }
}
