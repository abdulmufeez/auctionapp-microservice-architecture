using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        // this line is for initializing db with db name
        await DB.InitAsync("search_db",
            MongoClientSettings
                .FromConnectionString(
                    app.Configuration.GetConnectionString("MongoDbConnection")
                )
            );

        // make indexing for certains keys in entity
        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

        if (await DB.CountAsync<Item>() == 0)
        {
            var itemData = await File.ReadAllTextAsync("Data/auctions.json");

            if (string.IsNullOrEmpty(itemData))
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var items = JsonSerializer.Deserialize<List<Item>>(itemData, opts);

                await DB.SaveAsync(items);
            }
        }
        else
        {
            Console.WriteLine("Db initilzed and no need to seed data...");
        }
    }
}
