using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using YMNNetCoreFrameWork.Core.Authoratication;
using YMNNetCoreFrameWork.Host.Middles;

namespace YMNNetCoreFrameWork.Host.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<YMNUser> _userManager;

        private readonly SignInManager<YMNUser> _signInManager;

        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationController(UserManager<YMNUser>  userManager,SignInManager<YMNUser> signInManager,IConfiguration configuration,IHttpContextAccessor httpContextAccessor) {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public object Login() {

            return true;
        }

        [HttpGet("Register")]
        public async Task<object> Register(string name) {
     
            var user = new YMNUser() { UserName = name, Email = $"{name}@qq.com"};
            var result = await _userManager.CreateAsync(user,"Aa123456!");
            return result;
        }
        [HttpPost("Login")]
        public async Task<object> Login(string name, string password) {
            YMNSession.Configure(_httpContextAccessor);
            var user = await _userManager.FindByNameAsync(name);
            var result = await _signInManager.PasswordSignInAsync(user, password, false,false);
            //List<Claim> claims = new List<Claim>() {
            //     new Claim("userName",name)
            //};

            //这里可以随意加入自定义的参数，key可以自己随便起
            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.NameIdentifier, name)               
             };
            var token =  CreateAccessToken(claims);

            YMNSession.UserId = user.Id;
            YMNSession.UserName = user.UserName;
            YMNSession.TenantId = user.TenantId;
            return token;
        }

        [Authorize]
        [HttpGet("TestAuthentication")]       
        public async Task<object> TestAuthentication() {
            return "权限认证成功";
        }

        [HttpGet]
        [Route("Get2")]
        [Authorize("YMNPolicy")]
        public ActionResult<IEnumerable<string>> Get2()
        {
            //这是获取自定义参数的方法
         
            return new string[] { "只有授权的用户才能访问该接口", $"userName={YMNSession.UserName}" };
        }

        /*
         iss (issuer)：签发人

exp (expiration time)：过期时间

sub (subject)：主题

aud (audience)：受众

nbf (Not Before)：生效时间

iat (Issued At)：签发时间

jti (JWT ID)：编号
             */
        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;
            SymmetricSecurityKey symmetricSecurityKey =   new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("Authentication:JwtBearer")["SecurityKey"].ToString()));
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.GetSection("Authentication:JwtBearer")["Issuer"].ToString(),
                audience: _configuration.GetSection("Authentication:JwtBearer")["Audience"].ToString(),
                claims: claims,
                notBefore: now,
                expires:now.AddMinutes(30),
                // expires: now.Add(expiration ?? _configuration.Expiration),  SecurityKey
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }


        /// <summary>
        /// login
        /// </summary>
        /// <param name="userName">只能用user或者</param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/auth")]
        public IActionResult Get(string userName, string pwd)
        {
            if (CheckAccount(userName, pwd, out string role))
            {
                //每次登陆动态刷新
                Const.ValidAudience = userName + pwd + DateTime.Now.ToString();
                // push the user’s name into a claim, so we can identify the user later on.
                //这里可以随意加入自定义的参数，key可以自己随便起
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.NameIdentifier, userName),
                    new Claim("Role", role)
                };
                //sign the token using a secret key.This secret will be shared between your API and anything that needs to check that the token is legit.
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //.NET Core’s JwtSecurityToken class takes on the heavy lifting and actually creates the token.
                var token = new JwtSecurityToken(
                    //颁发者
                    issuer: Const.Domain,
                    //接收者
                    audience: Const.ValidAudience,
                    //过期时间
                    expires: DateTime.Now.AddMinutes(30),
                    //签名证书
                    signingCredentials: creds,
                    //自定义参数
                    claims: claims
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return BadRequest(new { message = "username or password is incorrect." });
            }
        }


        /// <summary>
        /// login
        /// </summary>
        /// <param name="userName">只能用user或者</param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/auth2")]
        public IActionResult Get2(string userName, string pwd)
        {
            if (CheckAccount(userName, pwd, out string role))
            {
                //每次登陆动态刷新
                Const.ValidAudience = userName + pwd + DateTime.Now.ToString();
                // push the user’s name into a claim, so we can identify the user later on.
                //这里可以随意加入自定义的参数，key可以自己随便起
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.NameIdentifier, userName),
                    new Claim("Role", role)
                };
                //sign the token using a secret key.This secret will be shared between your API and anything that needs to check that the token is legit.
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YMNNet_12345678912345678"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //.NET Core’s JwtSecurityToken class takes on the heavy lifting and actually creates the token.
                var token = new JwtSecurityToken(
                    //颁发者
                    issuer: "YMNNet",
                    //接收者
                    audience: "YMNNet",
                    //过期时间
                    expires: DateTime.Now.AddMinutes(30),
                    //签名证书
                    signingCredentials: creds,
                    //自定义参数
                    claims: claims
                    );

                return Ok(new
                {
                    token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return BadRequest(new { message = "username or password is incorrect." });
            }
        }


        /// <summary>
        /// 模拟登陆校验，因为是模拟，所以逻辑很‘模拟’
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool CheckAccount(string userName, string pwd, out string role)
        {
            role = "user";

            if (string.IsNullOrEmpty(userName))
                return false;

            if (userName.Equals("admin"))
                role = "admin";

            return true;
        }

    }
}