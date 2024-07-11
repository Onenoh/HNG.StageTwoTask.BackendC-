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
using System.ComponentModel.DataAnnotations;

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
            var validationContext = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);

            // Step 2: Check if validation failed
            if (!isValid)
            {
                List<ErrorField> errors = validationResults
                    .Select(vr => new ErrorField { Field = vr.MemberNames.FirstOrDefault(), Message = vr.ErrorMessage })
                    .ToList();

                return new RegisterResponseModel
                {
                    Status = "error",
                    Message = "Validation error",
                    Data = new RegisterResponseData
                    {
                        Errors = errors
                    }
                };
            }

            // Step 2: Check if the email already exists in the database
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                // If the email already exists, return an error response
                return new RegisterResponseModel
                {
                    Status = "error",
                    Message = "Email address already exists",
                    Data = new RegisterResponseData(),
                };
            }

            // Step 3: Generate password salt and hash
            byte[] passwordSalt = GenerateSalt();
            byte[] passwordHash = HashPassword(request.Password, passwordSalt);

            // Step 4: Create a new user object
            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                // Do not store the plain Password field
            };

            // Step 5: Add the new user to the context and save changes
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Step 6: Create a default organization for the new user
            var organisationResponse = await _organisationService.CreateDefaultOrganisationAsync(newUser);

            // Step 7: Generate a JWT token for the new user
            var token = _tokenService.GenerateJwtToken(newUser);

            // Step 8: Return a success response with the generated token and user information
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

        private bool ValidateRequestModel(RegisterRequestModel request, out List<string> errors)
        {
            var context = new ValidationContext(request, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(request, context, validationResults, validateAllProperties: true);

            errors = validationResults.Select(r => r.ErrorMessage).ToList();
            return isValid;
        }
    }
}
