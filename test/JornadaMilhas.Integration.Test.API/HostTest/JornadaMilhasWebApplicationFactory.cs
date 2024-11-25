using JornadaMilhas.API.DTO.Auth;
using JornadaMilhas.Dados;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Testcontainers.MsSql;

namespace JornadaMilhas.Integration.Test.API.HostTest;
public class JornadaMilhasWebApplicationFactory : WebApplicationFactory<Program>,IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Strong_password_123!")
        .Build();

    public async Task InitializeAsync()=> await _mssqlContainer.StartAsync();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {     
       builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<JornadaMilhasContext>));

            services.AddDbContext<JornadaMilhasContext>(options =>
                options
                .UseLazyLoadingProxies()
                .UseSqlServer(_mssqlContainer.GetConnectionString()));
        });       
    }

    public new async Task DisposeAsync()
    {
        await _mssqlContainer.DisposeAsync();
    }

    public async Task<HttpClient> GetClientWithAccessTokenAsync()
    {
        var client = this.CreateClient();

        var user = new UserDTO { Email = "tester@email.com", Password = "Senha123@" };

        var response = await client.PostAsJsonAsync("/auth-login", user);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var token = JsonSerializer.Deserialize<UserTokenDTO>(result, options);

        var clientAutenticado = this.CreateClient();
        
        clientAutenticado.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.Token);
        return clientAutenticado;
    }
}
