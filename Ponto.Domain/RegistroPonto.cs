namespace Ponto.Domain;

public class RegistroPonto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Data { get; set; }
    public TimeSpan? HoraEntradaManha { get; set; }
    public TimeSpan? HoraSaidaManha { get; set; }
    public TimeSpan? HoraEntradaTarde { get; set; }
    public TimeSpan? HoraSaidaTarde { get; set; }
    public string Observacao { get; set; }

    public TimeSpan? CalcularHorasTrabalhadas()
    {
        if (!HoraEntradaManha.HasValue || !HoraSaidaTarde.HasValue)
            return null;

        var horasManha = HoraSaidaManha.HasValue ? HoraSaidaManha.Value - HoraEntradaManha.Value : TimeSpan.Zero;
        var horasTarde = HoraSaidaTarde.HasValue ? HoraSaidaTarde.Value - HoraEntradaTarde.Value : TimeSpan.Zero;

        return horasManha + horasTarde;
    }
}