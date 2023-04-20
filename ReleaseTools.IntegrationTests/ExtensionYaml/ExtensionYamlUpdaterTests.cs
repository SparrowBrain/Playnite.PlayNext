using System;
using System.IO;
using AutoFixture.Xunit2;
using ReleaseTools.ExtensionYaml;
using Xunit;

namespace ReleaseTools.IntegrationTests.ExtensionYaml
{
    public class ExtensionYamlUpdaterTests : IDisposable
    {
        private const string ExtensionYaml = "ExtensionYaml\\TestData\\extension.yaml";
        private const string ExpectedExtensionYaml = "ExtensionYaml\\TestData\\extension_after.yaml";
        private const string ExtensionYamlBefore = "ExtensionYaml\\TestData\\extension_before.yaml";

        public ExtensionYamlUpdaterTests()
        {
            File.Copy(ExtensionYamlBefore, ExtensionYaml);
        }

        [Theory, AutoData]
        public void Update_ReplacesTheVersionWithTheGivenOne(
            ExtensionYamlUpdater sut)
        {
            // Arrange
            var expectedYaml = File.ReadAllText(ExpectedExtensionYaml);

            // Act
            sut.Update(ExtensionYaml, "1.2.3");

            // Assert
            var actual = File.ReadAllText(ExtensionYaml);
            Assert.Equal(expectedYaml, actual);
        }

        public void Dispose()
        {
            File.Delete(ExtensionYaml);
        }
    }
}