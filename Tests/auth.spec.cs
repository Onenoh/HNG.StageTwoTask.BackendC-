using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HNG.StageTwoTask.BackendC_.Models;
using HNG.StageTwoTask.BackendC_.Models.Access;
using Newtonsoft.Json;
using Xunit;

namespace HNG.StageTwoTask.BackendC_.Tests
{
    public class auth
    {
        private readonly HttpClient _client;

        public auth()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7111")
            };
        }


        [Fact]
        public async Task TestSuccessfulLogin()
        {
            //Arrange
            var loginRequest = new
            {
                email = "john.doe@example.com",
                password = "Password123"
            };

            var jsonRequest = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/auth/login", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Assert HTTP status code
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Unauthorized,
                $"Expected status code 200 or 401, but received {response.StatusCode}");

            var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // Assert response content properties for success
                Assert.Equal("success", (string)responseObject.status);
                Assert.Equal("Login successful", (string)responseObject.message);
                Assert.NotNull((string)responseObject.data.accessToken);
                Assert.Equal("John", (string)responseObject.data.user.firstName);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Assert response content properties for error
                Assert.Equal("error", (string)responseObject.status);
                Assert.Equal("Invalid credentials", (string)responseObject.message);
            }
        }

       
    }
}
