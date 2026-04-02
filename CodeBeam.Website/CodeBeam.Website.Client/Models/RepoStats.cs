namespace CodeBeam.Website.Client.Models;

public sealed class RepoStats
{
    public int Stars { get; set; }
    public int Forks { get; set; }
    public int OpenIssues { get; set; }
    public int Contributors { get; set; }
    public long Downloads { get; set; }
}

public sealed class AllRepoStatsDto
{
    public RepoStats UltimateAuth { get; set; } = new();
    public RepoStats MudExtensions { get; set; } = new();
    public RepoStats Bcss { get; set; } = new();
}
