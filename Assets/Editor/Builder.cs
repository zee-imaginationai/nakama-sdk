using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System;
using System.Collections.Generic;

namespace CustomEditorUtilities
{
    public class Builder: MonoBehaviour
    {
        //[SerializeField] private static BuilderConfig BuilderConfig;

        [MenuItem("Build/Build iOS")]
        public static void BuildiOS()
        {
            ConfigurePlayerSettingsForBuild(BuildTarget.iOS);
            Build(BuildTarget.iOS, BuildOptions.None, "iOSBuild");
        }

        [MenuItem("Build/Build Android APK")]
        public static void BuildAndroidAPK()
        {
            ConfigurePlayerSettingsForBuild(BuildTarget.Android);
            BuildAPK();
        }

        [MenuItem("Build/Build Android AAB")]
        public static void BuildAndroidAAB()
        {
            ConfigurePlayerSettingsForBuild(BuildTarget.Android);
            BuildAAB();
        }

        [MenuItem("Build/Build Android Development APK")]
        public static void BuildAndroidDevelopmentAPK()
        {
            ConfigurePlayerSettingsForBuild(BuildTarget.Android);
            BuildDevelopmentAPK();
        }

        private static void ConfigurePlayerSettingsForBuild(BuildTarget target)
        {
            var buildVersion = GetArgument("-buildversion");

            if (!string.IsNullOrEmpty(buildVersion))
            {
                PlayerSettings.bundleVersion = buildVersion;
            }
            else
            {
                //If the Argument is NULL pick the default from PlayerSettings
                buildVersion = PlayerSettings.bundleVersion;
            }

            int versionCode = GenerateVersionCode(buildVersion);

            if (target == BuildTarget.Android)
            {
                Debug.Log($"Setting Version Code to {versionCode}");
                PlayerSettings.Android.bundleVersionCode = versionCode;
            }
            else if (target == BuildTarget.iOS)
            {
                PlayerSettings.iOS.buildNumber = versionCode.ToString();
            }


            // PlayerSettings.Android.keystorePass = BuilderConfig.KeystorePassword;
            // PlayerSettings.Android.keyaliasPass = BuilderConfig.KeyAliasPassword;
            
            // PlayerSettings.Android.keystorePass = "sgs123";
            // PlayerSettings.Android.keyaliasPass = "sgs123";
        }

        private static int GenerateVersionCode(string version)
        {
            // Remove all dots from the version string to get a continuous number
            string numericVersion = version.Replace(".", "");

            // Try parsing the numeric version as an integer for version code
            if (int.TryParse(numericVersion, out int versionCode))
            {
                return versionCode;
            }
            else
            {
                Debug.LogError("Failed to parse version code from version string: " + version);
                return 1; // Default to 1 if parsing fails
            }
        }

        private static void BuildAPK()
        {
            //REF: https://docs.unity3d.com/6000.0/Documentation/Manual/android-symbols.html

            EditorUserBuildSettings.buildAppBundle = false;
            Build(BuildTarget.Android, BuildOptions.None,
                $"AndroidBuilds/{PlayerSettings.productName}_{PlayerSettings.bundleVersion}-{PlayerSettings.Android.bundleVersionCode}.apk");
        }

        private static void BuildAAB()
        {
            //REF: https://docs.unity3d.com/6000.0/Documentation/Manual/android-symbols.html
            
            EditorUserBuildSettings.buildAppBundle = true;
            Build(BuildTarget.Android, BuildOptions.None,
                $"AndroidBuilds/{PlayerSettings.productName}_{PlayerSettings.bundleVersion}-{PlayerSettings.Android.bundleVersionCode}.aab");
        }

        private static void BuildDevelopmentAPK()
        {
            //REF: https://docs.unity3d.com/6000.0/Documentation/Manual/android-symbols.html
            
            EditorUserBuildSettings.buildAppBundle = false;
            Build(BuildTarget.Android, BuildOptions.Development,
                $"AndroidBuilds/{PlayerSettings.productName}_{PlayerSettings.bundleVersion}-{PlayerSettings.Android.bundleVersionCode}_Dev.apk");
        }

        private static void Build(BuildTarget target, BuildOptions options, string pathName)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = GetActiveScenes(),
                locationPathName = $"Builds/{pathName}",
                target = target,
                options = options
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            HandleBuildReport(report);
        }

        private static void HandleBuildReport(BuildReport report)
        {
            var summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
                Debug.Log($"Build succeeded: {summary.totalSize} bytes");
            else if (summary.result == BuildResult.Failed)
                Debug.Log("Build failed");
        }

        private static string GetArgument(string argName)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == argName && args[i + 1] != "empty" && args[i + 1] != "null")
                    return args[i + 1];
            }

            return null;
        }

        private static string[] GetActiveScenes()
        {
            List<string> scenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    scenes.Add(scene.path);
            }

            return scenes.ToArray();
        }
    }
}