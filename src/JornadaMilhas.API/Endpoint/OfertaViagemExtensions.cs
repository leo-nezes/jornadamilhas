using JornadaMilhas.API.DTO.Request;
using JornadaMilhas.API.DTO.Response;
using JornadaMilhas.API.Service;
using JornadaMilhas.Dados.Database;
using JornadaMilhas.Dominio.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace JornadaMilhas.API.Endpoint;

public static class OfertaViagemExtensions
{
    public static void AddEndPointOfertas(this WebApplication app)
    {
        app.MapPost("/ofertas-viagem", async ([FromServices] OfertaViagemConverter converter, [FromServices] EntityDAL<OfertaViagem> entityDAL, [FromBody] OfertaViagemRequest ofertaReq) =>
        {
            OfertaViagem oferta = new();
            try
            {
                oferta = converter.RequestToEntity(ofertaReq);
                if (oferta.EhValido)
                {
                   await entityDAL.Adicionar(oferta);
                    return Results.Created("Oferta criada com sucesso!", converter.EntityToResponse(oferta));
                } 
                throw new Exception("Oferta inválida");
                
            }
            catch (Exception ex)
            {
               return Results.Problem($"Erro:{oferta.Erros} => {ex.Message}");
            }
            
        }).WithTags("Oferta Viagem").WithSummary("Adiciona uma nova oferta de viagem.").WithOpenApi().RequireAuthorization();

        app.MapGet("/ofertas-viagem", async ([FromServices] OfertaViagemConverter converter, [FromServices] EntityDAL<OfertaViagem> entityDAL) =>
        {        
            return  Results.Ok(converter.EntityListToResponseList(await entityDAL.Listar()));
        }).WithTags("Oferta Viagem").WithSummary("Listagem de ofertas de viagem cadastrados.").WithOpenApi().RequireAuthorization();

        app.MapGet("/ofertas-viagem/{id}", ([FromServices] OfertaViagemConverter converter, [FromServices] EntityDAL<OfertaViagem> entityDAL,int id) =>
        {
            var oferta = entityDAL.RecuperarPor(a => a.Id == id);
            if (oferta is null) return Results.NotFound();
            return Results.Ok(converter.EntityToResponse(oferta));
        }).WithTags("Oferta Viagem").WithSummary("Obtem oferta de viagem por id.").WithOpenApi().RequireAuthorization();

        app.MapDelete("/ofertas-viagem/{id}", async ([FromServices] OfertaViagemConverter converter, [FromServices] EntityDAL<OfertaViagem> entityDAL, int id) =>
        {
            var oferta = entityDAL.RecuperarPor(a => a.Id == id);
            if (oferta is null)
            {
                return Results.NotFound($"Oferta com ID={id} para exclusão não encontrado.");
            }
            await entityDAL.Deletar(oferta);
            return Results.NoContent();
        }).WithTags("Oferta Viagem").WithSummary("Deleta uma oferta de viagem por id.").WithOpenApi().RequireAuthorization();

        app.MapPut("/ofertas-viagem", async([FromServices] OfertaViagemConverter converter, [FromServices] EntityDAL<OfertaViagem> entityDAL, [FromBody] OfertaViagemEditRequest ofertaReq) =>
        {
           var ofertaAtualizada = entityDAL.RecuperarPor(a => a.Id == ofertaReq.Id);
            var ofertaConvertida = converter.RequestToEntity(ofertaReq);
            if (ofertaAtualizada is null)
            {
                return Results.NotFound($"Oferta com ID={ofertaReq.Id} para atualização não encontrado.");
            }
            ofertaAtualizada.Periodo = ofertaConvertida.Periodo;
            ofertaAtualizada.Preco = ofertaReq.preco;
            await entityDAL.Atualizar(ofertaAtualizada);
            return Results.NoContent();

        }).WithTags("Oferta Viagem").WithSummary("Atualiza uma oferta de viagem.").WithOpenApi().RequireAuthorization();

        app.MapGet("/ofertas-viagem/{pagina}/{tamanhoPorPagina}", async ([FromServices] OfertaViagemConverter converter, [FromServices] EntityDAL<OfertaViagem> entityDAL, int pagina, int tamanhoPorPagina) =>
        {
            var oferta = await entityDAL.ListarPaginado(pagina, tamanhoPorPagina);
            if (oferta is null) return Results.NotFound();
            return Results.Ok(converter.EntityListToResponseList(oferta));
        }).WithTags("Oferta Viagem").WithSummary("Obtem oferta de viagem paginado.").WithOpenApi().RequireAuthorization();

        app.MapGet("/ofertas-viagem/ultimo-registro", async ([FromServices] OfertaViagemConverter converter, [FromServices] EntityDAL<OfertaViagem> entityDAL) => {
            var oferta = converter.EntityToResponse(await entityDAL.UltimoRegistroAsync());
            if (oferta is null) { return Results.NotFound(); }
            return Results.Ok(oferta);
        }).WithTags("Oferta Viagem").WithSummary("Retorna a última oferta viagem cadastrada.").WithOpenApi().RequireAuthorization();
    }
}
