using JornadaMilhas.Dominio.Entidades;
using JornadaMilhas.Dominio.ValueObjects;

namespace JornadaMilhas.Unity.Test;

public class UnitTest1
{
    [Fact]
    public void PrimeiroMetodoDeTeste()
    {
        Assert.True(true);
    }
    [Fact(DisplayName = "DeveCriarOfertaCorretamente")]
    public void DeveCriarOfertaCorretamente()
    {
        Rota rota = new Rota("OrigemTeste", "DestinoTeste");
        DateTime dataIda = new DateTime(2024, 1, 1);
        DateTime dataVolta = new DateTime(2024, 1, 5);
        double preco = 100.0;
        Periodo periodo = new Periodo(dataIda, dataVolta);

        OfertaViagem oferta = new OfertaViagem(rota, periodo, preco);

        Assert.Equal(rota, oferta.Rota);
        Assert.Equal(dataIda, oferta.Periodo.DataInicial);
        Assert.Equal(dataVolta, oferta.Periodo.DataFinal);
        Assert.Equal(preco, oferta.Preco);

    }
}