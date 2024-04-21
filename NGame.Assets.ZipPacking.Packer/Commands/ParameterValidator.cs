using System.IO.Compression;
using NGame.Assets.ZipPacking.Packer.Setup;
using Singulink.IO;

namespace NGame.Assets.ZipPacking.Packer.Commands;



public interface IParameterValidator
{
	ValidatedCommand ValidateCommand(TaskParameters taskParameters);
}



public class ParameterValidator : IParameterValidator
{
	public ValidatedCommand ValidateCommand(TaskParameters taskParameters)
	{
		var assets = taskParameters.Assets;


		var minifyJson = taskParameters.MinifyJson;


		var compressionLevel =
			taskParameters.CompressionLevel switch
			{
				0 => CompressionLevel.Optimal,
				1 => CompressionLevel.Fastest,
				2 => CompressionLevel.NoCompression,
				3 => CompressionLevel.SmallestSize,
				var invalid => throw new InvalidOperationException($"Invalid CompressionLevel '{invalid}'")
			};


		var outputFilePath = DirectoryPath.ParseAbsolute(taskParameters.OutputDirectory);

		return new ValidatedCommand(
			assets,
			minifyJson,
			compressionLevel,
			outputFilePath
		);
	}
}
