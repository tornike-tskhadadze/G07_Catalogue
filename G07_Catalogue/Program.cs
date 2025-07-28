using Catalogue.Data.Reader;
using Catalogue.Database.Importer;
using Catalogue.Shared.Interfaces;
using Catalogue.Shared.Models;

namespace G07_Catalogue
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = args.Length > 0
                ? args[0]
                : "Data Source=WINDOWS-E8E5HO4\\SQLSERVER;Initial Catalog=G07_Catalog;Integrated Security=True;TrustServerCertificate=True";

            Console.WriteLine("Starting Catalogue Importer...");
            //Console.Write("Enter the path to the CSV file: ");
            string filePath = @"C:\Users\User\Downloads\Products.csv";

            IDataReader<Category> reader = CsvFileReaderFactory.Create(filePath);

            //reader.GetData();
            //foreach (var category in reader.GetData())
            //{
            //    Console.WriteLine($"Category: {category.Name} | Active: {category.IsActive}");

            //    foreach (var product in category.Products)
            //    {
            //        Console.WriteLine($"    Product: {product.Name} | Code: {product.Code} | Price: {product.Price} | Active: {product.IsActive}");
            //    }

            //    Console.WriteLine(); // spacing
            //}

            IDataWriter<Category> writer = DatabaseWriterFactory.Create(connectionString);
           
            try
            {
                IEnumerable<Category> categories = reader.GetData();
                writer.WriteData(categories);
                Console.WriteLine("Data import completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the import: {ex.Message}");
            }
        }
    }
}
