#if UNITY_EDITOR
#define DEBUG
#define ASSERT
#endif

using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
/// <summary>
/// A logger that wraps Unity's internal logger.
/// Calls to its methods are stripped in case the LOGGER_SYMBOL is not defined.
/// </summary>
public sealed class SDLogger
{
	private static readonly bool log = true;
	
	// [Conditional("DEBUG")]
	public static void Log(object message)
	{
		if(log) Debug.Log(message);
	}

	[Conditional("DEBUG")]
	public static void LogWarning(object message)
	{
		if(log) Debug.LogWarning(message);
	}
	
	[Conditional("DEBUG")]
	public static void LogWarning(object message, Object context)
	{
		if(log) Debug.LogWarning(message, context);
	}

	[Conditional("DEBUG")]
	public static void LogWarningFormat(string message, params object[] args)
	{
		if(log) Debug.LogWarningFormat(message, args);
	}
	
	[Conditional("DEBUG")]
	public static void LogWarningFormat(Object context, string message, params object[] args)
	{
		if(log) Debug.LogWarningFormat(context, message, args);
	}

	[Conditional("DEBUG")]
	public static void LogError(object message)
	{
		if(log) Debug.LogError(message);
	}

	[Conditional("DEBUG")]
	public static void LogError(object message, Object context)
	{
		if(log) Debug.LogError(message, context);
	}

	[Conditional("DEBUG")]
	public static void LogErrorFormat(string message, params object[] args)
	{
		if(log) Debug.LogErrorFormat(message, args);
	}
	
	[Conditional("DEBUG")]
	public static void LogErrorFormat(Object context, string message, params object[] args)
	{
		if(log) Debug.LogErrorFormat(context, message, args);
	}

	[Conditional("DEBUG")]
	public static void LogException(System.Exception exception)
	{
		if(log) Debug.LogException(exception);
	}
	
	[Conditional("DEBUG")]
	public static void LogException(System.Exception exception, Object context)
	{
		if(log) Debug.LogException(exception, context);
	}
}

