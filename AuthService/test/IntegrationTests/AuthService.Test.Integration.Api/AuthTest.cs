using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Crypto;
using AuthService.Infrastructure.Settings.Authentication;
using AuthService.Test.Integration.Api.Collections;
using AuthService.Test.Utility;
using AuthService.Test.Utility.Fixtures.Hosting;

namespace AuthService.Test.Integration.Api
{
    [Collection(nameof(TestHostCollection))]
    public partial class AuthTest
    {
        private readonly TestHostFixture _testHostFixture;
        private readonly JwtSettings _jwtSettings;
        private readonly Jwt _jwt;

        public AuthTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;

            var config = ConfigurationHelper.CreateConfiguration();
            _jwtSettings = config.GetSection(JwtSettings.SettingsKey).Get<JwtSettings>()!;

            _jwt = new Jwt(Options.Create(_jwtSettings), new TokenGenerator());
        }
    }
}
