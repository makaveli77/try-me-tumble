using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Xunit;
using TryMeTumble.Application.Services;
using TryMeTumble.Domain.Interfaces;
using StackExchange.Redis;
using TryMeTumble.Domain.Entities;
using TryMeTumble.Application.Interfaces;
using TryMeTumble.Application.DTOs;

namespace TryMeTumble.UnitTests
{
    public class WebsiteServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IWebsiteMetadataClient> _metadataClientMock;
        private readonly Mock<IConnectionMultiplexer> _redisMock;
        private readonly Mock<IDatabase> _redisDbMock;
        private readonly WebsiteService _websiteService;

        public WebsiteServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _metadataClientMock = new Mock<IWebsiteMetadataClient>();
            _redisMock = new Mock<IConnectionMultiplexer>();
            _redisDbMock = new Mock<IDatabase>();

            _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_redisDbMock.Object);

            _websiteService = new WebsiteService(
                _unitOfWorkMock.Object,
                _metadataClientMock.Object,
                _redisMock.Object);
        }

        [Fact]
        public async Task GetWebsiteByIdAsync_WhenWebsiteExists_ShouldReturnDto()
        {
            // Arrange
            var websiteId = Guid.NewGuid();
            var website = new Website { Id = websiteId, Url = "https://example.com", Title = "Example" };
            
            _unitOfWorkMock.Setup(u => u.Websites.GetByIdAsync(websiteId))
                .ReturnsAsync(website);

            // Act
            var result = await _websiteService.GetWebsiteByIdAsync(websiteId);

            // Assert
            result.Should().NotBeNull();
            result.Url.Should().Be("https://example.com");
            result.Title.Should().Be("Example");
        }

        [Fact]
        public async Task GetWebsiteByIdAsync_WhenWebsiteDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var websiteId = Guid.NewGuid();
            
            _unitOfWorkMock.Setup(u => u.Websites.GetByIdAsync(websiteId))
                .ReturnsAsync((Website)null);

            // Act
            var result = await _websiteService.GetWebsiteByIdAsync(websiteId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SubmitWebsiteAsync_WhenUrlAlreadyExists_ShouldReturnExistingDto()
        {
            // Arrange
            var websiteDto = new WebsiteDto { Url = "https://existing.com", Title = "New Attempt" };
            var userId = Guid.NewGuid();
            var existingWebsite = new Website { Id = Guid.NewGuid(), Url = "https://existing.com", Title = "Original Title" };

            _unitOfWorkMock.Setup(u => u.Websites.GetByUrlAsync(websiteDto.Url))
                .ReturnsAsync(existingWebsite);

            // Act
            var result = await _websiteService.SubmitWebsiteAsync(websiteDto, userId);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Original Title");
            
            // Verify AddAsync was never called
            _unitOfWorkMock.Verify(u => u.Websites.AddAsync(It.IsAny<Website>()), Times.Never);
        }
    }
}