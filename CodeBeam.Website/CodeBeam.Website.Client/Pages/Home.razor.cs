using CodeBeam.Website.Client.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace CodeBeam.Website.Client.Pages;

public partial class Home
{
    private AllRepoStatsDto? _repoStats;

    [Inject] HttpClient Http { get; set; } = null!;
    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            _repoStats = await Http.GetFromJsonAsync<AllRepoStatsDto>("/api/github/all");
        }
    }

    string Format(int n) =>
        n >= 1_000_000 ? $"{n / 1_000_000.0:0.#}M+" :
        n >= 1_000 ? $"{n / 1_000.0:0.#}k+" :
        n.ToString();

    string GetContributorLabel(int count)
    {
        return count <= 1
            ? "Maintained"
            : "Contributors";
    }
}
