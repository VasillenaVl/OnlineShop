using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OnlineShop
{
    class Program
    {
        private static volatile Dictionary<string, int> productsStore = new Dictionary<string, int>()
        {
            { "Eggs", 25},
            { "Milk", 20},
            { "Bread", 50},
            { "Sugar", 10},
            { "Salt", 12},
            { "Fish", 15},
            { "Flour", 25},
            { "Biscuits", 40},
            { "Water 2.5L", 60},
            { "Water 10L", 30}

        };

        private static volatile List<string> productsFromSupplier = new List<string>()
        {
            "Eggs", "Milk", "Bread", "Sugar", "Salt", "Fish", "Flour", "Biscuits", "Water 2.5L", "Water 10L",
            "Chicken Meat", "Nuts", "Spaghetti", "Pizza", "Juice", "Napkins", "Oranges", "Melon"
        };

        private static object _lockStore = new object();

        static void Main(string[] args)
        {

            Thread createBuyers = new Thread(CreateBuyers);
            createBuyers.Start();

            Thread createSuppliers = new Thread(CreateSuppliers);
            createSuppliers.Start();

            string input = String.Empty;
            while (input != "m")
            {
                input = Console.ReadLine();
                if (input == "s")
                {
                    var localCopy = productsStore;
                    foreach (var prd in localCopy)
                        Console.WriteLine($"Product:{prd.Key}, Quantity:{prd.Value}");
                }
            }
        }

        static void CreateBuyers()
        {
            int buyers = 20;

            for (int i = 0; i < buyers; i++)
            {
                Thread newBuyer = new Thread(CreateBuyer);
                newBuyer.Start();
                Thread.Sleep(2000);
            }
        }

        static void CreateBuyer()
        {
            Dictionary<string, int> store = productsStore;
            Random randomProduct = new Random();
            int product = randomProduct.Next(0, store.Count - 1);
            Random randomQuantity = new Random();
            int quantity = randomQuantity.Next(1, 3);
            Buy(store.ElementAt(product).Key, quantity);
        }

        static void CreateSuppliers()
        {
            int suppliers = 5;

            for (int i = 0; i < suppliers; i++)
            {
                Thread newSupplier = new Thread(CreateSupplier);
                newSupplier.Start();
                Thread.Sleep(3000);
            }
        }

        static void CreateSupplier()
        {
            Random randomSupplyProduct = new Random();
            int product = randomSupplyProduct.Next(0, productsFromSupplier.Count - 1);
            Random randomSupplyQuantity = new Random();
            int quantity = randomSupplyQuantity.Next(5, 20);
            Supply(productsFromSupplier[product], quantity);
        }

        static void Buy(string product, int quantity)
        {
            lock (_lockStore)
            {
                var store = productsStore;
                if (store.ContainsKey(product))
                {
                    if (store[product] >= quantity)
                        productsStore[product] -= quantity;
                    else
                        Console.WriteLine("Not enough quantity.");
                }
                else
                    Console.WriteLine("No such product.");
            }
        }

        static void Supply(string product, int quantity)
        {
            lock (_lockStore)
            {
                var store = productsStore;
                if (store.ContainsKey(product))
                    SupplyQuantity(product, quantity);
                else
                    SupplyNewProduct(product, quantity);
            }
        }

        static void SupplyQuantity(string product, int quantity)
        {
            productsStore[product] += quantity;
        }

        static void SupplyNewProduct(string product, int quantity)
        {
            productsStore.Add(product, quantity);
        }
    }
}
