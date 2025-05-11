using System.Reflection;
using System.Runtime.InteropServices;

namespace SoundFlow.Samples.SimplePlayer;

internal static class NativeLibraryResolver
{
    public static nint Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        var libraryPath = GetLibraryPath("libminiaudio");
        if (libraryPath is not null && File.Exists(libraryPath))
        {
            return NativeLibrary.Load(libraryPath);
        }

        return NativeLibrary.Load(libraryName, assembly, searchPath);
    }

    private static string? GetLibraryPath(string libraryName)
    {
        var relativeBase = Directory.Exists("runtimes") ? "runtimes" :
            Directory.EnumerateDirectories(Directory.GetCurrentDirectory(), "runtimes", SearchOption.AllDirectories)
                .Select(dirPath => Path.GetRelativePath(Directory.GetCurrentDirectory(), dirPath))
                .FirstOrDefault();

        if (string.IsNullOrEmpty(relativeBase))
            return null;

        if (OperatingSystem.IsWindows())
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X86 => $"{relativeBase}/win-x86/native/{libraryName}.dll",
                Architecture.X64 => $"{relativeBase}/win-x64/native/{libraryName}.dll",
                Architecture.Arm64 => $"{relativeBase}/win-arm64/native/{libraryName}.dll",
                _ => throw new PlatformNotSupportedException(
                    $"Unsupported Windows architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        if (OperatingSystem.IsMacOS())
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => $"{relativeBase}/osx-x64/native/{libraryName}.dylib",
                Architecture.Arm64 => $"{relativeBase}/osx-arm64/native/{libraryName}.dylib",
                _ => throw new PlatformNotSupportedException(
                    $"Unsupported macOS architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        if (OperatingSystem.IsLinux())
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => $"{relativeBase}/linux-x64/native/{libraryName}.so",
                Architecture.Arm => $"{relativeBase}/linux-arm/native/{libraryName}.so",
                Architecture.Arm64 => $"{relativeBase}/linux-arm64/native/{libraryName}.so",
                _ => throw new PlatformNotSupportedException(
                    $"Unsupported Linux architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        if (OperatingSystem.IsAndroid())
        {
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => $"{relativeBase}/android-x64/native/{libraryName}.so",
                Architecture.Arm => $"{relativeBase}/android-arm/native/{libraryName}.so",
                Architecture.Arm64 => $"{relativeBase}/android-arm64/native/{libraryName}.so",
                _ => throw new PlatformNotSupportedException(
                    $"Unsupported Android architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        if (OperatingSystem.IsIOS())
        {
            libraryName = libraryName.Replace("lib", "");
            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.Arm64 => $"{relativeBase}/ios-arm64/native/{libraryName}.framework/{libraryName}",
                _ => throw new PlatformNotSupportedException(
                    $"Unsupported iOS architecture: {RuntimeInformation.ProcessArchitecture}")
            };
        }

        throw new PlatformNotSupportedException(
            $"Unsupported operating system: {RuntimeInformation.OSDescription}");
    }
}
