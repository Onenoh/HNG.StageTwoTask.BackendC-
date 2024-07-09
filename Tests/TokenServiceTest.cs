using HNG.StageTwoTask.BackendC_.Models.Access;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;


namespace HNG.StageTwoTask.BackendC_.Tests
{
    public class TokenServiceTests
    {
        [Fact]
        public void GenerateJwtToken_ShouldContainCorrectUserDetailsAndExpirationTime()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Email = "test@example.com"
            };

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["Jwt:Key"]).Returns("95024DF5CC2609C0C58A2623DA60D1FE931CDE10B2A0BA50BBEF1C0DFB88DE16");
            configuration.Setup(c => c["Jwt:Issuer"]).Returns("Organisation");
            configuration.Setup(c => c["Jwt:Audience"]).Returns("Organisation");

            var tokenService = new TokenService(configuration.Object);

            // Act
            var token = tokenService.GenerateJwtToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Assert
            var subClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            var expiration = jwtToken.ValidTo;

            Assert.Equal(user.UserId.ToString(), subClaim);
            Assert.Equal(user.Email, emailClaim);
            Assert.True(expiration <= DateTime.UtcNow.AddMinutes(30));
        }


    }
}
