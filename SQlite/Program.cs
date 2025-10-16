using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace DatabaseFood
{
    class Program
    {
        static void Main()
        {
            //checking whether the database exists
            SqliteConnection.ClearAllPools();
            bool existingdatabase = File.Exists("Storage.db");

            if (existingdatabase == false)
            {
                CreateAndSeed();
            }

            Query();
        }

        static void CreateAndSeed()
        {
            using var connection = new SqliteConnection("Data Source=Storage.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE foods (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    amount INTEGER NOT NULL DEFAULT 0,
                    delivery INTEGER NOT NULL
                );

                INSERT INTO foods
                VALUES (1, 'Bread', 3, 0),
                       (2, 'Milk', 5, 0),
                       (3, 'Eggs', 15, 0);
            """;
            command.ExecuteNonQuery();
        }

        static void Query()
        {
            using var connection = new SqliteConnection("Data Source=Storage.db");
            var command = connection.CreateCommand();
            connection.Open();

            while (true)
            {
                Console.Write("Awaiting further input (Order, AddNew, Deliver, RemoveStorage, list, quit) ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "Order":
                        Console.Write("What product must be ordered? ");
                        var name = Console.ReadLine();
                        Console.Write("Amount of the product ordered: ");
                        int amount = int.Parse(Console.ReadLine()!);

                        command.CommandText = "UPDATE foods SET delivery = $delivery + delivery WHERE name = $name";
                        command.Parameters.AddWithValue("$name", name);
                        command.Parameters.AddWithValue("$delivery", amount);
                        
                        command.ExecuteNonQuery();
                        break;
                        
                    case "AddNew":
                        Console.Write("Name of the new product: ");
                        name = Console.ReadLine();
                        Console.Write("Amount of the ordered: ");
                        amount = int.Parse(Console.ReadLine()!);

                        command.CommandText = "INSERT INTO foods (name, amount, delivery) VALUES ($name, $amount, $delivery)";
                        command.Parameters.AddWithValue("$name", name);
                        command.Parameters.AddWithValue("$delivery", amount);
                        command.Parameters.AddWithValue("$amount", 0);

                        command.ExecuteNonQuery();
                        break;

                    case "Deliver":
                        Console.Write("What product was delivered? ");
                        name = Console.ReadLine();

                        command.CommandText = "UPDATE foods SET amount = amount + delivery, delivery = 0 WHERE name = $name";
                        command.Parameters.AddWithValue("$name", name);

                        command.ExecuteNonQuery();
                        break;

                    case "RemoveStorage":
                        Console.Write("Name of the product edited: ");
                        name = Console.ReadLine();
                        Console.Write("Amount of the said product removed: ");
                        amount = int.Parse(Console.ReadLine()!);

                        command.CommandText = "UPDATE foods SET amount = amount - $amount WHERE name = $name";
                        command.Parameters.AddWithValue("$name", name);
                        command.Parameters.AddWithValue("$amount", amount);

                        command.ExecuteNonQuery();
                        break;

                    case "list":
                        command = connection.CreateCommand();
                        command.CommandText = """
                            SELECT *
                            FROM foods
                            """;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine($"Stored {reader.GetString(2)} of {reader.GetValue(1)}, {reader.GetValue(3)} of the product is awaiting delivery");
                            }
                        }
                        break;

                    case "quit":
                        return;
                }
            }
        }
    }
}