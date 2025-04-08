using LoginAuthApi.DAL;
using LoginAuthApi.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EccomerceDbContext _userDbContext;
        public UserController(EccomerceDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromBody] User user)
        {
            try
            {
                if (user == null)
                { return BadRequest(); }

                User user1 = await _userDbContext
                    .Users
                    .FirstOrDefaultAsync(x => x.Username == user.Username && x.Password == user.Password);

                if (user1 == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                else
                {

                    Response.Cookies.Append("authToken", CreateJwtToken(user1), new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,            // Use HTTPS in production
                        SameSite = SameSiteMode.Strict,
                    });
                    Random random = new Random();
                    long otp = random.Next(0000, 1000);
                    SendEmailAsync(user1.Email, otp);
                    return Ok(new
                    {
                        Message = "Login success !! "
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> UserRegister([FromBody] User user)
        {
            try
            {
                if (user == null)
                { return BadRequest(); }

                await _userDbContext.Users.AddAsync(user);
                await _userDbContext.SaveChangesAsync();

                return Ok(new { Message = "Register successfully. " });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                List<User> usersList = new List<User>();
                usersList = await _userDbContext.Users.ToListAsync();

                if (usersList == null)
                    return NotFound("User not found.Something went wrong");
                else
                    return Ok(usersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        private string CreateJwtToken(User user)
        {
            try
            {
                JwtSecurityTokenHandler jwtTokenhandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.ASCII.GetBytes("Work Hard for tommorow and make it happen ....");
                ClaimsIdentity identity = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Role ,_userDbContext.Roles.Find(user.RoleId).RoleName),
                new Claim (ClaimTypes.Name,$"{user.FirstName} {user.LastName}")
                });

                SigningCredentials signingCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
                SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = identity,
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = signingCredentials
                };
                SecurityToken jwtToken = jwtTokenhandler.CreateToken(securityTokenDescriptor);
                return jwtTokenhandler.WriteToken(jwtToken);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private void SendEmailAsync(string toEmail, long otp)
        {
            try
            {

                var body = $"<b>Your otp is {otp}</b>";
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("abhimanyu.vir7499@gmail.com"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = "Auto generated otp for login.Please dont share.";
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("abhimanyu.vir7499@gmail.com", "okcn xmwv ezwb iegy");
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex) { }
        }
    }
}
