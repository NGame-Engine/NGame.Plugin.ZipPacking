using System.IO.Compression;
using System.Text;
using System.Text.Json;
using NGame.Assets.UsageFinder.TaskItems;
using NGame.Plugin.ZipPacking.Common;
using Singulink.IO;

namespace NGame.Plugin.ZipPacking.Packer.FileWriters;



public interface IAssetPackFactory
{
	string Create(
		string packName,
		IEnumerable<UsedAsset> assetFileSpecifications,
		IAbsoluteDirectoryPath outputPath,
		bool minifyJson,
		CompressionLevel compressionLevel
	);
}



public class AssetPackFactory : IAssetPackFactory
{
	public string Create(
		string packName,
		IEnumerable<UsedAsset> usedAssets,
		IAbsoluteDirectoryPath outputPath,
		bool minifyJson,
		CompressionLevel compressionLevel
	)
	{
		var packFileName = $"{packName}{ZipPackagingConventions.PackFileEnding}";
		var absoluteFilePath = outputPath.CombineFile(packFileName);
		var zipFileName = absoluteFilePath.PathDisplay.ToLowerInvariant();

		using var fileStream = File.Open(zipFileName, FileMode.Create);
		using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Create);

		foreach (var usedAsset in usedAssets)
		{
			WriteFile(usedAsset, usedAsset.JsonFilePath, zipArchive, minifyJson, compressionLevel);
			if (usedAsset.DataFilePath == null) continue;

			WriteFile(usedAsset, usedAsset.DataFilePath, zipArchive, false, compressionLevel);
		}

		return packFileName;
	}


	private static void WriteFile(
		UsedAsset usedAsset,
		IRelativeFilePath filePath,
		ZipArchive zipArchive,
		bool minifyJson,
		CompressionLevel compressionLevel
	)
	{
		var sourcePath =
			DirectoryPath
				.ParseAbsolute(usedAsset.ProjectDirectory.PathDisplay)
				.CombineFile(filePath.PathDisplay);

		using var sourceStream =
			minifyJson
				? OpenMinifiedJsonStream(sourcePath)
				: File.OpenRead(sourcePath.PathDisplay);

		var targetPath = filePath.GetNormalizedZipPath();
		var zipArchiveEntry = zipArchive.CreateEntry(targetPath, compressionLevel);
		using var zipStream = zipArchiveEntry.Open();
		sourceStream.CopyTo(zipStream);
	}


	private static Stream OpenMinifiedJsonStream(IAbsoluteFilePath absoluteFilePath)
	{
		var absolutePath = absoluteFilePath.PathDisplay;
		var allText = File.ReadAllText(absolutePath);
		var minified = JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonDocument>(allText));
		var bytes = Encoding.UTF8.GetBytes(minified);
		return new MemoryStream(bytes);
	}
}
