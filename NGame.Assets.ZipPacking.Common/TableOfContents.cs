namespace NGame.Assets.ZipPacking.Common;



public class TableOfContents
{
	public List<JsonPackage> Packages { get; init; } = new();
}



public class JsonPackage
{
	public string Name { get; init; } = null!;
	public List<JsonAsset> Assets { get; init; } = new();
}



public class JsonAsset
{
	public Guid Id { get; init; }
	public string FilePath { get; init; } = null!;
	public string? DataFile { get; init; }
}
