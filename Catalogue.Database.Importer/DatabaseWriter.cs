using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using DatabaseHelper.SqlServer;
using DatabaseHelper.Common;


namespace Catalogue.Database.Importer;


internal class DatabaseWriter : IDataWriter<Category>
{
    private readonly DatabaseHelper.SqlServer.Database _database;

    public DatabaseWriter(string connectionString)
    {
        _database = new DatabaseHelper.SqlServer.Database(connectionString);
    }

    public void WriteData(IEnumerable<Category> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        _database.OpenConnection();
        _database.BeginTransaction();

        try
        {
            foreach (var category in data)
            {
                foreach (var product in category.Products)
                {
                    _database.ExecuteNonQuery
                     (
                        "ImportProductDatas_SP",
                         CommandType.StoredProcedure,
                         _database.CreateParameter("@CategoryName", category.Name),
                         _database.CreateParameter("@CategoryIsActive", category.IsActive),
                         _database.CreateParameter("@ProductName", product.Name),
                         _database.CreateParameter("@ProductCode", product.Code),
                         _database.CreateParameter("@ProductPrice", product.Price),
                         _database.CreateParameter("@ProductIsActive", product.IsActive)
                    );
                }
            }

            _database.CommitTransaction();
        }
        catch
        {
            _database.RollbackTransaction();
            throw;
        }
        finally
        {
            _database.CloseConnection();
        }
    }
}

