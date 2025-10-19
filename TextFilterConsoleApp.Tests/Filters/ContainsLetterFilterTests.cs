using Microsoft.Extensions.Configuration;
using TextFilterConsoleApp.Filters;
using Xunit;

namespace TestFilterConsoleApp.Tests.Filters;

public class ContainsLetterFilterTests
{
    private readonly IConfiguration _configuration;
    private readonly ContainsLetterFilter _filter;

    public ContainsLetterFilterTests()
    {
        _configuration = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:ContainsLetterFilter:Enabled"] = "true",
            ["TextFilters:ContainsLetterFilter:Letter"] = "t"
        });
        _filter = new ContainsLetterFilter(_configuration);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("cat", true)]
    [InlineData("battle", true)]
    [InlineData("hello", false)]
    [InlineData("world", false)]
    [InlineData("house", false)]
    public void ShouldFilter_WithLetterT_ReturnsExpectedResult(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("TEST", true)]
    [InlineData("Test", true)]
    [InlineData("tEsT", true)]
    [InlineData("HELLO", false)]
    public void ShouldFilter_WithDifferentCasing_ReturnsExpectedResult(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("  test  ", true)]
    public void ShouldFilter_WithWhitespaceAndEmpty_ReturnsExpectedResult(string word, bool expectedResult)
    {
        var result = _filter.ShouldFilter(word);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Constructor_WithCustomLetter_UsesConfiguredLetter()
    {
        var customConfig = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:ContainsLetterFilter:Enabled"] = "true",
            ["TextFilters:ContainsLetterFilter:Letter"] = "x"
        });
        var customFilter = new ContainsLetterFilter(customConfig);

        Assert.True(customFilter.ShouldFilter("exact"));
        Assert.False(customFilter.ShouldFilter("test"));
    }

    [Fact]
    public void Constructor_WithEmptyLetterConfig_DefaultsToT()
    {
        var emptyConfig = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:ContainsLetterFilter:Enabled"] = "true",
            ["TextFilters:ContainsLetterFilter:Letter"] = ""
        });
        var defaultFilter = new ContainsLetterFilter(emptyConfig);

        Assert.True(defaultFilter.ShouldFilter("test"));
    }

    [Fact]
    public void ShouldFilter_WhenFilterDisabled_ReturnsFalse()
    {
        var disabledConfig = TestConfigurationBuilder.Create(new Dictionary<string, string>
        {
            ["TextFilters:ContainsLetterFilter:Enabled"] = "false",
            ["TextFilters:ContainsLetterFilter:Letter"] = "t"
        });
        var disabledFilter = new ContainsLetterFilter(disabledConfig);

        var result = disabledFilter.ShouldFilter("test");
        Assert.False(result);
    }

    [Fact]
    public void Name_ReturnsCorrectFilterName()
    {
        Assert.Equal("ContainsLetterFilter", _filter.Name);
    }

    [Fact]
    public void IsEnabled_ReturnsConfigurationValue()
    {
        Assert.True(_filter.IsEnabled);
    }

    [Fact]
    public void TargetLetter_ReturnsConfigurationValue()
    {
        Assert.Equal('t', _filter.TargetLetter);
    }
}