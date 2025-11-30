public static class PathHelper
{
    public static string FindFolderUpwards(string folderName)
    {
        var dir = AppContext.BaseDirectory;

        while (dir != null)
        {
            var candidate = Path.Combine(dir, folderName);
            if (Directory.Exists(candidate))
                return candidate;

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new DirectoryNotFoundException($"Folder '{folderName}' not found.");
    }

    public static string F1Data => FindFolderUpwards("Telemetry.F1Data");
}
