using JornadaMilhas.Dominio.Entidades;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using JornadaMilhas.Integration.Test.API.HostTest;
using JornadaMilhas.Dominio.ValueObjects;
using Newtonsoft.Json.Linq;

namespace JornadaMilhas.Integration.Test.API;
public class JornadaMilhas_OfertaViagemTest :IClassFixture<JornadaMilhasWebApplicationFactory>
{

    private JornadaMilhasWebApplicationFactory app;
    public JornadaMilhas_OfertaViagemTest(JornadaMilhasWebApplicationFactory _app)
    {
     
        app = _app;
    }

    //[Fact]
    public async Task GET_Retorna_Todas_OfertaViagem()
    {
        //Arrange 
        using var client = await app.GetClientWithAccessTokenAsync();

        //Act
        var resultado = await client.GetAsync("/ofertas-viagem");
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>("/ofertas-viagem");

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);

    }

    //[Fact]
    public async Task GET_Retorna_OfertaViagem_Por_Id()
    {
        //Arrange
        using var client = await app.GetClientWithAccessTokenAsync();

        int id = 1;
        //Act  
        var response = await client.GetFromJsonAsync<OfertaViagem>("/ofertas-viagem/" + id);

        //Assert
        Assert.True(response != null);
        Assert.Equal(id, response.Id);
        Assert.Contains("Origem1", response.ToString());
    }


    [Theory]
    [InlineData("Manaus-AM", "São Paulo-SP", "2024-01-01", "2024-01-02", 100)]
    [InlineData("Recife-PE", "São Paulo-SP", "2024-01-01", "2024-01-03", 110)]
    [InlineData("Vitória-ES", "São Paulo-SP", "2024-01-01", "2024-01-04", 120)]
    [InlineData("Rio de Janeiro-RJ", "São Paulo-SP", "2024-01-01", "2024-01-22", 250)]
    public async Task POST_Cadastra_OfertaViagem(string origem, string destino, string dataIn, string dataVol, double preco)
    {
        //Arrange  

        using var client = await app.GetClientWithAccessTokenAsync();

        var ofertaViagem = new OfertaViagem()
        {

            Preco = preco,
            Rota = new Rota(origem, destino),
            Periodo = new Periodo(DateTime.Parse(dataIn), DateTime.Parse(dataVol))

        };
        //Act  
        var response = await client.PostAsJsonAsync("/ofertas-viagem/", ofertaViagem);


        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task POST_Tentiva_De_Cadastro_OfertaViagem_Invalido_Com_Rota_Nula()
    {
        //Arrange
        using var client = await app.GetClientWithAccessTokenAsync();

        var ofertaViagem = new OfertaViagem()
        {
            Preco = 100,
            Rota = null,
            Periodo = new Periodo(DateTime.Parse("2024-01-01"), DateTime.Parse("2024-01-22"))

        };
        //Act  
        var response = await client.PostAsJsonAsync("/ofertas-viagem/", ofertaViagem);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }


    [Fact]
    public async Task POST_Tentiva_De_Cadastro_OfertaViagem_Usando_Token_Invalido()
    {
        //Arrange

        using var client = await app.GetClientWithAccessTokenAsync();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");

        var ofertaViagem = new OfertaViagem()
        {
            Preco = 100,
            Rota = new Rota("Origem", "Destino"),
            Periodo = new Periodo(DateTime.Parse("2024-01-01"), DateTime.Parse("2024-01-22"))

        };
        //Act  
        var response = await client.PostAsJsonAsync("/ofertas-viagem/", ofertaViagem);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task POST_Tentiva_De_Cadastro_OfertaViagem_Com_Rota_EndPoint_Vazia()
    {
        //Arrange  

        using var client = await app.GetClientWithAccessTokenAsync();
        var ofertaViagem = new OfertaViagem()
        {
            Preco = 100,
            Rota = new Rota("Origem", "Destino"),
            Periodo = new Periodo(DateTime.Parse("2024-01-01"), DateTime.Parse("2024-01-22"))

        };
        //Act  
        var response = await client.PostAsJsonAsync("", ofertaViagem);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_OfertaViagem_Por_Id()
    {
        //Arrange  


        using var client = await app.GetClientWithAccessTokenAsync();

        //Act
        var ofertaViagem = new OfertaViagem()
        {
            Preco = 100.0,
            Rota = new Rota("Origem 1", "Destino 1"),
            Periodo = new Periodo(DateTime.Parse("2024-01-01"), DateTime.Parse("2024-01-04"))
        };
        var responseAdd = await client.PostAsJsonAsync("/ofertas-viagem/", ofertaViagem);

        var responseUltimoAdd = await client.GetFromJsonAsync<OfertaViagem>($"/ofertas-viagem/ultimo-registro");

        var response = await client.DeleteAsync($"/ofertas-viagem/{responseUltimoAdd!.Id}");

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
        var ofertaViagem = new OfertaViagem()
        {
            Preco = 100.0,
            Rota = new Rota("Origem 1", "Destino 1"),
            Periodo = new Periodo(DateTime.Parse("2024-01-01"), DateTime.Parse("2024-01-04"))
        };
        var responseAdd = await client.PostAsJsonAsync("/ofertas-viagem/", ofertaViagem);

        var responseUltimoAdd = await client.GetFromJsonAsync<OfertaViagem>($"/ofertas-viagem/ultimo-registro");

        responseUltimoAdd!.Rota.Origem = "Origem Atualizada";
        responseUltimoAdd!.Rota.Origem = "Destino Atualizado";

        var response = await client.PutAsJsonAsync($"/ofertas-viagem/", responseUltimoAdd);

        //Assert
        Assert.True(response != null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }

    [Fact]
    public async Task GET_Retorna_OfertaViagem_Paginada()
    {
        //Arrange
        using var client =  await app.GetClientWithAccessTokenAsync();
              
        int pagina = 1;
        int tamanhoPorPagina = 2;
        //Act  
        var resultado = await client.GetAsync($"/ofertas-viagem/{pagina}/{tamanhoPorPagina}");
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem/{pagina}/{tamanhoPorPagina}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);
        Assert.True(response != null);
        Assert.Equal(tamanhoPorPagina, response.Count());
    }

    [Fact]
    public async Task GET_Retorna_OfertaViagem_Paginada_Pega_Oferta_Terceira_Pagina()
    {
        //Arrange  


        using var client = await app.GetClientWithAccessTokenAsync();

        int pagina = 1;
        int tamanhoPorPagina = 2;

        //Act  
        var resultado = await client.GetAsync($"/ofertas-viagem/{pagina}/{tamanhoPorPagina}");
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem/{pagina}/{tamanhoPorPagina}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);
        Assert.True(response != null);
        Assert.Equal(tamanhoPorPagina, response.Count());
    }

    [Fact]
    public async Task GET_Retorna_OfertaViagem_Paginada_Com_Pagina_E_TamanhoPagina_Zerado()
    {
        //Arrange 


        using var client = await app.GetClientWithAccessTokenAsync();

        int pagina = 0;
        int tamanhoPorPagina = 0;

        //Act  
        var resultado = await client.GetAsync($"/ofertas-viagem/{pagina}/{tamanhoPorPagina}");
        var response = await client.GetFromJsonAsync<ICollection<OfertaViagem>>($"/ofertas-viagem/{pagina}/{tamanhoPorPagina}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, resultado.StatusCode);
        Assert.True(response != null);
        Assert.Equal(tamanhoPorPagina, response.Count());
    }

    [Fact]
    public async Task GET_Tenta_Obter_OfertaViagem_Com_Id_Invalido()
    {
        //Arrange 

        using var client = await app.GetClientWithAccessTokenAsync();

        int id = int.MinValue;
        //Act  
        var resultado = await client.GetAsync("/ofertas-viagem/" + id);

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, resultado.StatusCode);

    }
}
