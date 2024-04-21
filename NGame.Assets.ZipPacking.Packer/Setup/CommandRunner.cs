using Microsoft.Extensions.Logging;
using NGame.Assets.UsageFinder.TaskItems;
using NGame.Assets.ZipPacking.Packer.Commands;
using NGame.Assets.ZipPacking.Packer.FileWriters;

namespace NGame.Assets.ZipPacking.Packer.Setup;



public class TaskParameters(
	List<UsedAsset> assets,
	bool minifyJson,
	int compressionLevel,
	string outputDirectory
)
{
	public List<UsedAsset> Assets { get; } = assets;
	public bool MinifyJson { get; } = minifyJson;
	public int CompressionLevel { get; } = compressionLevel;
	public string OutputDirectory { get; } = outputDirectory;
}



public interface ICommandRunner
{
	void Run(TaskParameters taskParameters);
}



internal class CommandRunner(
	ILogger<CommandRunner> logger,
	IParameterValidator parameterValidator,
	IAssetPackFactory assetPackFactory,
	ITableOfContentsWriter tableOfContentsWriter
)
	: ICommandRunner
{
	public void Run(TaskParameters taskParameters)
	{
		logger.LogInformation("Packing assets...");

		var validatedCommand = parameterValidator.ValidateCommand(taskParameters);

		
		var assets = validatedCommand.Assets;
		var outputPath = validatedCommand.Output;

		var minifyJson = validatedCommand.MinifyJson;
		var compressionLevel = validatedCommand.CompressionLevel;
		
		
		outputPath.Create();
		var createdPackNames = assets
			.GroupBy(x => x.Package)
			.Select(x =>
				assetPackFactory.Create(x.Key, x, outputPath, minifyJson, compressionLevel)
			);

		var packNamesString = string.Join(", ", createdPackNames);
		logger.LogInformation("Created asset packs {AssetPacks}", packNamesString);


		tableOfContentsWriter.Write(assets, outputPath, minifyJson);

		logger.LogInformation("Assets packs created");
	}
}
