using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpgsqlIssue1362
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder
            {
                Username = "user0",
                Password = "password",
                Database = "quotes",
                Host = "192.168.106.128",
                Port = 5432,
                SslMode = SslMode.Require,
                TrustServerCertificate = true,
                SearchPath = "dbo"
            };
            string connStr = builder.ToString();

            using (NpgsqlConnection connection = new NpgsqlConnection(connStr))
            {
                connection.Open();

                SetupDatabase(connection);
                WriteDataAsync(connection).Wait();
            }
        }

        public static void SetupDatabase(NpgsqlConnection connection)
        {
            using (NpgsqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
CREATE SCHEMA IF NOT EXISTS dbo ;

CREATE TABLE IF NOT EXISTS quotes (
    quote_id    serial PRIMARY KEY,
    author      VARCHAR(255),
    quote       TEXT
)";
                command.ExecuteNonQuery();
            }
        }

        public static async Task WriteDataAsync(NpgsqlConnection connection)
        {
            for (int i = 0; i < 5; ++i)
            {
                Console.Write("Attempt {0}: ", i + 1);

                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
INSERT INTO quotes (
    author,
    quote
) VALUES (
    'Jane Austen',
    'It is a truth universally acknowledged, that a single man in possession of a good fortune, must be in want of a wife.'
)";
                    int rowsInserted = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    if (rowsInserted == 1)
                    {
                        Console.WriteLine("Sucesss!");
                    }
                }
            }
        }
    }
}
