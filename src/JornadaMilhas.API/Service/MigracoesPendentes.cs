using JornadaMilhas.Dados;
using Microsoft.EntityFrameworkCore;

namespace JornadaMilhas.API.Service;
public static class MigracoesPendentes
{
    public static void ExecuteMigration(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var serviceDb = serviceScope.ServiceProvider.GetService<JornadaMilhasContext>();

            if (serviceDb != null)
            {
                try
                {
                    // Verifica se o banco de dados pode ser conectado
                    if (!serviceDb.Database.CanConnect())
                    {
                        // Garante que o banco de dados seja criado, se não existir.
                        serviceDb.Database.EnsureCreated();

                        // Se não puder conectar, isso significa que o banco não existe, então cria o banco
                        serviceDb.Database.Migrate();
                        Console.WriteLine("Banco de dados criado e migrações aplicadas.");
                    }
                    else
                    {
                        // Se o banco já existir, apenas aplica as migrações pendentes
                        serviceDb.Database.Migrate();
                        Console.WriteLine("Migrações aplicadas com sucesso.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao tentar conectar ao banco de dados: {ex.Message}");
                }
            }
        }
    }
}
