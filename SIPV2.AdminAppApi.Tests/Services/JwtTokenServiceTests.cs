using Microsoft.Extensions.Configuration;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Services;

public class JwtTokenServiceTests
{
    private static JwtTokenService CreateService(string? key = "clave-de-pruebas-de-al-menos-32-caracteres")
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = key,
                ["Jwt:Issuer"] = "SIPV2.Tests",
                ["Jwt:Audience"] = "SIPV2.Tests",
                ["Jwt:ExpiresMinutes"] = "30",
            })
            .Build();

        return new JwtTokenService(configuration);
    }

    [Fact]
    public void GenerateToken_ReturnsNonEmptyToken()
    {
        var service = CreateService();
        var user = new Mduser { Id = Guid.NewGuid(), Login = "jperez", Name = "Juan" };

        var token = service.GenerateToken(user, ["Admin"]);

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Equal(3, token.Split('.').Length); // header.payload.signature
    }

    [Fact]
    public void GenerateToken_WithoutJwtKey_Throws()
    {
        var service = CreateService(key: null);
        var user = new Mduser { Id = Guid.NewGuid(), Login = "jperez", Name = "Juan" };

        Assert.Throws<InvalidOperationException>(() => service.GenerateToken(user, []));
    }
}
