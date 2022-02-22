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
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "subnauticaPath.txt");
        if (!File.Exists(filePath))
        {
            return null;
        }

        string subnauticaPath = File.ReadAllText(filePath).Trim();
        if (string.IsNullOrEmpty(subnauticaPath))
        {
            errors?.Add($@"Configured game path was found empty in file: {filePath}. Please enter the path to the Subnautica installation.");
            return null;
        }

        if (!Directory.Exists(Path.Combine(subnauticaPath, "Subnautica_Data", "Managed")))
        {
            errors?.Add($@"Game installation directory config '{subnauticaPath}' is invalid. Please enter the path to the Subnautica installation.");
            return null;
        }

        return subnauticaPath;
    }
}
