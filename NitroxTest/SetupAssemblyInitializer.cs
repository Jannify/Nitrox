using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NitroxModel.Helper;
using NitroxModel.Logger;

namespace NitroxTest
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            NitroxEnvironment.Set(NitroxEnvironment.Types.TESTING);
            Log.Setup();

            if (Directory.GetCurrentDirectory().Contains(@"C:\Users\circleci\project\Nitrox")) //Check if environment is circleci
            {
                File.WriteAllText("path.txt", @"C:\PROGRA~2\Steam\steamapps\common\Subnautica");
            }
        }
    }
}
