using CodeBeam.Website.Client.Models;
using CodeBeam.Website.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace CodeBeam.Website.Service;

public class StatsService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;

    public StatsService(HttpClient http, IConfiguration configuration, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("CodeBeamWebsite");

        var token = configuration["GitHub:Token"];

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<GitHubRepoDto?> GetRepoAsync(string owner, string repo)
    {
        var key = $"repo:{owner}:{repo}";

        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

            var url = $"https://api.github.com/repos/{owner}/{repo}";
            return await _http.GetFromJsonAsync<GitHubRepoDto>(url);
        });
    }

    public async Task<int> GetContributorsCountAsync(string owner, string repo)
    {
        var key = $"contributors:{owner}:{repo}";

        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            var url = $"https://api.github.com/repos/{owner}/{repo}/contributors?per_page=1&anon=true";
            var response = await _http.GetAsync(url);

            if (!response.Headers.TryGetValues("Link", out var values))
                return 1;

            var linkHeader = values.FirstOrDefault();
            var match = Regex.Match(linkHeader!, @"page=(\d+)>; rel=""last""");

            return match.Success ? int.Parse(match.Groups[1].Value) : 1;
        });
    }

    public async Task<long> GetTotalDownloadsAsync(string packageId)
    {
        var key = $"nuget:{packageId}";

        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            var url = $"https://api-v2v3search-0.nuget.org/query?q=packageid:{packageId}&prerelease=true";
            var result = await _http.GetFromJsonAsync<NuGetSearchResult>(url);

            return result?.data?.FirstOrDefault()?.totalDownloads ?? 0;
        });
    }
}
