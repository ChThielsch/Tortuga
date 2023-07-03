using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

public class BuildPostProcessor
{
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // Only perform the post-process for StandaloneWindows and StandaloneWindows64 targets
        if (target != BuildTarget.StandaloneWindows && target != BuildTarget.StandaloneWindows64)
        {
            Debug.Log($"Post-processing build skipped for target: {target}");
            return;
        }

        // Get the build folder path
        string buildFolderPath = Path.GetDirectoryName(pathToBuiltProject);

        // Get the project name
        string projectName = PlayerSettings.productName;

        // Get the new folder path next to the build folder
        string newFolderName = Path.GetFileName(buildFolderPath) + "_Files";
        string newFolderPath = Path.Combine(Path.GetDirectoryName(buildFolderPath), newFolderName);

        // Move the specified folders to the new folder
        MoveFolder(Path.Combine(buildFolderPath, $"{projectName}_BackUpThisFolder_ButDontShipItWithYourGame"), newFolderPath);
        MoveFolder(Path.Combine(buildFolderPath, $"{projectName}_BurstDebugInformation_DoNotShip"), newFolderPath);

        Debug.Log("Post-processing build complete.");
    }

    private static void MoveFolder(string sourcePath, string destinationPath)
    {
        if (!Directory.Exists(sourcePath))
            return;

        // Create the destination folder if it doesn't exist
        if (!Directory.Exists(destinationPath))
            Directory.CreateDirectory(destinationPath);

        // Get the folder name from the source path
        string folderName = Path.GetFileName(sourcePath);

        // Create the destination folder path with the folder name in the new location
        string destinationFolderPath = Path.Combine(destinationPath, folderName);

        // Move the entire source folder to the destination folder
        Directory.Move(sourcePath, destinationFolderPath);
    }
}
