using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Catalogue.Database.Importer;

internal class DatabaseWriter : IDataWriter<Category>
{
    private readonly string _connectionString;

    public DatabaseWriter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void WriteData(IEnumerable<Category> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using SqlConnection connection = new(_connectionString);
        connection.Open();

        foreach (var category in data)
        {
            foreach (var product in category.Products)
            {
                using SqlCommand cmd = new("ImportProductData_SP", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@CategoryName", category.Name);
                cmd.Parameters.AddWithValue("@CategoryIsActive", category.IsActive);
                cmd.Parameters.AddWithValue("@ProductName", product.Name);
                cmd.Parameters.AddWithValue("@ProductCode", product.Code);
                cmd.Parameters.AddWithValue("@ProductPrice", product.Price);
                cmd.Parameters.AddWithValue("@ProductIsActive", product.IsActive);

                var returnValue = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                returnValue.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();

                int result = (int)returnValue.Value;
                if (result != 0)
                {
                    throw new Exception($"Stored procedure failed with return code {result} for product '{product.Name}'");
                }
            }
        }
    }
}