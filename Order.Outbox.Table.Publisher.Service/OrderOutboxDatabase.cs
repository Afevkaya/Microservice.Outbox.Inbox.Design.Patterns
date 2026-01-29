using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Order.Outbox.Table.Publisher.Service.Entities;

namespace Order.Outbox.Table.Publisher.Service;

public class OrderOutboxDatabase
{
    private readonly string _connectionString;
    public OrderOutboxDatabase(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlConnection");
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        return await connection.QueryAsync<T>(sql);
    }
    
    public async Task ExecuteAsync(string sql)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await connection.ExecuteAsync(sql);
    }
}