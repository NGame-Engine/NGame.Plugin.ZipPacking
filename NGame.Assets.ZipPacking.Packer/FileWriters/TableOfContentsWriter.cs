using System.Text.Json;
using System.Text.Json.Serialization;
using NGame.Assets.UsageFinder.TaskItems;
using NGame.Assets.ZipPacking.Common;
using Singulink.IO;

namespace NGame.Assets.ZipPacking.Packer.FileWriters;



public interface ITableOfContentsWriter
{
	void Write(
		List<UsedAsset> usedAssets,
		IAbsoluteDirectoryPath targetFolder,
		bool minifyJson
	);
}



public class TableOfContentsWriter(
	IEnumerable<JsonConverter> jsonConverters
) : ITableOfContentsWriter
{
	public void Write(
		List<UsedAsset> usedAssets,
		IAbsoluteDirectoryPath targetFolder,
		bool minifyJson
	)
	{
		var tableOfContents = CreateTableOfContents(usedAssets);
		WriteTableOfContentsFile(targetFolder, tableOfContents, minifyJson);
	}


	private static TableOfContents CreateTableOfContents(List<UsedAsset> usedAssets) =>
		new()
		{
			Packages =
				usedAssets
					.GroupBy(x => x.Package)
					.Select(x => CreateJsonPackage(x.Key, x))
					.ToList()
		};


	private static JsonPackage CreateJsonPackage(string name, IEnumerable<UsedAsset> usedAssets) =>
		new()
		{
			Name = name,
			Assets =
				usedAssets
					.Select(CreateJsonAsset)
					.ToList()
		};


	private static JsonAsset CreateJsonAsset(UsedAsset usedAsset) =>
		new()
		{
			Id = usedAsset.AssetId,
			FilePath = usedAsset.JsonFilePath.GetNormalizedZipPath(),
			DataFile = usedAsset.DataFilePath?.GetNormalizedZipPath()
		};


	private void WriteTableOfContentsFile(
		IAbsoluteDirectoryPath targetFolder,
		TableOfContents tableOfContents,
		bool minifyJson
	)
	{
		var jsonSerializerOptions = new JsonSerializerOptions();
		foreach (var jsonConverter in jsonConverters)
		{
			jsonSerializerOptions.Converters.Add(jsonConverter);
		}

		jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
		jsonSerializerOptions.WriteIndented = minifyJson == false;

		var jsonString = JsonSerializer.Serialize(tableOfContents, jsonSerializerOptions);


		var path = targetFolder.CombineFile(ZipPackagingConventions.TableOfContentsFileName);
		File.WriteAllText(path.PathDisplay, jsonString);
	}
}
