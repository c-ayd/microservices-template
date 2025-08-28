using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AuthService.Application.Abstractions.Authentication;
using AuthService.Application.Abstractions.Crypto;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Crypto;
using AuthService.Infrastructure.Settings.Authentication;
using AuthService.Test.Integration.Api.Collections;
using AuthService.Test.Utility;
using AuthService.Test.Utility.Fixtures.Hosting;

namespace AuthService.Test.Integration.Api.Controllers.Authentication
{
    [Collection(nameof(TestHostCollection))]
    public partial class AuthenticationControllerTest
    {
        private readonly TestHostFixture _testHostFixture;
        private readonly IHashing _hashing;
        private readonly IJwt _jwt;

        public AuthenticationControllerTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
            _testHostFixture.SetDefaultOptions();

            _hashing = new Hashing();

            var config = ConfigurationHelper.CreateConfiguration();
            var jwtSettings = config.GetSection(JwtSettings.SettingsKey).Get<JwtSettings>()!;
            _jwt = new Jwt(Options.Create(jwtSettings), new TokenGenerator());
        }
    }
}
