using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Linq;

namespace BrightWayAssessment_Fanfan
{
    class Program
    {
        static string jsonURL = "https://www.brightway.com/CodeTests/pizzas.json";
        static int toppings { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! Starting process to get pizza toppings");

            // Call method to retriece the pizza JSON
            var pizzaJSON = GetPizzaJSON(jsonURL);
            Console.WriteLine("Pizza Toppings JSON received! Time to deserialize the object.");

            // Deserialize the pizzaJSON variable into a list
            var deserializedPizzaJSONList = JsonConvert.DeserializeObject<List<Toppings>>(pizzaJSON);
            Console.WriteLine("Pizza JSON Deserialized. Time to sort!");

            // Group the topping configurations with a count of each
            // Start by ordering each toppings array in the list
            // to get a clearer idea for grouping.
            foreach (var toppingConfiguration in deserializedPizzaJSONList)
            {
                Array.Sort(toppingConfiguration.toppings);
            }

            Console.WriteLine("Topping configurations have been sorted. Time to join!");

            // Join the individual string arrays into a single string to
            // make grouping simple
            var joinedList = new List<string>();

            foreach (var toppingConfiguration in deserializedPizzaJSONList)
            {
                var joinedConfig = String.Join("|", toppingConfiguration.toppings);
                joinedList.Add(joinedConfig);
            }

            Console.WriteLine("Topping configurations have been joined. Time to group and count!");

            // Now that our string arrays have all been sorted we can group them
            // Also want a count for each grouping
            var groupedList = joinedList.GroupBy(a => a)
                .Select(b => new
                {
                    config = b.Key,
                    count = b.Count()
                })
                .ToList();

            Console.WriteLine("Topping configurations have been grouped and counted. Time to order the list!");

            // Order the list from most ordered to least ordered
            var orderedList = groupedList.OrderByDescending(a => a.count).ToList();

            Console.WriteLine("Topping configurations have been ordered. Time to present the top 20 configurations:");

            // Display the top 20 configurations with a number of how many times they've been ordered
            for (int i = 0; i < 20; i++)
            {
                var pizzaItem = orderedList.ElementAt(i);
                Console.WriteLine($"{i + 1}. {String.Join(", ", pizzaItem.config.Split("|"))}; ordered {pizzaItem.count} times.");
            }

            Console.WriteLine("\nCheers!");
        }

        public static string GetPizzaJSON(string url)
        {
            var jsonReceived = "";

            using (WebClient wc = new WebClient())
            {
                try
                {
                    jsonReceived = wc.DownloadString(url);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Exception occured when retrieving the JSON data. Exception is: '{e.Message}'.");
                }
            }

            if (String.IsNullOrEmpty(jsonReceived))
            {
                Console.WriteLine("Error - our variable is empty/null");
                Environment.Exit(-1);
            }

            return jsonReceived;
        }
    }

    public class Toppings
    {
        public string[] toppings { get; set; }
    }
}
