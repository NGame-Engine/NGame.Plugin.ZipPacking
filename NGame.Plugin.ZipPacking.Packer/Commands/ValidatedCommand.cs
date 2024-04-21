using System.IO.Compression;
using NGame.Assets.UsageFinder.TaskItems;
using Singulink.IO;

namespace NGame.Plugin.ZipPacking.Packer.Commands;



public class ValidatedCommand(
	List<UsedAsset> assets,
	bool minifyJson,
	CompressionLevel compressionLevel,
	IAbsoluteDirectoryPath output
)
{
	public List<UsedAsset> Assets { get; } = assets;
	public bool MinifyJson { get; } = minifyJson;
	public CompressionLevel CompressionLevel { get; } = compressionLevel;
	public IAbsoluteDirectoryPath Output { get; } = output;
}
