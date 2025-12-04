using KartverketRegister.Models;
using Xunit;

namespace KartverketRegister.Tests.Models
{
    public class GeneralResponseTests
    {
        [Fact]
        public void Constructor_SetsProperties_Correctly_TwoArgs()
        {
            // Arrange
            bool success = true;
            string message = "Test successful";

            // Act
            var response = new GeneralResponse(success, message);

            // Assert
            Assert.Equal(success, response.Success);
            Assert.Equal(message, response.Message);
            Assert.Null(response.Data);
        }

        [Fact]
        public void Constructor_SetsProperties_Correctly_ThreeArgs()
        {
            // Arrange
            bool success = false;
            string message = "Test failed";
            object data = new { Value = 123 };

            // Act
            var response = new GeneralResponse(success, message, data);

            // Assert
            Assert.Equal(success, response.Success);
            Assert.Equal(message, response.Message);
            Assert.Equal(data, response.Data);
        }
    }
}
