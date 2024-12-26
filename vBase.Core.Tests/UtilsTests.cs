using FluentAssertions;
using vBase.Core.Utilities;

namespace vBase.Core.Tests;

public class UtilsTests
{
  [Test]
  public void AsserNotNull_WhenValueIsNull_ThrowsArgumentNullException()
  {
    // Arrange
    string? value = null;
    string message = "Test message";

    // Act
    Action act = () => value.AsserNotNull(message);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage(message);
  }

  [Test]
  public void AsserNotNull_WhenValueIsNotNull_ReturnsValue()
  {
    // Arrange
    string value = "Test value";
    string message = "Test message";

    // Act
    var result = value.AsserNotNull(message);

    // Assert
    result.Should().Be(value);
  }

  [Test]
  public void BuildUri_WhenCalled_ReturnsUri()
  {
    // Arrange
    var baseUri = new Uri("http://test.com/api");
    var path = "test";
    var queryParams = new Dictionary<string, string>
    {
      { "key1", "&value1" },
      { "key2", "value2" }
    };

    // Act
    var result = Utils.BuildUri(baseUri, path, queryParams);

    // Assert
    result.Should().Be(new Uri("http://test.com/api/test?key1=%26value1&key2=value2"));
  }

  [Test]
  public void LoadEmbeddedJson_WhenResourceExists_ReturnsJson()
  {
    // Arrange
    var path = "CommitmentService.json";

    // Act
    var result = Utils.LoadEmbeddedJson(path);

    // Assert
    result.Should().NotBeEmpty();
  }
}