using System;
using System.Threading;
using System.Threading.Tasks;
using NitroxModel.Platforms.OS.Windows;

namespace NitroxModel.Platforms.OS.Shared;

public abstract class RegistryEx
{
    private static readonly Lazy<IRegistryEx> instance = new(() => Environment.OSVersion.Platform switch
                                                             {
                                                                 PlatformID.Unix => new NonWinRegistryEx(),
                                                                 PlatformID.MacOSX => new NonWinRegistryEx(),
                                                                 _ => new WinRegistryEx()
                                                             },
                                                             LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    ///     Reads the value of the registry key or returns the default value of <see cref="T" />.
    /// </summary>
    /// <param name="pathWithValue">
    ///     Full path to the registry key. If the registry hive is omitted then "current user" is used.
    /// </param>
    /// <param name="defaultValue">The default value if the registry key is not found or failed to convert to <see cref="T" />.</param>
    /// <typeparam name="T">Type of value to read. If the value in the registry key does not match it will try to convert.</typeparam>
    /// <returns>Value as read from registry or null if not found.</returns>
    public static T Read<T>(string pathWithValue, T defaultValue = default) => instance.Value.Read(pathWithValue, defaultValue);

    /// <summary>
    ///     Deletes the whole subtree or value, whichever exists.
    /// </summary>
    /// <param name="pathWithOptionalValue">If no value name is given it will delete the key instead.</param>
    /// <returns>True if something was deleted.</returns>
    public static bool Delete(string pathWithOptionalValue) => instance.Value.Delete(pathWithOptionalValue);

    public static void Write<T>(string pathWithKey, T value) => instance.Value.Write(pathWithKey, value);

    /// <summary>
    ///     Waits for a registry value to have the given value.
    /// </summary>
    public static Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, CancellationToken token) => instance.Value.CompareAsync(pathWithKey, predicate, token);

    /// <summary>
    ///     Waits for a registry value to have the given value.
    /// </summary>
    public static Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, TimeSpan timeout = default) => instance.Value.CompareAsync(pathWithKey, predicate, timeout);
}

public interface IRegistryEx
{
    public T Read<T>(string pathWithValue, T defaultValue = default);

    public bool Delete(string pathWithOptionalValue);

    public void Write<T>(string pathWithKey, T value);

    public Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, CancellationToken token);

    public Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, TimeSpan timeout = default);
}
