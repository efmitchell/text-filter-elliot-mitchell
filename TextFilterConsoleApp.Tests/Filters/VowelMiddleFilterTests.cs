using Microsoft.Extensions.Configuration;
using TextFilterConsoleApp.Filters;
using Xunit;

namespace TestFilterConsoleApp.Tests.Filters;

public class VowelMiddleFilterTests
{
    private readonly IConfiguration _configuration;
    private readonly VowelMiddleFilter _filter;

    public VowelMiddleFilterTests()
    {
        _configuration = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:VowelMiddleFilter:Enabled"] = "true"
        });
        _filter = new VowelMiddleFilter(_configuration);
    }

    [Theory]
    [InlineData("clean", true)]
    [InlineData("what", true)]
    [InlineData("currently", true)]
    [InlineData("the", false)]
    [InlineData("rather", false)]
    public void ShouldFilter_WithVowelInMiddle_ReturnsExpectedResult(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("a", false)]
    [InlineData("at", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    public void ShouldFilter_WithShortWords_ReturnsFalse(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("word", true)]
    [InlineData("hello", false)]
    [InlineData("world", false)]
    public void ShouldFilter_WithFourAndFiveLetterWords_ReturnsExpectedResult(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("CLEAN", true)]
    [InlineData("THE", false)]
    [InlineData("ClEaN", true)]
    public void ShouldFilter_WithDifferentCasing_ReturnsExpectedResult(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void ShouldFilter_WhenFilterDisabled_ReturnsFalse()
    {
        var disabledConfig = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:VowelMiddleFilter:Enabled"] = "false"
        });
        var disabledFilter = new VowelMiddleFilter(disabledConfig);

        var result = disabledFilter.ShouldFilter("clean");
        Assert.False(result);
    }

    [Fact]
    public void Name_ReturnsCorrectFilterName()
    {
        Assert.Equal("VowelMiddleFilter", _filter.Name);
    }

    [Fact]
    public void IsEnabled_ReturnsConfigurationValue()
    {
        Assert.True(_filter.IsEnabled);
    }
}