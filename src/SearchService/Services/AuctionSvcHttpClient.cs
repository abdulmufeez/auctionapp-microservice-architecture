using MongoDB.Entities;
using SearchService.Models;

namespace SearchService;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        // find of type item and return in string by using this <Item, string>()
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(s => s.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        return await _client.GetFromJsonAsync<List<Item>>(
            $"{_config["AuctionServiceUrl"]}/api/auctions?date={lastUpdated}");
    }
}
