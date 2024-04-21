using Singulink.IO;

namespace NGame.Plugin.ZipPacking.Packer.FileWriters;



public static class PathExtensions
{
	public static string GetNormalizedZipPath(
		this IRelativeFilePath relativeFilePath
	) =>
		relativeFilePath.PathDisplay.Replace('\\', '/');
}
