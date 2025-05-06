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
        var horasManha = TimeSpan.Zero;
        var horasTarde = TimeSpan.Zero;

        if (HoraEntradaManha.HasValue && HoraSaidaManha.HasValue && !HoraEntradaTarde.HasValue && !HoraSaidaTarde.HasValue)
        {
            horasManha = HoraSaidaManha.HasValue ? HoraSaidaManha.Value - HoraEntradaManha.Value : TimeSpan.Zero;
            return horasManha;
        }

        if (HoraEntradaTarde.HasValue && HoraSaidaTarde.HasValue && !HoraEntradaManha.HasValue && !HoraSaidaManha.HasValue)
        {
            horasTarde = HoraSaidaTarde.HasValue ? HoraSaidaTarde.Value - HoraEntradaTarde.Value : TimeSpan.Zero;
            return horasTarde;
        }

        horasManha = HoraSaidaManha.HasValue ? HoraSaidaManha.Value - HoraEntradaManha.Value : TimeSpan.Zero;
        horasTarde = HoraSaidaTarde.HasValue ? HoraSaidaTarde.Value - HoraEntradaTarde.Value : TimeSpan.Zero;

        return horasManha + horasTarde;
    }
}