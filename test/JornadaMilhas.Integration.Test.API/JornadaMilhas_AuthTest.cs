using JornadaMilhas.API.DTO.Auth;
using System.Net.Http.Json;
using System.Net;
using JornadaMilhas.Integration.Test.API.HostTest;


namespace JornadaMilhas.Integration.Test.API;

public class JornadaMilhas_AuthTest: IClassFixture<JornadaMilhasWebApplicationFactory>
{

    private JornadaMilhasWebApplicationFactory app;
    public JornadaMilhas_AuthTest(JornadaMilhasWebApplicationFactory _app)
    {

        app = _app;
    }

        [Fact]
    public async Task POST_Efetua_Login_Com_Sucesso()
    {
        //Arrange   

        var user = new UserDTO { Email = "tester@email.com", Password = "Senha123@" };

        using var client = await app.GetClientWithAccessTokenAsync();

        //Act
        var resultado = await client.PostAsJsonAsync("/auth-login", user);

        //Assert
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);
    }


    [Fact]
    public async Task POST_Efetua_Login_Com_Falha()
    {

        var user = new UserDTO { Email = "test@email.com", Password = "Senha123@" };

        using var client = await app.GetClientWithAccessTokenAsync();

        var resultado = await client.PostAsJsonAsync("/auth-login", user);

        Assert.Equal(HttpStatusCode.BadRequest, resultado.StatusCode);
    }
}