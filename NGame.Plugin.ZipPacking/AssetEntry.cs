namespace NGame.Plugin.ZipPacking;



internal class AssetEntry(
	string packFileName,
	string filePath,
	string? dataFilePath
)
{
	public string PackFileName { get; } = packFileName;
	public string FilePath { get; } = filePath;
	public string? DataFilePath { get; } = dataFilePath;
}
