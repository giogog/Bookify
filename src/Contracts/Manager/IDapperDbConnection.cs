namespace Contracts;

public interface IDapperDbConnection
{
    Task<IEnumerable<T>> LoadDataProcedure<T, U>(string Action, U parameters, string ConnectionKey = "DefaultConnection");
    Task<IEnumerable<T>> LoadDataSqlQuery<T, U>(string Action, U parameters, string ConnectionKey = "DefaultConnection");
    Task<bool> CheckIfExists<T>(string query, T parameters, string ConnectionKey = "DefaultConnection");
    Task<T?> LoadFirstOrDefault<T, U>(string query, U parameters, string ConnectionKey = "DefaultConnection");
    Task SaveDataProcedure<T>(string Action, T parameters, string ConnectionKey = "DefaultConnection");
    Task SaveDataSqlQuery<T>(string Action, T parameters, string ConnectionKey = "DefaultConnection");

}
