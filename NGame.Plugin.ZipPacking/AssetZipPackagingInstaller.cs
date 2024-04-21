using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NGame.Assets.Serialization;

namespace NGame.Plugin.ZipPacking;



public static class AssetZipPackagingInstaller
{
	public static IHostApplicationBuilder AddAssetZipPackaging(
		this IHostApplicationBuilder builder
	)
	{
		builder.Services.AddSingleton<IStoredAssetReader, ZippedStoredAssetReader>();
		builder.Services.AddTransient<IZippedAssetUnpacker, ZippedAssetUnpacker>();

		return builder;
	}
}
