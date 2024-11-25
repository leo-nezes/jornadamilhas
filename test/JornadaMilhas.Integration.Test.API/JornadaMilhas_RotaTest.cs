using JornadaMilhas.Dominio.Entidades;
using System.Net.Http.Json;
using System.Net;
using JornadaMilhas.Integration.Test.API.HostTest;
using System.Net.Http.Headers;

namespace JornadaMilhas.Integration.Test.API;
public class JornadaMilhas_RotaTest : IClassFixture<JornadaMilhasWebApplicationFactory>
{
    private JornadaMilhasWebApplicationFactory app;
    public JornadaMilhas_RotaTest(JornadaMilhasWebApplicationFactory _app)
    {

        app = _app;
    }

    [Fact]
    public async Task GET_Retorna_Todas_Rotas()
    {
        //Arrange  
        using var client = await app.GetClientWithAccessTokenAsync();

        //Act
        var resultado = await client.GetAsync("/rota-viagem");
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>("/rota-viagem");

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);

    }

    [Fact]
    public async Task GET_Retorna_RotaPor_Id()
    {
        //Arrange  
        using var client = await app.GetClientWithAccessTokenAsync();

        int id = 1;
        //Act  
        var response = await client.GetFromJsonAsync<OfertaViagem>("/rota-viagem/" + id);

        //Assert
        Assert.True(response != null);
        Assert.Equal(id, response.Id);
    }


    [Theory]
    [InlineData("Manaus-AM", "São Paulo-SP")]
    [InlineData("Recife-PE", "São Paulo-SP")]
    [InlineData("Vitória-ES", "São Paulo-SP")]
    [InlineData("Rio de Janeiro-RJ", "São Paulo-SP")]
    public async Task POST_Cadastra_Rota(string origem, string destino)
    {
        //Arrange  
        using var client = await app.GetClientWithAccessTokenAsync();
        var ofertaViagem = new Rota(origem, destino);

        //Act  
        var response = await client.PostAsJsonAsync("/rota-viagem/", ofertaViagem);


        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task POST_Tentiva_De_Cadastro_RotaViagem_Invalido_Com_Rota_Nula()
    {
        //Arrange  
        using var client = await app.GetClientWithAccessTokenAsync();
        Rota rotaViagem = new();

        //Act  
        var response = await client.PostAsJsonAsync("/rota-viagem/", rotaViagem);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }


    [Fact]
    public async Task POST_Tentiva_De_Cadastro_RotaViagem_Usando_Token_Invalido()
    {
        //Arrange  
        using var client = await app.GetClientWithAccessTokenAsync();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        var ofertaViagem = new Rota("Origem", "Destino");

        //Act  
        var response = await client.PostAsJsonAsync("/rota-viagem/", ofertaViagem);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task POST_Tentiva_De_Cadastro_RotaViagem_Com_Rota_EndPoint_Vazia()
    {
        //Arrange  
        using var client = await app.GetClientWithAccessTokenAsync();
        var rotaViagem = new Rota("Origem", "Destino");

        //Act  
        var response = await client.PostAsJsonAsync("", rotaViagem);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_RotaViagem_Por_Id()
    {
        //Arrange  

        using var client = await app.GetClientWithAccessTokenAsync();

        //Act
        var rotaViagem = new Rota("Origem 1", "Destino 1");

        var responseAdd = await client.PostAsJsonAsync("/rota-viagem/", rotaViagem);

        var responseUltimoAdd = await client.GetFromJsonAsync<Rota>($"/rota-viagem/ultimo-registro");

        var response = await client.DeleteAsync($"/rota-viagem/{responseUltimoAdd!.Id}");

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }

    [Fact]
    public async Task UPDATE_OfertaViagem_Por_Id()
    {
        //Arrange 

        using var client = await app.GetClientWithAccessTokenAsync();

        //Act
        var rotaViagem = new Rota("Origem 1", "Destino 1");

        var responseAdd = await client.PostAsJsonAsync("/rota-viagem/", rotaViagem);

        var responseUltimoAdd = await client.GetFromJsonAsync<Rota>($"/rota-viagem/ultimo-registro");

        responseUltimoAdd!.Origem = "Origem Atualizada";
        responseUltimoAdd!.Destino = "Destino Atualizado";

        var response = await client.PutAsJsonAsync($"/rota-viagem/", responseUltimoAdd);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }
}
