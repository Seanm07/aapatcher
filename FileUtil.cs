using System.IO;
using System.IO.Compression;

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

    public static bool ChangeFileExtensions(string directory, string fromExt, string toExt) {
        try {
            string[] files = Directory.GetFiles(directory, "*." + fromExt, SearchOption.AllDirectories);

            foreach (string file in files)
                File.Move(file, Path.ChangeExtension(file, "." + toExt));
        } catch (IOException) {
            return false;
        }

        return true;
    }

    public static bool DeleteDirectory(string filePath) {
        try {
            if (Directory.Exists(filePath))
                Directory.Delete(filePath, true);
        } catch (IOException) {
            return false;
        }

        return true;
    }

    public static string TempFilePath() {
        return Path.GetTempPath() + "archeage_addon_manager/";
    }

    public static bool CreateZipFile(string[] filesToZip, string zipFilePath) {
        try {
            ZipArchive newZipFile = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);

            foreach (string fileToZip in filesToZip) {
                if (File.Exists(fileToZip)) {
                    newZipFile.CreateEntryFromFile(fileToZip, Path.GetFileName(fileToZip));
                } else {
                    throw new IOException("Failed to create zip, file " + fileToZip + " does not exist!");
                }
            }
            
            // Dispose of the zip file or it'll be locked in use by the program
            newZipFile.Dispose();
        } catch (IOException) {
            return false;
        }

        return true;
    }
}
