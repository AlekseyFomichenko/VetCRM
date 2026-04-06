using Microsoft.EntityFrameworkCore;
using VetCRM.Modules.Clients.Infrastructure;

namespace VetCRM.Api.Services;

public sealed class SqlScriptsInitializer(
    ClientDbContext dbContext,
    ILogger<SqlScriptsInitializer> logger)
{
    private readonly ClientDbContext _dbContext = dbContext;
    private readonly ILogger<SqlScriptsInitializer> _logger = logger;

    public async Task RunAsync(CancellationToken ct)
    {
        string sqlDir = Path.Combine(AppContext.BaseDirectory, "sql");
        if (!Directory.Exists(sqlDir))
        {
            _logger.LogWarning("SQL scripts directory not found: {SqlDir}", sqlDir);
            return;
        }

        var scripts = new[]
        {
            new ScriptDescriptor("001_Init_Clients.sql", TableExistsAsync("Clients", ct)),
            new ScriptDescriptor("002_Expand_Clients_Table.sql", ClientsExpandedAsync(ct)),
            new ScriptDescriptor("003_Init_Pets.sql", TableExistsAsync("pets", ct)),
            new ScriptDescriptor("004_Init_Appointments.sql", TableExistsAsync("Appointments", ct)),
            new ScriptDescriptor("005_Init_MedicalRecords.sql", TableExistsAsync("MedicalRecords", ct)),
            new ScriptDescriptor("006_Init_Identity.sql", TableExistsAsync("Users", ct)),
            new ScriptDescriptor("007_Init_Notifications.sql", TableExistsAsync("ReminderLogs", ct)),
        };

        foreach (var script in scripts)
        {
            bool applied = await script.IsApplied;
            if (applied)
            {
                _logger.LogInformation("SQL script already applied: {Script}", script.FileName);
                continue;
            }

            string filePath = Path.Combine(sqlDir, script.FileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"SQL script not found: {filePath}", filePath);

            string sql = await File.ReadAllTextAsync(filePath, ct);
            if (string.IsNullOrWhiteSpace(sql))
                continue;

            _logger.LogInformation("Applying SQL script: {Script}", script.FileName);
            await _dbContext.Database.ExecuteSqlRawAsync(sql, ct);
        }
    }

    private async Task<bool> ClientsExpandedAsync(CancellationToken ct)
    {
        bool hasAddress = await ColumnExistsAsync("Clients", "Address", ct);
        bool hasCreatedAt = await ColumnExistsAsync("Clients", "CreatedAt", ct);
        bool hasEmail = await ColumnExistsAsync("Clients", "Email", ct);
        bool hasFullName = await ColumnExistsAsync("Clients", "FullName", ct);
        bool hasNotes = await ColumnExistsAsync("Clients", "Notes", ct);
        bool hasPhone = await ColumnExistsAsync("Clients", "Phone", ct);
        bool hasStatus = await ColumnExistsAsync("Clients", "Status", ct);
        bool hasPhoneIndex = await IndexExistsAsync("IX_Clients_Phone", ct);

        return hasAddress &&
               hasCreatedAt &&
               hasEmail &&
               hasFullName &&
               hasNotes &&
               hasPhone &&
               hasStatus &&
               hasPhoneIndex;
    }

    private async Task<bool> TableExistsAsync(string tableName, CancellationToken ct)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_name = @tableName
            )
            """;
        return await ExecuteExistsQueryAsync(sql, "@tableName", tableName, ct);
    }

    private async Task<bool> ColumnExistsAsync(string tableName, string columnName, CancellationToken ct)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM information_schema.columns
                WHERE table_schema = 'public'
                  AND table_name = @tableName
                  AND column_name = @columnName
            )
            """;

        var connection = _dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@tableName";
        tableParam.Value = tableName;
        command.Parameters.Add(tableParam);

        var columnParam = command.CreateParameter();
        columnParam.ParameterName = "@columnName";
        columnParam.Value = columnName;
        command.Parameters.Add(columnParam);

        object? result = await command.ExecuteScalarAsync(ct);
        return result is bool exists && exists;
    }

    private async Task<bool> IndexExistsAsync(string indexName, CancellationToken ct)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM pg_indexes
                WHERE schemaname = 'public'
                  AND indexname = @indexName
            )
            """;
        return await ExecuteExistsQueryAsync(sql, "@indexName", indexName, ct);
    }

    private async Task<bool> ExecuteExistsQueryAsync(string sql, string paramName, string paramValue, CancellationToken ct)
    {
        var connection = _dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var parameter = command.CreateParameter();
        parameter.ParameterName = paramName;
        parameter.Value = paramValue;
        command.Parameters.Add(parameter);

        object? result = await command.ExecuteScalarAsync(ct);
        return result is bool exists && exists;
    }

    private sealed record ScriptDescriptor(string FileName, Task<bool> IsApplied);
}
