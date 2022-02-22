using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroxModel.Platforms.OS.Windows;

namespace NitroxTest.Platforms.OS.Windows;

[TestClass]
public class RegistryTest
{
    [TestMethod]
    public async Task WaitsForRegistryKeyToExist()
    {
        const string PATH_TO_KEY = @"SOFTWARE\Nitrox\test";

        WinRegistryEx registryEx = new();

        registryEx.Write(PATH_TO_KEY, 0);
        Task<bool> readTask = Task.Run(async () =>
        {
            try
            {
                await registryEx.CompareAsync<int>(PATH_TO_KEY,
                                                   v => v == 1337,
                                                   TimeSpan.FromSeconds(5));
                return true;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        });

        registryEx.Write(PATH_TO_KEY, 1337);
        Assert.IsTrue(await readTask);

        // Cleanup (we can keep "Nitrox" key intact).
        registryEx.Delete(PATH_TO_KEY);
        Assert.IsNull(registryEx.Read<string>(PATH_TO_KEY));
    }
}
