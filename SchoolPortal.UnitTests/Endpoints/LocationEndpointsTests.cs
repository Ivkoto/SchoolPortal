using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.UnitTests.Endpoints;

public class LocationEndpointsTests
{
    private readonly Mock<ILocationRepository> locationRepositoryMock;
    private readonly Location locationEndpoint;
    private readonly DefaultHttpContext httpContext;

    public LocationEndpointsTests()
    {
        locationRepositoryMock = new();
        locationEndpoint = new();
        httpContext = new();
    }

    [Fact]
    public async Task GetNeighbourhoods_ReturnsOk_WhenNeighbourhoodsExist()
    {
        // Arrange
        var settlement = "София";
        var neighbourhoods = new List<NeighbourhoodModel>
        {
            It.IsAny<NeighbourhoodModel>(),
            It.IsAny<NeighbourhoodModel>()
        };

        locationRepositoryMock
            .Setup(repo => repo.GetNeighbourhoodsBySettlement(settlement))
            .ReturnsAsync(neighbourhoods);

        // Act
        var result = await locationEndpoint.GetNeighbourhoods(
            settlement,
            httpContext,
            locationRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetNeighbourhoodsResponse>>(result);
        var okResult = result as Ok<GetNeighbourhoodsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(neighbourhoods.Count, response.NeighbourhoodsCount);
        Assert.Equal(neighbourhoods, response.Neighbourhoods);
    }

    [Fact]
    public async Task GetNeighbourhoods_ReturnsEmptyList_WhenNoNeighbourhoodsExist()
    {
        // Arrange
        var settlement = "София";
        var neighbourhoods = new List<NeighbourhoodModel>();

        locationRepositoryMock
            .Setup(repo => repo.GetNeighbourhoodsBySettlement(settlement))
            .ReturnsAsync(neighbourhoods);

        // Act
        var result = await locationEndpoint.GetNeighbourhoods(
            settlement,
            httpContext,
            locationRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetNeighbourhoodsResponse>>(result);
        var okResult = result as Ok<GetNeighbourhoodsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response.NeighbourhoodsCount);
        Assert.Empty(response.Neighbourhoods);
    }
}
