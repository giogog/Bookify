using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using Contracts;

namespace Infrastructure.Context;

public class DapperDbConnection: IDapperDbConnection
{
    private readonly IConfiguration _config;

    public DapperDbConnection(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IEnumerable<T>> LoadDataProcedure<T, U>(string Action, U parameters, string ConnectionKey = "DefaultConnection")
    {
        using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(ConnectionKey));

        return await dbConnection.QueryAsync<T>(Action, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<T>> LoadDataSqlQuery<T, U>(string Action, U parameters, string ConnectionKey = "DefaultConnection")
    {
        using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(ConnectionKey));

        return await dbConnection.QueryAsync<T>(Action, parameters, commandType: CommandType.Text);
    }


    public async Task SaveDataProcedure<T>(string Action, T parameters, string ConnectionKey = "DefaultConnection")
    {
        using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(ConnectionKey));

        await dbConnection.ExecuteAsync(Action, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task SaveDataSqlQuery<T>(string Action, T parameters, string ConnectionKey = "DefaultConnection")
    {
        using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(ConnectionKey));

        await dbConnection.ExecuteAsync(Action, parameters, commandType: CommandType.Text);
    }

    public async Task<bool> CheckIfExists<T>(string query, T parameters, string ConnectionKey = "DefaultConnection")
    {
        using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(ConnectionKey));
        var result = await dbConnection.ExecuteScalarAsync<int>(query, parameters, commandType: CommandType.Text);
        return result > 0;
    }

    public async Task<T?> LoadFirstOrDefault<T, U>(string query, U parameters, string ConnectionKey = "DefaultConnection")
    {
        using IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString(ConnectionKey));
        return await dbConnection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: CommandType.Text);
        
    }

}