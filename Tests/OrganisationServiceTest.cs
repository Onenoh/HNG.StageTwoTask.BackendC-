using System.Linq;
using System.Text;
using HNG.StageTwoTask.BackendC_.Data;
using HNG.StageTwoTask.BackendC_.Implementation;
using HNG.StageTwoTask.BackendC_.Implementation.Interfaces;
using HNG.StageTwoTask.BackendC_.Models.Access;
using HNG.StageTwoTask.BackendC_.Models.Organisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace HNG.StageTwoTask.BackendC_.Tests
{
    public class OrganisationServiceTests
    {
        private readonly OrganisationService _organisationService;
        private readonly AppDbContext _context;
        private readonly Mock<IAuthService> _mockAuthService = new Mock<IAuthService>();
        private readonly Mock<ITokenService> _mockTokenService = new Mock<ITokenService>();
        private readonly Mock<IConfiguration> _mockConfiguration = new Mock<IConfiguration>();

        public OrganisationServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDatabase")
                          .Options;
            _context = new AppDbContext(options);
            _organisationService = new OrganisationService(_context, _mockTokenService.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetOrganisationsAsync_Should_Return_Organisations_For_Valid_UserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password123",
                PasswordHash = Encoding.UTF8.GetBytes("hashedpassword"),
                Phone = "1234567890",
                PasswordSalt = Encoding.UTF8.GetBytes("salted")
            };
            var organisations = new List<Organisations>
            {
                new Organisations { OrgId = Guid.NewGuid().ToString(), Name = "Org1", Description = "Description 1" },
                new Organisations { OrgId = Guid.NewGuid().ToString(), Name = "Org2", Description = "Description 2" }
            };

            _context.Users.Add(user);
            _context.Organisations.AddRange(organisations);
            await _context.SaveChangesAsync();

            foreach (var org in organisations)
            {
                _context.OrganisationUsers.Add(new OrganisationUser { OrgId = org.OrgId, UserId = userId });
            }
            await _context.SaveChangesAsync();

            // Act
            var result = await _organisationService.GetOrganisationsAsync(userId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("success", result.Status);
            Assert.Equal("Organisations retrieved successfully", result.Message);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Organisations);
            Assert.Equal(2, result.Data.Organisations.Count);
            Assert.Equal("Org1", result.Data.Organisations[0].Name);
            Assert.Equal("Org2", result.Data.Organisations[1].Name);
        }


        [Fact]
        public async Task GetOrganisationByIdAsync_Should_Return_Organisation_Details_For_Valid_UserId_And_OrgId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password123",
                PasswordHash = Encoding.UTF8.GetBytes("hashedpassword"),
                Phone = "1234567890",
                PasswordSalt = Encoding.UTF8.GetBytes("salted")
            };
            var organisation = new Organisations { OrgId = orgId.ToString(), Name = "Org1", Description = "Description 1" };

            _context.Users.Add(user);
            _context.Organisations.Add(organisation);
            await _context.SaveChangesAsync();
            _context.OrganisationUsers.Add(new OrganisationUser { OrgId = orgId.ToString(), UserId = userId });
            await _context.SaveChangesAsync();

            // Act
            var result = await _organisationService.GetOrganisationByIdAsync(userId.ToString(), orgId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("success", result.Status);
            Assert.Equal("Organisation retrieved successfully", result.Message);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Organisations);
        }


        [Fact]
        public async Task CreateOrganisationAsync_Should_Create_New_Organisation_For_Valid_UserId_And_Request()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new OrganisationRequestModel
            {
                Name = "New Org",
                Description = "New Description"
            };
            var user = new User
            {
                UserId = userId,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password123",
                PasswordHash = Encoding.UTF8.GetBytes("hashedpassword"),
                Phone = "1234567890",
                PasswordSalt = Encoding.UTF8.GetBytes("salted")
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _organisationService.CreateOrganisationAsync(userId.ToString(), request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("success", result.Status);
            Assert.Equal("Organisation created successfully", result.Message);
            Assert.Equal(201, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Organisations);
            Assert.NotNull(_context.Organisations.SingleOrDefault(o => o.Name == "New Org"));
        }


        [Fact]
        public async Task AddUserToOrganisationAsync_Should_Add_User_To_Organisation_For_Valid_OrgId_And_UserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orgId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId, 
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password123",
                PasswordHash = Encoding.UTF8.GetBytes("hashedpassword"),
                Phone = "1234567890",
                PasswordSalt = Encoding.UTF8.GetBytes("salted")
            };
            var organisation = new Organisations { OrgId = orgId.ToString(), Name = "Org1", Description = "Description 1" };

            _context.Users.Add(user);
            _context.Organisations.Add(organisation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _organisationService.AddUserToOrganisationAsync(orgId.ToString(), userId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal("success", result.Status);
            Assert.Equal("User added to organisation successfully", result.Message);
            Assert.Equal(200, result.StatusCode);

            // Check if the user was added to the organisation
            Assert.NotNull(_context.OrganisationUsers.SingleOrDefault(ou => ou.UserId == userId && ou.OrgId == orgId.ToString()));
        }



        [Fact]
        public async Task CreateDefaultOrganisationAsync_Should_Create_Organisation_For_User()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                PasswordHash = new byte[] { 1, 2, 3 }, 
                Phone = "1234567890"
            };

            // Act
            var organisation = await _organisationService.CreateDefaultOrganisationAsync(user);

            // Assert
            Assert.NotNull(organisation);
            Assert.NotEmpty(organisation.OrgId);
            Assert.Equal("John's Organization", organisation.Name);
            Assert.Equal("Organization created by John", organisation.Description);

            var addedOrganisation = await _context.Organisations.FirstOrDefaultAsync(o => o.OrgId == organisation.OrgId);
            Assert.NotNull(addedOrganisation);
            Assert.Equal("John's Organization", addedOrganisation.Name);
        }
    }
}
