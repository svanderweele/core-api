using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Core.Gaming.API.Contracts.Requests;
using Core.Gaming.Tests.Contracts.Responses;

namespace Core.Gaming.Tests;

public class GameLiveTest :  IAsyncLifetime
{
    private readonly HttpClient _httpClient = new(
        new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }) { BaseAddress = new Uri("https://localhost:5001"), };

    private readonly HttpClient _authHttpClient = new()
        { BaseAddress = new Uri("https://z7qv6ih936.execute-api.eu-west-1.amazonaws.com"), };


    private async Task LoginUser()
    {
        var request = new
        {
            Email = "vanderweelesimon@gmail.com",
            Password = "Test123"
        };

        var response = await _authHttpClient.PostAsync("/prod/api/authentication/login", JsonContent.Create(request));

        if (response.IsSuccessStatusCode == false)
        {
            return;
        }

        var responseData = await response.Content.ReadFromJsonAsync<LoginResponse>();

        if (responseData == null)
        {
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", responseData.Token);
    }

    [Fact]
    public async Task GivenARequest_WhenCallingCreateGame_ThenTheAPIReturnsUnauthorized()
    {
        // Arrange.
        var expectedStatusCode = System.Net.HttpStatusCode.Unauthorized;
        var stopwatch = Stopwatch.StartNew();
        var requestBody = new CreateGameRequest()
        {
            Name = "Pug Life"
        };

        // Act.
        var response = await _httpClient.PostAsJsonAsync("/api/games", requestBody);

        // Assert.
        TestHelpers.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }

    [Fact]
    public async Task GivenARequest_WhenCallingCreateGame_ThenTheAPIReturnsBadRequest()
    {
        // Arrange.
        var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
        var stopwatch = Stopwatch.StartNew();
        var request = new CreateGameRequest()
        {
            Name = "Pug Life"
        };


        // Act.
        await LoginUser();
        var response = await _httpClient.PostAsJsonAsync("/api/games", request);

        // Assert.
        TestHelpers.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }

    private Guid _createdGameId = Guid.Empty;

    [Fact]
    public async Task GivenARequest_WhenCallingCreateGame_ThenTheAPIReturnsExpectedResponse()
    {
        // Arrange.
        var expectedStatusCode = System.Net.HttpStatusCode.Created;
        var stopwatch = Stopwatch.StartNew();
        var request = new CreateGameRequest()
        {
            Name = "Pug Life",
            DisplayIndex = 23,
            ReleaseDate = new DateTime(2000, 11, 08),
            // GameCategory = Guid.Parse("df061beb-dab1-443c-95ff-a38eae2bc3d1"),
            GameCategory = Guid.Parse("df061beb-dab1-443c-95ff-a38eae2b1108"),
            Thumbnail =
                "https://cdnroute.bpsgameserver.com/v3/bgr/Common/Common/neutral/image/2022/11/game.relaxPugLife.thumbnail.196x196.jpg",
            Devices = new[] { "mobile" },
            Collections = new[] { Guid.Parse("9e8babfd-93b7-4d3e-8156-9595c990753d") },
        };

        // Act.
        await LoginUser();

        var response = await _httpClient.PostAsync("/api/games", JsonContent.Create(request));
        var data = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
        if (data != null) _createdGameId = data.Id;

        // Assert.
        TestHelpers.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }

    [Fact]
    public async Task GivenARequest_WhenCallingGetGames_ThenTheAPIReturnsExpectedResponse()
    {
        // Arrange.
        var expectedStatusCode = System.Net.HttpStatusCode.OK;
        var stopwatch = Stopwatch.StartNew();

        // Act.
        var response = await _httpClient.GetAsync("/api/games");


        // Assert.
        TestHelpers.AssertCommonResponseParts(stopwatch, response, expectedStatusCode);
    }


    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_createdGameId != Guid.Empty)
        {
            await _httpClient.DeleteAsync($"/api/games/{_createdGameId}");
        }
    }
}