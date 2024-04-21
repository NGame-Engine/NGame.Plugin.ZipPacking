using Microsoft.Build.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NGame.Assets.UsageFinder.TaskItems;
using NGame.Assets.ZipPacking.Packer.Setup;

namespace NGame.Assets.ZipPacking.Task;



public class CreateAssetPacks : Microsoft.Build.Utilities.Task
{
	[Required] public ITaskItem[] Assets { get; set; } = null!;
	[Required] public bool MinifyJson { get; set; }
	[Required] public int CompressionLevel { get; set; }
	[Required] public string OutputDirectory { get; set; } = null!;


	public override bool Execute()
	{
		try
		{
			var assets =
				Assets
					.Select(UsedAssetMapper.Map)
					.ToList();

			var taskParameters = new TaskParameters(
				assets,
				MinifyJson,
				CompressionLevel,
				OutputDirectory
			);


			var builder = Host.CreateApplicationBuilder();


			builder.Logging.AddAbstractedLogger(
				new LogActionSet(
					x => Log.LogMessage(MessageImportance.Low, x),
					x => Log.LogMessage(MessageImportance.Normal, x),
					x => Log.LogMessage(MessageImportance.High, x),
					x => Log.LogWarning(x),
					x => Log.LogError(x),
					x => Log.LogError(x),
					x => Log.LogErrorFromException(x, true)
				)
			);

			builder.AddZipPacker();


			var host = builder.Build();


			var commandRunner = host.Services.GetRequiredService<ICommandRunner>();
			commandRunner.Run(taskParameters);
		}
		catch (Exception e)
		{
			Log.LogErrorFromException(e, true);
		}

		return Log.HasLoggedErrors == false;
	}
}
