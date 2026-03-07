// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// File help class
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Try deleting the file/directory
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool TryDelete(string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path)) return false;
            if (Directory.Exists(path)) Directory.Delete(path, recursive: true);
            else File.Delete(path);
            return true;
        }
        catch (Exception)
        {
            // ignored
            return false;
        }
    }

    /// <summary>
    /// copy directory
    /// </summary>
    /// <param name="sourceDir"></param>
    /// <param name="destinationDir"></param>
    /// <param name="overwrite"></param>
    public static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = false)
    {
        // Check if the source directory exists
        if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException("Source directory not found: " + sourceDir);

        // If the target directory does not exist, create it
        if (!Directory.Exists(destinationDir)) Directory.CreateDirectory(destinationDir!);

        // Get all files under the source directory and copy them
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string name = Path.GetFileName(file);
            string dest = Path.Combine(destinationDir, name);
            File.Copy(file, dest, overwrite);
        }

        // Recursively copy all subdirectories
        foreach (string directory in Directory.GetDirectories(sourceDir))
        {
            string name = Path.GetFileName(directory);
            string dest = Path.Combine(destinationDir, name);
            CopyDirectory(directory, dest, overwrite);
        }
    }

    /// <summary>
    /// Insert content before the lastIndex identifier of the file (back up the original file)
    /// </summary>
    /// <param name="filePath">file path</param>
    /// <param name="insertContent">content to insert</param>
    /// <param name="identifier">Identifier</param>
    /// <param name="lastIndex">The penultimate identifier</param>
    /// <param name="createBackup">Whether to create backup files</param>
    public static async Task InsertsStringAtSpecifiedLocationInFile(string filePath, string insertContent, char identifier, int lastIndex, bool createBackup = false)
    {
        // Parameter verification
        if (lastIndex < 1) throw new ArgumentOutOfRangeException(nameof(lastIndex));
        if (identifier == 0) throw new ArgumentException("Identifier cannot be null character");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Target file does not exist", filePath);

        // Create backup file
        if (createBackup)
        {
            string backupPath = $"{filePath}.bak_{DateTime.Now:yyyyMMddHHmmss}";
            File.Copy(filePath, backupPath, true);
        }

        using var reader = new StreamReader(filePath, Encoding.UTF8);
        var content = await reader.ReadToEndAsync();
        reader.Close();
        // reverse search algorithm
        int index = content.LastIndexOf(identifier);
        if (index == -1)
        {
            throw new ArgumentException($"{identifier} is not included in the file");
        }

        int resIndex = content.LastIndexOf(identifier, index - lastIndex);
        if (resIndex == -1)
        {
            throw new ArgumentException($"Documentin{identifier}insufficient{lastIndex}piece");
        }

        StringBuilder sb = new StringBuilder(content);
        sb = sb.Insert(resIndex, insertContent);
        await WriteToFileAsync(filePath, sb);
    }

    /// <summary>
    /// Write file contents
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="sb"></param>
    public static async Task WriteToFileAsync(string filePath, StringBuilder sb)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        await using var writer = new StreamWriter(filePath, false, new UTF8Encoding(false)); // No BOM
        await writer.WriteAsync(sb.ToString());
        writer.Close();
        Console.WriteLine($"Document【{filePath}】WriteCompleted");
        Console.ResetColor();
    }
}