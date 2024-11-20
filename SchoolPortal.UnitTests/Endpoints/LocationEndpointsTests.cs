using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.UnitTests.Endpoints
{
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
            result
                .Should().BeOfType<Ok<GetNeighbourhoodsResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetNeighbourhoodsResponse>)?.Value;

            response.Should().NotBeNull();
            response!.NeighbourhoodsCount.Should().Be(neighbourhoods.Count);
            response.Neighbourhoods.Should().BeEquivalentTo(neighbourhoods);
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
            result
                .Should().BeOfType<Ok<GetNeighbourhoodsResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetNeighbourhoodsResponse>)?.Value;

            response.Should().NotBeNull();
            response!.NeighbourhoodsCount.Should().Be(0);
            response.Neighbourhoods.Should().BeEmpty();
        }
    }
}
