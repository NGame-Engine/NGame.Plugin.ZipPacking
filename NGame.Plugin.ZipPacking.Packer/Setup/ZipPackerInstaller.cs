using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NGame.Assets.UsageFinder.Setup;
using NGame.Plugin.ZipPacking.Packer.Commands;
using NGame.Plugin.ZipPacking.Packer.FileWriters;

namespace NGame.Plugin.ZipPacking.Packer.Setup;



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
