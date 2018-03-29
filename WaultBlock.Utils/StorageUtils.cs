using System.IO;

namespace WaultBlock.Utils
{
    public class StorageUtils
    {
        private static void CleanDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                var dirs = Directory.GetDirectories(path);

                foreach (var file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (var directory in dirs)
                {
                    CleanDirectory(directory);
                }

                Directory.Delete(path, false);
            }
        }

        public static void CleanupStorage()
        {
            var tmpDir = EnvironmentUtils.GetTmpPath();
            var homeDir = EnvironmentUtils.GetIndyHomePath();

            CleanDirectory(tmpDir);
            CleanDirectory(homeDir);
        }
    }
}
