using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TextFilterConsoleApp.Filters;
using TextFilterConsoleApp.Services;
using Xunit;

namespace TestFilterConsoleApp.Tests.Services;

public class TextProcessorTests
{
    private readonly Mock<IFileService> _mockFileService;
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<TextProcessor>> _mockLogger;
    private readonly TextProcessor _textProcessor;

    public TextProcessorTests()
    {
        _mockFileService = new Mock<IFileService>();
        _configuration = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["Processing:ChunkSize"] = "1000"
        });
        _mockLogger = new Mock<ILogger<TextProcessor>>();

        _textProcessor = new TextProcessor(
            _mockFileService.Object,
            new List<ITextFilter>(),
            _configuration,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task ProcessFileAsync_WithNoFilters_ReturnsOriginalText()
    {
        var testLines = new[] { "Hello world test" }.ToAsyncEnumerable();
        _mockFileService.Setup(fs => fs.ReadTextAsync("test.txt", It.IsAny<CancellationToken>()))
                       .Returns(testLines);

        var result = await _textProcessor.ProcessFileAsync("test.txt");

        Assert.Equal("Hello world test", result);
    }

    [Fact]
    public async Task ProcessFileAsync_WithEnabledFilters_AppliesFiltering()
    {
        var filter1 = new Mock<ITextFilter>();
        filter1.Setup(f => f.IsEnabled).Returns(true);
        filter1.Setup(f => f.Name).Returns("TestFilter1");
        filter1.Setup(f => f.ShouldFilter("test")).Returns(true);
        filter1.Setup(f => f.ShouldFilter("Hello")).Returns(false);
        filter1.Setup(f => f.ShouldFilter("world")).Returns(false);

        var testLines = new[] { "Hello world test" }.ToAsyncEnumerable();
        _mockFileService.Setup(fs => fs.ReadTextAsync("test.txt", It.IsAny<CancellationToken>()))
                       .Returns(testLines);

        var processor = new TextProcessor(
            _mockFileService.Object,
            new[] { filter1.Object },
            _configuration,
            _mockLogger.Object
        );

        var result = await processor.ProcessFileAsync("test.txt");

        Assert.Equal("Hello world", result);
    }

    [Fact]
    public async Task ProcessFileAsync_WithDisabledFilters_IgnoresFilters()
    {
        var filter1 = new Mock<ITextFilter>();
        filter1.Setup(f => f.IsEnabled).Returns(false);
        filter1.Setup(f => f.Name).Returns("DisabledFilter");
        filter1.Setup(f => f.ShouldFilter(It.IsAny<string>())).Returns(true);

        var testLines = new[] { "Hello world test" }.ToAsyncEnumerable();
        _mockFileService.Setup(fs => fs.ReadTextAsync("test.txt", It.IsAny<CancellationToken>()))
                       .Returns(testLines);

        var processor = new TextProcessor(
            _mockFileService.Object,
            new[] { filter1.Object },
            _configuration,
            _mockLogger.Object
        );

        var result = await processor.ProcessFileAsync("test.txt");

        Assert.Equal("Hello world test", result);
        filter1.Verify(f => f.ShouldFilter(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ProcessFileAsync_WithMultipleLines_ProcessesAllLines()
    {
        var testLines = new[] { "Hello world", "This is test", "Another line" }.ToAsyncEnumerable();
        _mockFileService.Setup(fs => fs.ReadTextAsync("test.txt", It.IsAny<CancellationToken>()))
                       .Returns(testLines);

        var result = await _textProcessor.ProcessFileAsync("test.txt");

        Assert.Equal("Hello world This is test Another line", result);
    }

    [Fact]
    public async Task ProcessFileAsync_WithChunkedProcessing_HandlesLargeInput()
    {
        var chunkedConfig = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["Processing:ChunkSize"] = "2" // Small chunk size for testing
        });

        var filter1 = new Mock<ITextFilter>();
        filter1.Setup(f => f.IsEnabled).Returns(true);
        filter1.Setup(f => f.Name).Returns("TestFilter");
        filter1.Setup(f => f.ShouldFilter("world")).Returns(true);
        filter1.Setup(f => f.ShouldFilter(It.IsNotIn("world"))).Returns(false);

        var testLines = new[] { "Hello world test case" }.ToAsyncEnumerable();
        _mockFileService.Setup(fs => fs.ReadTextAsync("test.txt", It.IsAny<CancellationToken>()))
                       .Returns(testLines);

        var processor = new TextProcessor(
            _mockFileService.Object,
            new[] { filter1.Object },
            chunkedConfig,
            _mockLogger.Object
        );

        var result = await processor.ProcessFileAsync("test.txt");

        Assert.Equal("Hello test case", result);
    }

    [Fact]
    public async Task ProcessFileAsync_WithEmptyInput_ReturnsEmptyString()
    {
        var testLines = new string[0].ToAsyncEnumerable();
        _mockFileService.Setup(fs => fs.ReadTextAsync("test.txt", It.IsAny<CancellationToken>()))
                       .Returns(testLines);

        var result = await _textProcessor.ProcessFileAsync("test.txt");

        Assert.Equal("", result);
    }

    [Fact]
    public async Task ProcessFileAsync_WithWhitespaceOnlyWords_FiltersOutWhitespace()
    {
        var testLines = new[] { "Hello   world  test   " }.ToAsyncEnumerable();
        _mockFileService.Setup(fs => fs.ReadTextAsync("test.txt", It.IsAny<CancellationToken>()))
                       .Returns(testLines);

        var result = await _textProcessor.ProcessFileAsync("test.txt");

        Assert.Equal("Hello world test", result);
    }
}

