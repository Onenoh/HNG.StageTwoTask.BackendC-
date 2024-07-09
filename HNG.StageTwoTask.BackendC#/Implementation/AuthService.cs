using HNG.StageTwoTask.BackendC_.Data;
using HNG.StageTwoTask.BackendC_.Implementation.Interfaces;
using HNG.StageTwoTask.BackendC_.Models;
using HNG.StageTwoTask.BackendC_.Models.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HNG.StageTwoTask.BackendC_.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IOrganisationService _organisationService;

        public AuthService(AppDbContext context, ITokenService tokenService, IConfiguration configuration, IOrganisationService organisationService)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
            _organisationService = organisationService;
        }


        public async Task<RegisterResponseModel> RegisterAsync(RegisterRequestModel request)
        {
            var validationErrors = ValidateUser(request);
            if (validationErrors.Count > 0)
            {
                return new RegisterResponseModel
                {
                    Status = "error",
                    Message = "Validation error",
                    Data = new RegisterResponseData()
                };
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return new RegisterResponseModel
                {
                    Status = "error",
                    Message = "Email address already exists",
                    Data = new RegisterResponseData(),
                };
            }

            byte[] passwordSalt = GenerateSalt();
            byte[] passwordHash = HashPassword(request.Password, passwordSalt);

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Password = request.Password
            };

            
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var organisationResponse = await _organisationService.CreateDefaultOrganisationAsync(newUser);

            var token = _tokenService.GenerateJwtToken(newUser);

            return new RegisterResponseModel
            {
                Status = "success",
                Message = "User registered successfully",
                Data = new RegisterResponseData
                {
                    AccessToken = token,
                    User = new RegisterData
                    {
                        UserId = newUser.UserId.ToString(),
                        Email = newUser.Email,
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                    }
                }
            };
        }

        public async Task<LoginResponseModel> LoginAsync(LoginRequestModel request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return new LoginResponseModel
                {
                    Status = "error",
                    Message = "Invalid email or password",
                    Data = new LoginDataResponse()
                };
            }

            var token = _tokenService.GenerateJwtToken(user);

            return new LoginResponseModel
            {
                Status = "success",
                Message = "Login successful",
                Data = new LoginDataResponse
                {
                    AccessToken = token,
                    User = new RegisterData
                    {
                        UserId = user.UserId.ToString(),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.Phone
                    }
                }
            };
        }



        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return storedHash.SequenceEqual(computedHash);
            }
        }


        private static void CreateHash(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16]; 
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }


        private List<ErrorResponse> ValidateUser(RegisterRequestModel user)
        {
            var errors = new List<ErrorResponse>();

            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                errors.Add(new ErrorResponse { Field = "FirstName", Message = "First name is required" });
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                errors.Add(new ErrorResponse { Field = "LastName", Message = "Last name is required" });
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add(new ErrorResponse { Field = "Email", Message = "Email is required" });
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                errors.Add(new ErrorResponse { Field = "Password", Message = "Password is required" });
            }

            return errors;
        }
    }
}
