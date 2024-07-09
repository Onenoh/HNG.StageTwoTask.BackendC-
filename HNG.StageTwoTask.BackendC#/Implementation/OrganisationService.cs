using HNG.StageTwoTask.BackendC_.Data;
using HNG.StageTwoTask.BackendC_.Implementation.Interfaces;
using HNG.StageTwoTask.BackendC_.Models.Access;
using HNG.StageTwoTask.BackendC_.Models.Organisation;
using Microsoft.EntityFrameworkCore;

namespace HNG.StageTwoTask.BackendC_.Implementation
{
    public class OrganisationService : IOrganisationService
    {
        private readonly AppDbContext Context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public OrganisationService(AppDbContext context, ITokenService tokenService, IConfiguration configuration)
        {
            Context = context;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<Organisations> CreateDefaultOrganisationAsync(User user)
        {
            var organisation = new Organisations
            {
                OrgId = Guid.NewGuid().ToString(),
                Name = $"{user.FirstName}'s Organization",
                Description = $"Organization created by {user.FirstName}"
            };

            Context.Organisations.Add(organisation);
            await Context.SaveChangesAsync();

            var userOrganization = new OrganisationUser
            {
                UserId = user.UserId,
                OrgId = organisation.OrgId
            };

            Context.OrganisationUsers.Add(userOrganization);
            await Context.SaveChangesAsync();

            return organisation;
        }

        public async Task<OrganisationResponseModel> GetUserDetailsAsync(string userId, string requestingUserId)
        {
            if (requestingUserId != userId)
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "Unauthorized",
                    StatusCode = 401
                };
            }

            // Convert string userId to Guid
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "Invalid UserId format",
                    StatusCode = 400
                };
            }

            var user = await Context.Users
                .Where(u => u.UserId == userGuid) 
                .Select(u => new
                {
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Phone,
                    u.OrganisationUser
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "User not found",
                    StatusCode = 404
                };
            }

            return new OrganisationResponseModel
            {
                Status = "success",
                Message = "User details retrieved successfully",
                StatusCode = 200,
                Data = new ResponseModelData
                {
                    User = new User
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.Phone
                    }
                }
            };
        }

        public async Task<OrganisationResponseModel> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await Context.Users.FindAsync(userId);

                if (user == null)
                {
                    return new OrganisationResponseModel
                    {
                        Status = "error",
                        Message = "User not found",
                        StatusCode = 404
                    };
                }

                return new OrganisationResponseModel
                {
                    Status = "success",
                    Message = "User record retrieved successfully",
                    StatusCode = 200,
                    Data = new ResponseModelData
                    {
                        User = new User
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            Phone = user.Phone
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "Failed to retrieve user record",
                    StatusCode = 500
                };
            }
        }

        public async Task<OrganisationResponseModel> GetOrganisationsAsync(string userId)
        {
            var organisations = await Context.Organisations
                .Where(o => o.OrganisationUser.Any(ou => ou.UserId.ToString() == userId))
                .Select(o => new Organisations 
                {
                    OrgId = o.OrgId,
                    Name = o.Name,
                    Description = o.Description
                })
                .ToListAsync();

            if (organisations == null || organisations.Count == 0)
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "No organisations found",
                    StatusCode = 404 
                };
            }

            var responseData = new ResponseModelData
            {
                Organisations = organisations
            };

            return new OrganisationResponseModel
            {
                Status = "success",
                Message = "Organisations retrieved successfully",
                StatusCode = 200,
                Data = responseData
            };
        }

        public async Task<OrganisationResponseModel> GetOrganisationByIdAsync(string userId, string orgId)
        {
            var organisation = await Context.Organisations
                .FirstOrDefaultAsync(o => o.OrgId == orgId &&
                                          o.OrganisationUser.Any(ou => ou.UserId.ToString() == userId));

            if (organisation == null)
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "Organisation not found or you do not have access",
                    StatusCode = 404 
                };
            }

            var responseData = new ResponseModelData
            {
                Organisations = new List<Organisations>
                {
                    new Organisations
                    {
                        OrgId = organisation.OrgId,
                        Name = organisation.Name,
                        Description = organisation.Description
                    }
                }
            };

            return new OrganisationResponseModel
            {
                Status = "success",
                Message = "Organisation retrieved successfully",
                StatusCode = 200,
                Data = responseData
            };
        }

        public async Task<OrganisationResponseModel> CreateOrganisationAsync(string userId, OrganisationRequestModel request)
        {
            var user = await Context.Users.FindAsync(Guid.Parse(userId));
            if (user == null)
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "User not found",
                    StatusCode = 404,
                    Data = null
                };
            }

            var organisation = new Organisations
            {
                OrgId = Guid.NewGuid().ToString(), 
                Name = request.Name,
                Description = request.Description
            };

            Context.Organisations.Add(organisation);
            await Context.SaveChangesAsync();

            Context.OrganisationUsers.Add(new OrganisationUser
            {
                OrgId = organisation.OrgId, 
                UserId = Guid.Parse(userId) 
            });
            await Context.SaveChangesAsync();

            return new OrganisationResponseModel
            {
                Status = "success",
                Message = "Organisation created successfully",
                StatusCode = 201,
                Data = new ResponseModelData
                {
                    Organisations = new List<Organisations>
                    {
                        new Organisations
                        {
                            OrgId = organisation.OrgId.ToString(), // Convert Guid to string for response if needed
                            Name = organisation.Name,
                            Description = organisation.Description
                        }
                    }
                }
            };
        }

        public async Task<OrganisationResponseModel> AddUserToOrganisationAsync(string orgId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = "User ID is required",
                    StatusCode = 400
                };
            }

            var organisation = await Context.Organisations.FindAsync(orgId);
            if (organisation == null)
            {
                return new OrganisationResponseModel
                {
                    Status = "error",
                    Message = $"Organisation with ID {orgId} not found",
                    StatusCode = 404
                };
            }

            var organisationUser = new OrganisationUser
            {
                UserId = Guid.Parse(userId),
                OrgId = orgId
            };

            Context.OrganisationUsers.Add(organisationUser);
            await Context.SaveChangesAsync();

            return new OrganisationResponseModel
            {
                Status = "success",
                Message = "User added to organisation successfully",
                StatusCode = 200
            };
        }
    }
}
