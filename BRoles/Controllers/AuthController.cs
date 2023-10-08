using BRoles.Data;
using BRoles.Helpers;
using BRoles.Models;
using BRoles.tools;
using BRoles.UtilityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace BRoles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {     //dependency injections
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> logger;
        public readonly FullStackDBContext _cntx;
        public readonly Icheck _checkuser;

        public AuthController(FullStackDBContext fullstackdbcontext, IConfiguration configuration,
            IEmailService emailService, ILogger<AuthController> logger, Icheck checkuser
            )
        {
            _checkuser= checkuser;
            this.logger = logger;
            _cntx = fullstackdbcontext;
            _configuration = configuration;
            _emailService = emailService;
        }





        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationUser request)
        {


            Log.WriteLine("inside the register ");
            int apiNum = 2;

            checkUser checkrequest = new checkUser()
            {
                Username = request.Username,
                Password = request.Password,
                Email = request.Email,
                Salary = request.Salary,
                Token = "",
                Role = request.Role,
                Desactivated = false,
            };





            try
            {

                #region CHECK DATA
                String Format = "ok";//_checkuser.checkData(checkrequest, apiNum);
                #endregion


                if (Format == "ok")
                {
                    Log.WriteLine("inside the ok");

                    
                    UserDto userexist= _cntx.users.FirstOrDefault(x=>x.Username == request.Username);
                 //   UserDto user = _cntx.users.FirstOrDefault(x => (x.Username == request.Username))
                       if (userexist != null)
                       {

                           _checkuser.saveAttempt(request.Username, "register", "username already exists");

                           return Ok(new
                           {
                               statuscode = 400,
                               Message = "username already exists"

                           }); 


                       }
                       


                    var user = new UserDto();

                    user.Id = Guid.NewGuid();
                    user.Username = request.Username;
                    user.Email = request.Email;
                    user.Salary = request.Salary;
                    user.Role = request.Role;



                    createPasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    user.Username = request.Username;

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

                    _checkuser.saveAttempt(request.Username, "register", "");

                    await _cntx.AddAsync(user);
                    await _cntx.SaveChangesAsync();
                    return Ok(user);

                }


                else
                {
                    return BadRequest(new
                    {
                        statusCode = 400,
                        Message = Format

                    }); 



                }



            }

            catch (Exception ex)
            {
                logger.LogError("GENERAL EXCEPTION \n" + ex.Message);
                return Ok(new
                {
                    StatusCode = 500,
                    Message = "execption raised ! " + ex.Message
                }) ;
            }
        }


     




        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAuth request)
        {
            Log.WriteLine("inside the logine ");
            int apiNum = 1;

            checkUser checkrequest = new checkUser()
            {
                Username = request.Username,
                Password = request.Password,
                Email = "",
                Salary = 0,
                Token = "",
                Role = "",
                Desactivated = false,


            };





         
            try
            {
                #region CHECK DATA
                String Format = _checkuser.checkData(checkrequest, apiNum);
                #endregion

                if (Format=="ok")
                {
                    
                    string logMsg = "";
                    logMsg += "inside the login method";
                    UserDto user = await _cntx.users.FirstOrDefaultAsync(x => (x.Username == request.Username)||( x.Email == request.Username));
               
                    if (user == null)
                    {
                        _checkuser.saveAttempt(request.Username, "login", "username or email not found");

                        return Ok(new //i used ok so that i can read the status code inside the next not the error
                        {
                        StatusCode = 404,
                            Message = "user not found"

                        });

                    }



                    if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        _checkuser.saveAttempt(user.Username, "login", "wrong password");



                        return Ok(new
                        {
                            StatusCode = 400,
                            Message = "wrong password"

                        });
                    }


                    user.Token = createToken(user);
                    _checkuser.saveAttempt(user.Username, "login", "");

                    return Ok(new
                    {
                        StatusCode = 200,
                        Token = user.Token,
                        Message = "login success !",
                        Desactivated = user.Desactivated,
                        Username=user.Username,
                        Email=user.Email,
                        Role=user.Role,

                    }

                        ); 




                }
                else
                {
                    return BadRequest(new
                    {
                        message=Format
                    });
                
                
                }

                }
            catch (Exception ex)
            {
                Log.WriteLine("GENERAL EXCEPTION \n" + ex.Message);

                return BadRequest(new
                {

                    Message = "execption raised! "+ ex.Message,

                });
              
            }



      





        }

        private UserDto GetUserLogin(UserAuth request, ref string logMsg)
        {
           
                UserDto user = _cntx.users.FirstOrDefault(x => (x.Username == request.Username));//check if the user is logging with it's username
                user ??= _cntx.users.FirstOrDefault(x => x.Email == request.Username);//check if the user is logging with it's email
                logMsg += user != null ? "| User Found" : "| User Not found";
                return user;
            

        }



        private void createPasswordHash(string password, out byte[] PasswordHash, out byte[] passwordSalt)
        {
            //the param is used for output purposes

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }



        private bool VerifyPasswordHash(string password, byte[] passwordHashRequest, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHashRequest);
            }
        }





        private string createToken(UserDto user)
        {
            try
            {
                List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username),
                 new Claim(ClaimTypes.Role,user.Role),
                  new Claim(ClaimTypes.Email,user.Email)
            };
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(5),
                    signingCredentials: creds
                    );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return (string)jwt;
            }
            catch (Exception ex)
            {
                logger.LogError("GENERAL EXCEPTION \n" + ex.Message);
                return ("");


            }

        }






        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllusers()
        {
            try
            {
                var users = await _cntx.users.ToListAsync();
                //ToListAsync() is used to retrieve data from a database and converts it to a list of objects 
                return Ok(users);
            }
            catch (Exception ex)
            {
                logger.LogError("GENERAL EXCEPTION \n" + ex.Message);
                return BadRequest(ex.Message);
            }


        }





     


        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel2 emailmodel)
        {
            try
            {

                var email = emailmodel.email;
                var user = await _cntx.users.FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "email not found"
                    });
                }


                var tokenBytes = RandomNumberGenerator.GetBytes(64);
                var emailToken = Convert.ToBase64String(tokenBytes);


                user.ResetPasswordToken = emailToken;
                user.ResetPasswordExpiry = DateTime.Now.AddDays(15);


                string from = _configuration["EmailSettings:From"];

                char characterToAdd = '"';
                email = characterToAdd + email + characterToAdd;


                var emailModel = new EmailModel(email, "reset password",
                 EmailBody.EmailStringBody(email, emailToken));


                _emailService.SendEmail(emailModel);

                //sus
            //    _cntx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
               await _cntx.SaveChangesAsync();
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Email sent!",
                    Email = email,
                    EmailToken = emailToken,
                });


            }
            catch (Exception ex)
            {
                logger.LogError("GENERAL EXCEPTION \n" + ex.Message);
                return BadRequest(ex.Message);
            }


        }








        [HttpPost("send-password")]
        public async Task<IActionResult> SendPasswordEmail( [FromBody] RegistrationUser user)
        {
            try
            {

                var email = user.Email;
                var username = user.Username;

                var password = user.Password;


                string from = _configuration["EmailSettings:From"];

                char characterToAdd = '"';
                email = characterToAdd + email + characterToAdd;



                var emailModel = new EmailModel(email, "new account ",
                $@"<html>

                </html> "

                 );


                _emailService.SendEmailPassword(emailModel, password, username);


                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Email sent!",
                    Email = email,
                    Password = password,
                    Login = username,

                });

            }
            catch (Exception ex)
            {
                logger.LogError("GENERAL EXCEPTION \n" + ex.Message);
                return BadRequest(ex.Message);
               
            }


        }









        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {

            try
            {
                var newToken = resetPasswordDto.EmailToken.Replace(" ", "+");

                var user = await _cntx.users.FirstOrDefaultAsync(a => a.Email == resetPasswordDto.Email);
                if (user == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "user not found!"
                    });

                }



                var tokenCode = user.ResetPasswordToken;
                DateTime emailTokenExpiry = user.ResetPasswordExpiry;
                if (tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "(expired token) orrrr wrong token "
                    });
                }

                createPasswordHash(resetPasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);


                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                //sus
               // _cntx.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _cntx.SaveChangesAsync();
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Password changed !",



                });
            }
            catch (Exception ex)
            {
                logger.LogError("GENERAL EXCEPTION \n" + ex.Message);
                return BadRequest(ex.Message);
            }


        }










        [HttpPost("activateddesactivated")]
        public async Task<IActionResult> desactivateActivate(EmailModel2 emailmodel)
        {
            try
            {
                string email = emailmodel.email;
                var user = await _cntx.users.FirstOrDefaultAsync(x => x.Email == email);


                if (User == null)
                {
                    return NotFound();
                }

                if (user.Desactivated == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        message = "user.desactivated is null!"
                    });

                }

                if (user.Desactivated == null || user.Desactivated == false)
                {
                    user.Desactivated = true;
                }

                else
                {
                    user.Desactivated = false;
                }

                await _cntx.SaveChangesAsync();

                return Ok(new
                {
                    StatusCode = 200,
                    message = "success",
                    desactivated = user.Desactivated
                });

            }

            catch (Exception ex)
            {
                logger.LogError("GENERAL EXCEPTION \n" + ex.Message);
                return BadRequest(ex.Message);
            }


        }










    }



}

