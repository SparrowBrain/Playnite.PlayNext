using AutoFixture.Xunit2;
using ReleaseTools.InstallerManifestYaml;
using Xunit;

namespace ReleaseTools.IntegrationTests.InstallerManifestYaml
{
    public class PlayniteSdkVersionParserTests
    {
        private const string ProjectFile = @"InstallerManifestYaml\TestData\PlayNext.csproj";

        [Fact]
        public void GetVersion_ReturnsVersionFromProjectFile()
        {
            // Arrange
            var sut = new PlayniteSdkVersionParser(ProjectFile);

            // Act
            var result = sut.GetVersion();

            // Assert
            Assert.Equal("70.50.60", result);
        }
    }
}