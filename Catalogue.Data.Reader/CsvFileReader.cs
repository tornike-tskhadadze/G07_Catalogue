using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;
using System.Collections.Generic;
using System.Globalization;

namespace Catalogue.Data.Reader;

internal sealed class CsvFileReader : IDataReader<Category>
{
    private readonly string _filePath;

    public CsvFileReader(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
        _filePath = filePath;
    }

    public IEnumerable<Category> GetData()
    {
        Dictionary<string, Category> categories = new();

        using (StreamReader reader = new StreamReader(_filePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                string[] columns = line.Split('\t');
                if (columns.Length == 6)
                {
                    if ((columns[1] != "0" && columns[1] != "1") || (columns[5] != "0" && columns[5] != "1"))
                        throw new Exception("Invalid syntax in IsActive columns");

                    if (!categories.TryGetValue(columns[0], out var category))
                    {
                        category = new Category
                        {
                            Name = columns[0],
                            IsActive = columns[1] == "1",
                            Products = new List<Product>()
                        };
                        categories[columns[0]] = category;
                    }

                    category.Products.Add(new Product
                    {
                        Name = columns[2],
                        Code = columns[3],
                        Price = decimal.Parse(columns[4], CultureInfo.InvariantCulture),
                        IsActive = columns[5] == "1"
                    });
                }
            }
        }
                
        return categories.Values;
    }
}

