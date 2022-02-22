using System;
using System.Threading;
using System.Threading.Tasks;

namespace NitroxModel.Platforms.OS.Shared;

public class NonWinRegistryEx : IRegistryEx
{
    public T Read<T>(string pathWithValue, T defaultValue = default)
    {
        throw new NotImplementedException();
    }

    public bool Delete(string pathWithOptionalValue)
    {
        throw new NotImplementedException();
    }

    public void Write<T>(string pathWithKey, T value)
    {
        throw new NotImplementedException();
    }

    public Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task CompareAsync<T>(string pathWithKey, Func<T, bool> predicate, TimeSpan timeout = default)
    {
        throw new NotImplementedException();
    }
}
