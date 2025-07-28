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


//using Catalogue.Shared.Interfaces;
//using Catalogue.Shared.Models;

//namespace Catalogue.Data.Reader;

//internal sealed class CsvFileReader : IDataReader<Category>
//{
//    private readonly string _filePath;

//    internal CsvFileReader(string filePath)
//    {
//        ArgumentNullException.ThrowIfNull(filePath);
//        if (!File.Exists(filePath)) throw new FileNotFoundException($"The file '{filePath}' does not exist.", filePath);
//        _filePath = filePath;
//    }

//    public IEnumerable<Category> GetData()
//    {
//        // Maybe dictionary will be better...
//        var categories = new HashSet<Category>();
//        using var reader = new StreamReader(_filePath);

//        string? line;

//        while ((line = reader.ReadLine()) != null)
//        {
//            string[] tokens = line.Split('\t');
//            string categoryName = tokens[0];
//            string productCode = tokens[3];

//            var category = categories.FirstOrDefault(c => c.Name == categoryName)
//                ?? CreateCategoryFromTokens(tokens);
//            categories.Add(category);

//            var product = category.Products.FirstOrDefault(c => c.Code == productCode);
//            if (product == null)
//            {
//                product = new Product
//                {
//                    Name = productName,
//                    Code = productCode,
//                    Price = productPrice,
//                    IsActive = productIsActive
//                };
//                category.Products.Add(product);
//            }
//            else
//            {
//                product.Name = productName;
//                product.Price = productPrice;
//                product.IsActive = productIsActive;
//            }
//        }

//        return categories;
//    }

//    private static Category CreateCategoryFromTokens(string[] tokens)
//    {
//        return new Category
//        {
//            Name = tokens[0],
//            IsActive = tokens[1] == "1",
//            Products = new List<Product>()
//        };
//    }

//    private static Product CreateProductFromTokens(string[] tokens)
//    {
//        return new Product
//        {
//            Name = tokens[2],
//            Code = tokens[3],
//            Price = decimal.Parse(tokens[4]),
//            IsActive = tokens[5] == "1"
//        };
//    }
//}






//public IEnumerable<Category> GetData()
//{
//    List<Category> categories = new();

//    using (StreamReader reader = new StreamReader(_filePath))
//    {
//        while (!reader.EndOfStream)
//        {
//            string line = reader.ReadLine();
//            string[] columns = line.Split('\t');

//            if (columns.Length == 6)
//            {
//                string categoryName = columns[0];
//                bool categoryIsActive = columns[1] == "1" ? true : false;
//                string productName = columns[2];
//                string productCode = columns[3];
//                decimal productPrice = decimal.Parse(columns[4]);
//                bool productIsActive = columns[5] == "1" ? true : false;

//                Category? distinctCategory = null;
//                foreach (var c in categories)
//                {
//                    if (c.Name == categoryName)
//                    {
//                        distinctCategory = c;
//                        break;
//                    }
//                }

//                if (distinctCategory == null)
//                {
//                    distinctCategory = new Category
//                    {
//                        Name = categoryName,
//                        IsActive = categoryIsActive,
//                        Products = new List<Product>()
//                    };
//                    categories.Add(distinctCategory);
//                }

//                distinctCategory.Products.Add(new Product
//                {
//                    Name = productName,
//                    Code = productCode,
//                    Price = productPrice,
//                    IsActive = categoryIsActive
//                });
//            }

//        }
//    }
//    return categories;
//}