using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services;
using Lider_V_APIServices.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public AccountAPIController(UserManager<User> userManager, IAccountRepository accountRepository, ApplicationDbContext cotnext)
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
            this._response = new ResponseDto();
            _cotnext = cotnext;
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

                if (string.IsNullOrWhiteSpace(register.RegisterPassword) || register.RegisterPassword.Length < 4
                || !register.RegisterPassword.Any(char.IsUpper) || !register.RegisterPassword.Any(char.IsDigit))
                {
                    _response.IsSuccess = false;
                    _response.Result = "Пароль должен содержать как минимум 4 символа, хотя бы одну заглавную букву и одну цифру.";
                    return StatusCode(400, _response);
                }

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

                var userDto = new
                {
                    user.Id,
                    user.UserName,
                    user.UserFirstName,
                    user.UserLastName,
                    user.Email,
                    user.RegistrationDate,
                    user.LastLoginDate
                };

                _response.Result = userDto;
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

                var userDto = new
                {
                    user.Id,
                    user.UserName,
                    user.UserFirstName,
                    user.UserLastName,
                    user.Email,
                    user.RegistrationDate,
                    user.LastLoginDate
                };

                _response.Result = userDto;
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

                var userDto = new
                {
                    user.Id,
                    user.UserName,
                    user.UserFirstName,
                    user.UserLastName,
                    user.Email,
                    user.RegistrationDate,
                    user.LastLoginDate
                };

                _response.Result = userDto;
                return StatusCode(200, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
                return StatusCode(500, _response);
            }
        }
    }
}
