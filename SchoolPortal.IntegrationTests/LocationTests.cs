using System.Net;
using System.Net.Http.Json;
using SchoolPortal.Api.Models;
using SchoolPortal.IntegrationTests.Fixtures;

namespace SchoolPortal.IntegrationTests;

[Collection("IntegrationTestsCollection")]
public class LocationTests : IAsyncLifetime, IClassFixture<SchoolPortalApiApplicationFactory<Program>>
{
    private readonly HttpClient httpClient;
    private readonly DatabaseFixture dbFixture;
    private readonly TestDataSeeder dataSeeder;

    public LocationTests(SchoolPortalApiApplicationFactory<Program> factory, DatabaseFixture dbFixture)
    {
        this.httpClient = factory.CreateClient();
        this.dbFixture = dbFixture;
        this.dataSeeder = new TestDataSeeder(dbFixture);
    }

    public async Task InitializeAsync() => await dbFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetNeighbourhoods_ReturnsNeighbourhoods_WhenNeighbourhoodsExist()
    {
        // Arrange
        var settlement = "София";
        var neighbourhoods = new string[] { "Лозенец", "Младост", "Дружба" };

        foreach (var neighbourhood in neighbourhoods)
        {
            await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        }       

        // Act
        var response = await httpClient.GetAsync($"api/v1/location/neighbourhoods/{settlement}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var neighbourhoodsResponse = await response.Content.ReadFromJsonAsync<GetNeighbourhoodsResponse>();

        Assert.NotNull(neighbourhoodsResponse);
        Assert.NotEmpty(neighbourhoodsResponse.Neighbourhoods);
        Assert.Equal(3, neighbourhoodsResponse.NeighbourhoodsCount);
        Assert.Contains(neighbourhoodsResponse.Neighbourhoods, n => n.Neighbourhood == "Лозенец");
        Assert.Contains(neighbourhoodsResponse.Neighbourhoods, n => n.Neighbourhood == "Младост");
        Assert.Contains(neighbourhoodsResponse.Neighbourhoods, n => n.Neighbourhood == "Дружба");
    }

    [Fact]
    public async Task GetNeighbourhoods_ReturnsEmptyModel_WhenNoNeighbourhoodsExist()
    {
        // Arrange
        var settlement = "София";

        // Act
        var response = await httpClient.GetAsync($"api/v1/location/neighbourhoods/{settlement}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var neighbourhoodsResponse = await response.Content.ReadFromJsonAsync<GetNeighbourhoodsResponse>();

        Assert.NotNull(neighbourhoodsResponse);
        Assert.Empty(neighbourhoodsResponse.Neighbourhoods);
        Assert.Equal(0, neighbourhoodsResponse.NeighbourhoodsCount);
    }
}

