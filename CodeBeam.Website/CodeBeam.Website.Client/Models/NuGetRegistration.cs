namespace CodeBeam.Website.Client.Models;

public class NuGetSearchResult
{
    public List<NuGetSearchPackage> data { get; set; } = [];
}

public class NuGetSearchPackage
{
    public string id { get; set; } = "";
    public long totalDownloads { get; set; }
}
