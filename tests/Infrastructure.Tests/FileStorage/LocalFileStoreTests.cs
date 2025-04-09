using FluentAssertions;
using Infrastructure.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Xunit;

namespace Infrastructure.Tests.FileStorage;

public class LocalFileStoreTests
{
    private readonly Mock<IHostEnvironment> _mockEnvironment;
    private readonly Mock<ILogger<LocalFileStore>> _mockLogger;
    private readonly Mock<IFormFile> _mockFormFile;
    private readonly string _testRootPath;
    private readonly LocalFileStore _fileStore;

    public LocalFileStoreTests()
    {
        _mockEnvironment = new Mock<IHostEnvironment>();
        _mockLogger = new Mock<ILogger<LocalFileStore>>();
        _mockFormFile = new Mock<IFormFile>();

        // set a temporary test path
        _testRootPath = Path.Combine(Path.GetTempPath(), "TestReceipts");
        _mockEnvironment.Setup(x => x.ContentRootPath).Returns(Path.GetTempPath());

        _fileStore = new LocalFileStore(_mockEnvironment.Object, _mockLogger.Object);

        // clean up existing test directory
        if (Directory.Exists(_testRootPath))
        {
            Directory.Delete(_testRootPath, true);
        }
    }

    [Fact]
    public async Task SaveAsync_ValidFile_SavesFileAndReturnsRelativePath()
    {
        // arrange
        var fileName = "test.pdf";
        var content = "This is a test file content";
        var contentBytes = Encoding.UTF8.GetBytes(content);

        _mockFormFile.Setup(f => f.FileName).Returns(fileName);
        _mockFormFile.Setup(f => f.Length).Returns(contentBytes.Length);

        _mockFormFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => 
            {
                var ms = new MemoryStream(contentBytes);
                ms.CopyTo(stream);
            })
            .Returns(Task.CompletedTask);

        // act
        var result = await _fileStore.SaveAsync(_mockFormFile.Object);

        // assert
        result.Should().NotBeNullOrEmpty();
        result.Should().StartWith("Receipts/");
        result.Should().EndWith(".pdf");

        _mockFormFile.Verify(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }
} 