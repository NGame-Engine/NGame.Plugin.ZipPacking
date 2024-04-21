using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NGame.Assets.UsageFinder.Setup;
using NGame.Assets.ZipPacking.Packer.Commands;
using NGame.Assets.ZipPacking.Packer.FileWriters;

namespace NGame.Assets.ZipPacking.Packer.Setup;



public static class ZipPackerInstaller
{
	public static IHostApplicationBuilder AddZipPacker(
		this IHostApplicationBuilder builder
	)
	{
		builder.AddUsageFinder();

		builder.Services.AddTransient<IParameterValidator, ParameterValidator>();

		builder.Services.AddTransient<ICommandRunner, CommandRunner>();

		builder.Services.AddTransient<IAssetPackFactory, AssetPackFactory>();
		builder.Services.AddTransient<ITableOfContentsWriter, TableOfContentsWriter>();


		return builder;
	}
}
