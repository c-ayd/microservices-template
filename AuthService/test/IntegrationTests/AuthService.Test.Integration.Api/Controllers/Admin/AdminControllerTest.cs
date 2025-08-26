using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AuthService.Application.Abstractions.Authentication;
using AuthService.Infrastructure.Authentication;
using AuthService.Infrastructure.Crypto;
using AuthService.Infrastructure.Settings.Authentication;
using AuthService.Test.Integration.Api.Collections;
using AuthService.Test.Utility;
using AuthService.Test.Utility.Fixtures.Hosting;

namespace AuthService.Test.Integration.Api.Controllers.Admin
{
    [Collection(nameof(TestHostCollection))]
    public partial class AdminControllerTest
    {
        private readonly TestHostFixture _testHostFixture;
        private readonly IJwt _jwt;

        public AdminControllerTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
            _testHostFixture.SetDefaultOptions();

            var config = ConfigurationHelper.CreateConfiguration();
            var jwtSettings = config.GetSection(JwtSettings.SettingsKey).Get<JwtSettings>()!;
            _jwt = new Jwt(Options.Create(jwtSettings), new TokenGenerator());
        }
    }
}
