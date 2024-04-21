using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Hosting;
using NGame.Assets.Serialization;
using NGame.Plugin.ZipPacking.Common;
using TableOfContents = NGame.Plugin.ZipPacking.Common.TableOfContents;

namespace NGame.Assets.ZipPacking;



internal class ZippedStoredAssetReader(
	IHostEnvironment hostEnvironment,
	IEnumerable<JsonConverter> jsonConverters,
	IZippedAssetUnpacker zippedAssetUnpacker
) : IStoredAssetReader
{
	private Dictionary<Guid, AssetEntry>? _assetEntries;


	public string GetAssetJson(Guid assetId)
	{
		_assetEntries ??= ReadTableOfContents();
		var assetEntry = _assetEntries[assetId];
		return zippedAssetUnpacker.GetAssetJson(assetEntry);
	}


	public byte[]? GetAssetData(Guid assetId)
	{
		_assetEntries ??= ReadTableOfContents();
		var assetEntry = _assetEntries[assetId];
		return zippedAssetUnpacker.GetAssetData(assetEntry);
	}


	private Dictionary<Guid, AssetEntry> ReadTableOfContents()
	{
		var jsonSerializerOptions = new JsonSerializerOptions();
		foreach (var jsonConverter in jsonConverters)
		{
			jsonSerializerOptions.Converters.Add(jsonConverter);
		}

		var filePath = Path.Combine(
			AssetConventions.AssetPackSubFolder,
			ZipPackagingConventions.TableOfContentsFileName
		);

		using var tocFileStream =
			hostEnvironment
				.ContentRootFileProvider
				.GetFileInfo(filePath)
				.CreateReadStream();

		var tableOfContents = JsonSerializer.Deserialize<TableOfContents>(tocFileStream, jsonSerializerOptions)!;


		var result = new Dictionary<Guid, AssetEntry>();

		foreach (var jsonPackage in tableOfContents.Packages)
		{
			foreach (var jsonAsset in jsonPackage.Assets)
			{
				result.Add(
					jsonAsset.Id,
					new AssetEntry(
						jsonPackage.Name,
						jsonAsset.FilePath,
						jsonAsset.DataFile
					)
				);
			}
		}

		return result;
	}
}
