using System;
using Bridge;
using Bridge.Html5;
using Newtonsoft.Json;

namespace Demo
{
    public class Program
    {
        public static void Main()
        {
            Product product = new Product();

            product.Name = "Apple";
            product.ExpiryDate = new DateTime(2008, 12, 28);
            product.Price = 3.99;
            product.Sizes = new string[] { "Small", "Medium", "Large" };

            string output = JsonConvert.SerializeObject(product, Formatting.Indented);

            // Write the json string
            Console.WriteLine(output);

            // Deserialize the json back into a real Product
            Product deserializedProduct = JsonConvert.DeserializeObject<Product>(output);

            // Write the rehydrated values
            var msg = $"An {deserializedProduct.Name} for ${deserializedProduct.Price}";

            Console.WriteLine(msg);
        }
    }

    [ObjectLiteral]
    public class Product
    {
        public string Name { get; set; }
        
        public DateTime ExpiryDate { get; set; }

        public double Price { get; set; }

        public string[] Sizes { get; set; }
    }
}