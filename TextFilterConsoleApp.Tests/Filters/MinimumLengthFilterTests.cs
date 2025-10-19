using Microsoft.Extensions.Configuration;
using TextFilterConsoleApp.Filters;
using Xunit;

namespace TestFilterConsoleApp.Tests.Filters;

public class MinimumLengthFilterTests
{
    private readonly IConfiguration _configuration;
    private readonly MinimumLengthFilter _filter;

    public MinimumLengthFilterTests()
    {
        _configuration = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:MinimumLengthFilter:Enabled"] = "true",
            ["TextFilters:MinimumLengthFilter:MinimumLength"] = "3"
        });
        _filter = new MinimumLengthFilter(_configuration);
    }

    [Theory]
    [InlineData("a", true)]
    [InlineData("ab", true)]
    [InlineData("abc", false)]
    [InlineData("abcd", false)]
    [InlineData("", true)]
    public void ShouldFilter_WithVariousLengths_ReturnsExpectedResult(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("  a  ", true)]
    [InlineData("  abc  ", false)]
    [InlineData("   ", true)]
    public void ShouldFilter_WithWhitespace_TrimsBeforeChecking(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void ShouldFilter_WhenFilterDisabled_ReturnsFalse()
    {
        var disabledConfig = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:MinimumLengthFilter:Enabled"] = "false",
            ["TextFilters:MinimumLengthFilter:MinimumLength"] = "3"
        });
        var disabledFilter = new MinimumLengthFilter(disabledConfig);

        var result = disabledFilter.ShouldFilter("a");
        Assert.False(result);
    }

    [Fact]
    public void Constructor_WithCustomMinimumLength_UsesConfiguredValue()
    {
        var customConfig = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:MinimumLengthFilter:Enabled"] = "true",
            ["TextFilters:MinimumLengthFilter:MinimumLength"] = "5"
        });
        var customFilter = new MinimumLengthFilter(customConfig);

        Assert.True(customFilter.ShouldFilter("test"));
        Assert.False(customFilter.ShouldFilter("testing"));
    }

    [Fact]
    public void Name_ReturnsCorrectFilterName()
    {
        Assert.Equal("MinimumLengthFilter", _filter.Name);
    }

    [Fact]
    public void IsEnabled_ReturnsConfigurationValue()
    {
        Assert.True(_filter.IsEnabled);
    }

    [Fact]
    public void MinimumLength_ReturnsConfigurationValue()
    {
        Assert.Equal(3, _filter.MinimumLength);
    }
}