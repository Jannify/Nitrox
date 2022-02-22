using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using NitroxModel.Platforms.OS.Shared;

namespace NitroxModel.Platforms.OS.Windows;

public class WinRegistryEx : IRegistryEx
{
    public T Read<T>(string pathWithValue, T defaultValue = default)
    {
        (RegistryKey baseKey, string valueKey) = GetKey(pathWithValue, false);
        if (baseKey == null)
        {
            return defaultValue;
        }

        try
        {
            object value = baseKey.GetValue(valueKey);
            if (value == null)
            {
                return defaultValue;
            }
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
        }
        catch (Exception)
        {
            return defaultValue;
        }
        finally
        {
            baseKey.Dispose();
        }
    }

    public bool Delete(string pathWithOptionalValue)
    {
        (RegistryKey key, string valueKey) = GetKey(pathWithOptionalValue);
        if (key == null)
        {
            return false;
        }

        // Try delete the key.
        RegistryKey prev = key;
        key = key.OpenSubKey(valueKey);
        if (key != null)
        {
            key.DeleteSubKeyTree(valueKey);
            key.Dispose();
            prev.Dispose();
            return true;
        }
        key = prev; // Restore state for next step

        // Not a key, delete the value if it exists.
        if (key.GetValue(valueKey) != null)
        {
            key.DeleteValue(valueKey);
            key.Dispose();
            return true;
        }

        // Nothing to delete.
        return false;
    }

    public void Write<T>(string pathWithKey, T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        (RegistryKey baseKey, string valueKey) = GetKey(pathWithKey, true, true);
        if (baseKey == null)
        {
            return;
        }

        // Figure out what kind of value to store.
        RegistryValueKind? kind = value switch
        {
            int => RegistryValueKind.DWord,
            long => RegistryValueKind.QWord,
            byte[] => RegistryValueKind.Binary,
            string => RegistryValueKind.String,
            _ => null
        };
        // If regKey already exists and we don't know how to parse the value, use existing kind.
        if (!kind.HasValue)
        {
            try
            {
                kind = baseKey.GetValueKind(valueKey);
            }
            catch (Exception)
            {
                // ignored - thrown when key does not exist..
            }
        }

        try
        {
            baseKey.SetValue(valueKey, value, kind.GetValueOrDefault(RegistryValueKind.String));
        }
        finally
        {
            baseKey.Dispose();
        }
    }

    public Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, CancellationToken token)
    {
        static bool Test(RegistryKey regKey, string regKeyName, Func<T, bool> testPredicate)
        {
            T preTestVal = regKey?.GetValue(regKeyName) is T typedValue ? typedValue : default;
            return testPredicate(preTestVal);
        }

        if (token == default)
        {
            CancellationTokenSource source = new(TimeSpan.FromSeconds(10));
            token = source.Token;
        }

        // Test once before in-case it is already successful.
        (RegistryKey baseKey, string valueKey) = GetKey(pathWithKey, false);
        if (Test(baseKey, valueKey, predicate))
        {
            baseKey.Dispose();
            return Task.CompletedTask;
        }

        // Wait for predicate to be successful.
        return Task.Run(async () =>
                        {
                            try
                            {
                                while (!token.IsCancellationRequested)
                                {
                                    // If regkey didn't exist yet it might later.
                                    if (baseKey == null)
                                    {
                                        (baseKey, valueKey) = GetKey(pathWithKey, false);
                                    }

                                    if (!Test(baseKey, valueKey, predicate))
                                    {
                                        await Task.Delay(100, token);
                                        continue;
                                    }

                                    break;
                                }
                            }
                            finally
                            {
                                baseKey?.Dispose();
                            }
                        },
                        token);
    }

    public Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, TimeSpan timeout = default)
    {
        CancellationTokenSource source = new(timeout == default ? TimeSpan.FromSeconds(10) : timeout);
        return CompareAsync(pathWithKey, predicate, source.Token);
    }

    private static (RegistryKey baseKey, string valueKey) GetKey(string path, bool needsWriteAccess = true, bool createIfNotExists = false)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return (null, null);
        }
        path = path.Trim();

        // Parse path to get the registry key instance and the name of the .
        string[] parts = path.Split(Path.DirectorySeparatorChar);
        string[] partsWithoutHive;
        RegistryHive hive = RegistryHive.CurrentUser;
        string regPathWithoutHiveOrKey;
        if (path.IndexOf("Computer", StringComparison.OrdinalIgnoreCase) < 0)
        {
            partsWithoutHive = parts.TakeUntilLast().ToArray();
            regPathWithoutHiveOrKey = string.Join(Path.DirectorySeparatorChar.ToString(), partsWithoutHive);
        }
        else
        {
            partsWithoutHive = parts.Skip(2).TakeUntilLast().ToArray();
            regPathWithoutHiveOrKey = string.Join(Path.DirectorySeparatorChar.ToString(), partsWithoutHive);
            hive = parts[1].ToLowerInvariant() switch
            {
                "hkey_classes_root" => RegistryHive.ClassesRoot,
                "hkey_local_machine" => RegistryHive.LocalMachine,
                "hkey_current_user" => RegistryHive.CurrentUser,
                "hkey_users" => RegistryHive.Users,
                "hkey_current_config" => RegistryHive.CurrentConfig,
                _ => throw new ArgumentException($"Path must contain a valid registry hive but was given '{parts[1]}'", nameof(path))
            };
        }

        RegistryKey hiveRef = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
        RegistryKey key = hiveRef.OpenSubKey(regPathWithoutHiveOrKey, needsWriteAccess);
        // Should the key (and its path leading to it) be created?
        if (key == null && createIfNotExists)
        {
            key = hiveRef;
            foreach (string part in partsWithoutHive)
            {
                RegistryKey prev = key;
                key = key?.OpenSubKey(part, needsWriteAccess) ?? key?.CreateSubKey(part, needsWriteAccess);

                // Cleanup old/parent key reference
                prev?.Dispose();
            }
        }

        return (key, parts[parts.Length - 1]);
    }
}
