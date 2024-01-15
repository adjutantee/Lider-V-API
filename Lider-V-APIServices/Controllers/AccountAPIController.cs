using Lider_V_APIServices.DbContexts;
using Lider_V_APIServices.Models;
using Lider_V_APIServices.Models.Dto;
using Lider_V_APIServices.Services;
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
                    var token = _accountRepository.GenerateJwtTokenByUser(user);
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

                var user = new User
                {
                    Email = register.RegisterEmail,
                    UserName = register.RegisterLogin,
                    UserFirstName = register.RegisterFirstName,
                    UserLastName = register.RegisterLastName,
                };

                var result = await _userManager.CreateAsync(user, register.RegisterPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Constants.UserRoleName);
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
    }
}
