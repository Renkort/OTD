using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEditor.Build.Content;

[CreateAssetMenu(fileName = "BuildSettings", menuName = "Build/Build Settings")]
public class BuildSettings : ScriptableObject
{
    public List<string> filesToCopy = new List<string>
    {
        "CHANGELOG.md",
        "README.md"
    };
}
public class AdvancedBuildPostprocessor : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }
    
    public void OnPostprocessBuild(BuildReport report)
    {
        string buildPath = Path.GetDirectoryName(report.summary.outputPath);
        string projectPath = Application.dataPath.Replace("/Assets", "");
        string[] settingsGuids = AssetDatabase.FindAssets("t:BuildSettings");
        if (settingsGuids.Length > 0)
        {
            string settingsPath = AssetDatabase.GUIDToAssetPath(settingsGuids[0]);
            BuildSettings settings = AssetDatabase.LoadAssetAtPath<BuildSettings>(settingsPath);

            foreach (string fileName in settings.filesToCopy)
            {
                string sourceFile = Path.Combine(projectPath, fileName);
                string destFile = Path.Combine(buildPath, fileName);

                if (File.Exists(sourceFile))
                {
                    string destDir = Path.GetDirectoryName(destFile);
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }
                    File.Copy(sourceFile, destFile, true);
                    Debug.Log($"Copied: {fileName}");
                }
            }

        }
    }
}
