using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NitroxModel.Discovery.InstallationFinders;

/// <summary>
///     Tries to read a file that contains the installation directory of Subnautica.
/// </summary>
public class TextFileGameFinder : IFindGameInstallation
{
    public string FindGame(IList<string> errors = null)
    {
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "subnauticaPath.txt");
        if (!File.Exists(filePath))
        {
            return null;
        }

        string path = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(path))
        {
            errors?.Add($@"Configured game path was found empty in file: {filePath}. Please enter the path to the Subnautica installation.");
            return null;
        }

        if (!Directory.Exists(Path.Combine(path, "Subnautica_Data", "Managed")))
        {
            errors?.Add($@"Game installation directory config '{path}' is invalid. Please enter the path to the Subnautica installation.");
            return null;
        }

        return path;
    }
}
