using Dapper;

namespace Ponto.Data;

public static class DatabaseConfig
{
    public static string ConnectionString { get; set; } =
        "Host=localhost;Port=5432;Database=ponto_db;Username=postgres;Password=prodan46;";

    public static void InitializeDatabase()
    {
        using var connection = new Npgsql.NpgsqlConnection(ConnectionString);
        connection.Open();

        // Criação das tabelas se não existirem
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Usuarios (
                Id SERIAL PRIMARY KEY,
                Nome VARCHAR(100) NOT NULL,
                Codigo VARCHAR(20) NOT NULL UNIQUE,
                Departamento VARCHAR(100),
                Cargo VARCHAR(100),
                HorarioPadrao VARCHAR(100)
            );
            
            CREATE TABLE IF NOT EXISTS RegistrosPonto (
                Id SERIAL PRIMARY KEY,
                UsuarioId INTEGER REFERENCES Usuarios(Id),
                Data DATE NOT NULL,
                HoraEntradaManha TIME,
                HoraSaidaManha TIME,
                HoraEntradaTarde TIME,
                HoraSaidaTarde TIME,
                Observacao TEXT
            );
        ");
    }
}
