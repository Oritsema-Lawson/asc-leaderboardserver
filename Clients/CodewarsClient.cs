using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace APCServer.Clients;

public class CodewarsClient
{
    private readonly HttpClient _client;

    public CodewarsClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<string?>> GetCompletedKata(string username)
    {
        using HttpResponseMessage response = await _client.GetAsync($"users/{username}/code-challenges/completed?page=0");
        using var jsonResponse = await response.Content.ReadAsStreamAsync();

        var root = await JsonNode.ParseAsync(jsonResponse);

        return root?["data"]?.AsArray()
                .Select(x => x?["id"]?.GetValue<string>())
                .Where(x => x != null)
                .ToList() ?? new List<string?>();
    }
}