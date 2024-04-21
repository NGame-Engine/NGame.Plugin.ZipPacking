using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Hosting;
using NGame.Assets.Serialization;
using NGame.Assets.ZipPacking.Common;

namespace NGame.Assets.ZipPacking;



internal interface IZippedAssetUnpacker
{
	string GetAssetJson(AssetEntry assetEntry);
	byte[]? GetAssetData(AssetEntry assetEntry);
}



internal class ZippedAssetUnpacker(
	IHostEnvironment hostEnvironment
) : IZippedAssetUnpacker
{
	public string GetAssetJson(AssetEntry assetEntry)
	{
		var packagePath =
			Path.Combine(
				AssetConventions.AssetPackSubFolder,
				$"{assetEntry.PackFileName}{ZipPackagingConventions.PackFileEnding}"
			);

		using var assetPackStream =
			hostEnvironment
				.ContentRootFileProvider
				.GetFileInfo(packagePath)
				.CreateReadStream();

		using var zipArchive = new ZipArchive(assetPackStream, ZipArchiveMode.Read);

		var jsonPath = assetEntry.FilePath;
		var zipEntry =
			zipArchive.GetEntry(jsonPath) ??
			throw new InvalidOperationException($"Did not find {jsonPath} in {packagePath}");

		using var zipStream = zipEntry.Open();
		using var reader = new StreamReader(zipStream, Encoding.UTF8);
		return reader.ReadToEnd();
	}


	public byte[]? GetAssetData(AssetEntry assetEntry)
	{
		var packagePath =
			Path.Combine(
				AssetConventions.AssetPackSubFolder,
				$"{assetEntry.PackFileName}{ZipPackagingConventions.PackFileEnding}"
			);

		using var assetPackStream =
			hostEnvironment
				.ContentRootFileProvider
				.GetFileInfo(packagePath)
				.CreateReadStream();

		using var zipArchive = new ZipArchive(assetPackStream, ZipArchiveMode.Read);

		var dataPath = assetEntry.DataFilePath;
		if (dataPath == null) return null;

		var zipEntry =
			zipArchive.GetEntry(dataPath) ??
			throw new InvalidOperationException($"Did not find {dataPath} in {packagePath}");

		using var stream = zipEntry.Open();
		using var memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}
}
