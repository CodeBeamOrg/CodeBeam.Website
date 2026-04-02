using CodeBeam.Website.Client.Models;
using CodeBeam.Website.Components;
using CodeBeam.Website.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using MudExtensions.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddMudServices();
builder.Services.AddMudExtensions();

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<StatsService>();

//builder.Services.AddScoped(sp =>
//{
//    var nav = sp.GetRequiredService<NavigationManager>();
//    return new HttpClient
//    {
//        BaseAddress = new Uri(nav.BaseUri)
//    };
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapGet("/api/github/all", async ([FromServices] StatsService service) =>
{
    async Task<RepoStats> Get(string repo, string? package)
    {
        try
        {
            var repoData = await service.GetRepoAsync("CodeBeamOrg", repo);
            var contributors = await service.GetContributorsCountAsync("CodeBeamOrg", repo);
            
            long downloads = 0;
            if (package != null)
            {
                downloads = await service.GetTotalDownloadsAsync(package);
            }

            if (repoData is null)
                return new RepoStats();

            return new RepoStats
            {
                Stars = repoData.stargazers_count,
                Forks = repoData.forks_count,
                OpenIssues = repoData.open_issues_count,
                Contributors = contributors,
                Downloads = downloads
            };
        }
        catch
        {
            //Console.WriteLine(ex.ToString());
            return new RepoStats();
        }
        
    }

    var ultimateAuthTask = Get("UltimateAuth", null);
    var mudTask = Get("CodeBeam.MudBlazor.Extensions", "CodeBeam.MudBlazor.Extensions");
    var bcssTask = Get("BCSS", "CodeBeam.BCSS");

    await Task.WhenAll(ultimateAuthTask, mudTask, bcssTask);

    return Results.Ok(new AllRepoStatsDto
    {
        UltimateAuth = ultimateAuthTask.Result,
        MudExtensions = mudTask.Result,
        Bcss = bcssTask.Result
    });
});

//app.MapGet("/api/github/ultimateauth", async ([FromServices] GitHubService service) =>
//{
//    var repo = await service.GetRepoAsync("CodeBeamOrg", "UltimateAuth");
//    var contributors = await service.GetContributorsCountAsync("CodeBeamOrg", "UltimateAuth");

//    if (repo is null)
//        return Results.Problem("GitHub fetch failed");

//    return Results.Ok(new GitHubStatsDto
//    {
//        Stars = repo.stargazers_count,
//        Forks = repo.forks_count,
//        OpenIssues = repo.open_issues_count,
//        Contributors = contributors
//    });
//});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CodeBeam.Website.Client._Imports).Assembly);

app.Run();
