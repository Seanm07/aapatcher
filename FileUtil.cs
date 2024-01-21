using System.IO;

public class FileUtil {
    // https://stackoverflow.com/a/937558
    public static bool IsFileLocked(string path) {
        try {
            using (FileStream stream = new FileInfo(path).Open(FileMode.Open, FileAccess.Read, FileShare.None)) {
                stream.Close();
            }
        } catch (IOException) {
            // the file is unavailable because it is:
            // still being written to
            // or being processed by another thread
            // or does not exist (has already been processed)
            return true;
        }

        // file is not locked
        return false;
    }

    public static bool CopyDirectory(string sourceDir, string destDir, bool overwriteExisting = true) {
        try {
            // Get the subdirectories for the specified directory
            string[] subDirectories = Directory.GetDirectories(sourceDir);

            foreach (string subDir in subDirectories) {
                // Create the corresponding subdirectory in the destination directory
                string subDirName = Path.GetFileName(subDir);
                string newSubDir = Path.Combine(destDir, subDirName);

                Directory.CreateDirectory(newSubDir);

                // Recursively copy the subdirectory
                CopyDirectory(subDir, newSubDir);
            }

            // Get all files in the source directory
            string[] files = Directory.GetFiles(sourceDir);

            // Copy each file to the destination directory
            foreach (string file in files) {
                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(destDir, fileName);

                File.Copy(file, destinationPath, overwriteExisting);
            }
        } catch (IOException) {
            return false;
        }

        return true;
    }
}
