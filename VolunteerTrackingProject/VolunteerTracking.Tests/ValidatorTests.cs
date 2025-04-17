using Xunit;
using VolunteerTracking;

namespace VolunteerTracking.Tests
{
    public class ValidatorTests
    {
        [Theory]
        [InlineData("Password!", true)]
        [InlineData("pass", false)] // too short
        [InlineData("password", false)] // no uppercase, no special
        [InlineData("Password", false)] // no special char
        [InlineData("Passw!", true)]
        public void IsPasswordValid_ReturnsExpectedResult(string password, bool expected)
        {
            bool result = Validator.IsPasswordValid(password);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("validUser", true)]
        [InlineData("abc", false)] // too short
        [InlineData("invalid_user", false)] // invalid character
        [InlineData("valid user", true)] // space allowed
        public void IsUsernameValid_ReturnsExpectedResult(string username, bool expected)
        {
            bool result = Validator.IsUsernameValid(username);
            Assert.Equal(expected, result);
        }
    }
}
