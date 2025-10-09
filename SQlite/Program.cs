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
            bool existingdatabase = File.Exists("FoodDatabase.db");

            if (existingdatabase == false)
            {
                CreateAndSeed();
            }

            Query();
        }

        static void CreateAndSeed()
        {
            using var connection = new SqliteConnection("Data Source=FoodDatabase.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE foods (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    amount INTEGER NOT NULL
                );

                INSERT INTO foods
                VALUES (1, 'Bread', 3),
                       (2, 'Milk', 5),
                       (3, 'Eggs', 15);
            """;
            command.ExecuteNonQuery();
        }

        static void Query()
        {
            var command = connection.CreateCommand(); 
            using var connection = new SqliteConnection("Data Source=FoodDatabase.db");
            connection.Open();

            //Product
            Console.Write("Name of the product added: ");
            var name = Console.ReadLine();

            command.CommandText = "INSERT INTO foods (name) VALUES ($name)";
            command.Parameters.AddWithValue("$name", name);

            command.ExecuteNonQuery();

            command.CommandText = "SELECT last_insert_rowid()";
            var newId = (long)command.ExecuteScalar()!;

            Console.WriteLine($"Your new product's ID is {newId}.");

            Console.Write("User ID: ");
            var line = Console.ReadLine();
            if (line is null)
            {
                return;
            }

            var id = int.Parse(line);

            #region snippet_HelloWorld
            using var connection = new SqliteConnection("Data Source=FoodDatabase.db");

            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT name
                FROM user
                WHERE id = $id
            """;
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var name = reader.GetString(0);

                Console.WriteLine($"Hello, {name}!");
            }
            #endregion
        }
    }
}