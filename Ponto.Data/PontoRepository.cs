using Dapper;
using Ponto.Domain;
using Npgsql;

namespace Ponto.Data;

public class PontoRepository
{
    private readonly string _connectionString;

    public PontoRepository()
    {
        _connectionString = DatabaseConfig.ConnectionString;
    }

    // Método existente (registro automático)
    public void RegistrarPonto(RegistroPonto registro)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            INSERT INTO RegistrosPonto 
            (UsuarioId, Data, HoraEntradaManha, HoraSaidaManha, HoraEntradaTarde, HoraSaidaTarde, Observacao)
            VALUES 
            (@UsuarioId, @Data, 
             CAST(@HoraEntradaManha AS time), 
             CAST(@HoraSaidaManha AS time), 
             CAST(@HoraEntradaTarde AS time), 
             CAST(@HoraSaidaTarde AS time), 
             @Observacao)
            ON CONFLICT (UsuarioId, Data) 
            DO UPDATE SET
                HoraEntradaManha = COALESCE(CAST(@HoraEntradaManha AS time), RegistrosPonto.HoraEntradaManha),
                HoraSaidaManha = COALESCE(CAST(@HoraSaidaManha AS time), RegistrosPonto.HoraSaidaManha),
                HoraEntradaTarde = COALESCE(CAST(@HoraEntradaTarde AS time), RegistrosPonto.HoraEntradaTarde),
                HoraSaidaTarde = COALESCE(CAST(@HoraSaidaTarde AS time), RegistrosPonto.HoraSaidaTarde),
                Observacao = @Observacao
        ";

        connection.Execute(sql, registro);
    }

    // Novo método para edição de registros
    public bool AtualizarRegistroPonto(RegistroPonto registro)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            UPDATE RegistrosPonto 
            SET 
                HoraEntradaManha = CAST(@HoraEntradaManha AS time),
                HoraSaidaManha = CAST(@HoraSaidaManha AS time),
                HoraEntradaTarde = CAST(@HoraEntradaTarde AS time),
                HoraSaidaTarde = CAST(@HoraSaidaTarde AS time),
                Observacao = @Observacao
            WHERE 
                UsuarioId = @UsuarioId AND 
                Data = @Data";

        var linhasAfetadas = connection.Execute(sql, registro);
        return linhasAfetadas > 0;
    }

    // Novo método para adição manual de registros
    public void AdicionarRegistroManual(RegistroPonto registro)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            INSERT INTO RegistrosPonto 
            (UsuarioId, Data, HoraEntradaManha, HoraSaidaManha, HoraEntradaTarde, HoraSaidaTarde, Observacao)
            VALUES 
            (@UsuarioId, @Data, 
             CAST(@HoraEntradaManha AS time), 
             CAST(@HoraSaidaManha AS time), 
             CAST(@HoraEntradaTarde AS time), 
             CAST(@HoraSaidaTarde AS time), 
             @Observacao)";

        connection.Execute(sql, registro);
    }

    // Métodos existentes
    public IEnumerable<RegistroPonto> ObterRegistrosPorPeriodo(int usuarioId, DateTime inicio, DateTime fim)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            SELECT * FROM RegistrosPonto 
            WHERE UsuarioId = @UsuarioId 
            AND Data BETWEEN @Inicio AND @Fim
            ORDER BY Data
        ";

        return connection.Query<RegistroPonto>(sql, new { UsuarioId = usuarioId, Inicio = inicio.Date, Fim = fim.Date });
    }

    public Usuario ObterUsuarioPorCodigo(string codigo)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT * FROM Usuarios WHERE Codigo = @Codigo";
        return connection.QueryFirstOrDefault<Usuario>(sql, new { Codigo = codigo });
    }

    // Novo método para obter registro específico
    public RegistroPonto ObterRegistroPorData(int usuarioId, DateTime data)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT * FROM RegistrosPonto WHERE UsuarioId = @UsuarioId AND Data = @Data";
        return connection.QueryFirstOrDefault<RegistroPonto>(sql, new
        {
            UsuarioId = usuarioId,
            Data = data.Date
        });
    }
}