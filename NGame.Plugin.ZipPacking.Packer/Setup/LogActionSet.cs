﻿namespace NGame.Plugin.ZipPacking.Packer.Setup;



public class LogActionSet(
	Action<string> logTrace,
	Action<string> logDebug,
	Action<string> logInformation,
	Action<string> logWarning,
	Action<string> logError,
	Action<string> logCritical,
	Action<Exception> logException
)
{
	public Action<string> LogTrace { get; } = logTrace;
	public Action<string> LogDebug { get; } = logDebug;
	public Action<string> LogInformation { get; } = logInformation;
	public Action<string> LogWarning { get; } = logWarning;
	public Action<string> LogError { get; } = logError;
	public Action<string> LogCritical { get; } = logCritical;
	public Action<Exception> LogException { get; } = logException;
}
