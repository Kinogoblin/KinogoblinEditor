using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;

#if ADDRESSABLES_PRESENT
using System;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
#endif

namespace Kinogoblin.Editor
{
    public static class BuildSettingsGUI
    {
        // ==================== CONFIG ====================
        private static string _buildPath = "Builds/Android";

        // Keystore
        private static string _keystorePath = "";
        private static string _keystorePass = "";
        private static string _aliasName = "";
        private static string _aliasPass = "";

        private static bool _playBeepSound = true;
        private static bool _useKeystore = true;
        private static bool _openFolderAfterBuild = true;

        // Styles
        private static GUIStyle _headerStyle;
        private static readonly GUILayoutOption _headerHeight = GUILayout.Height(25);

        // Prefs keys
        private static string ProjectKeyPrefix => $"{Application.productName}_Kinogoblin_Keystore_";

        private static string KeystorePathKey => ProjectKeyPrefix + "Path";
        private static string KeystorePassKey => ProjectKeyPrefix + "Pass";
        private static string AliasNameKey => ProjectKeyPrefix + "AliasName";
        private static string AliasPassKey => ProjectKeyPrefix + "AliasPass";

        // ==================== GUI ====================
        public static void BuildSettingsGUIWindow()
        {
            if (_headerStyle == null)
                _headerStyle = new GUIStyle(GUI.skin.box)
                    { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };

            GUILayout.Box("ANDROID BUILD SETTINGS", _headerStyle, GUILayout.ExpandWidth(true), _headerHeight);
            GUILayout.Space(5);

            LoadPrefs();

            // ===== Build Path =====
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Build Path:", GUILayout.Width(80));
            _buildPath = GUILayout.TextField(_buildPath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selectedPath = EditorUtility.SaveFolderPanel("Select Build Folder", _buildPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                    _buildPath = selectedPath;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            
            // ===== Options =====
            GUILayout.Label("Options", EditorStyles.boldLabel);
            _playBeepSound = EditorGUILayout.Toggle("Play Beep Sound", _playBeepSound);
            _openFolderAfterBuild = EditorGUILayout.Toggle("Open Folder After Build", _openFolderAfterBuild);

            GUILayout.Space(10);

            // ===== Keystore =====
            GUILayout.Label("Keystore Settings", EditorStyles.boldLabel);
            _useKeystore = EditorGUILayout.Toggle("Use Custom Keystore", _useKeystore);

            if (_useKeystore)
            {
                EditorGUILayout.BeginHorizontal();
                _keystorePath = EditorGUILayout.TextField("Keystore Path", _keystorePath);
                if (GUILayout.Button("Browse", GUILayout.Width(80)))
                {
                    string selectedKeystore =
                        EditorUtility.OpenFilePanel("Select Keystore File", _keystorePath, "keystore");
                    if (!string.IsNullOrEmpty(selectedKeystore))
                        _keystorePath = selectedKeystore;
                }

                EditorGUILayout.EndHorizontal();

                _keystorePass = EditorGUILayout.PasswordField("Keystore Pass", _keystorePass);
                _aliasName = EditorGUILayout.TextField("Alias Name", _aliasName);
                _aliasPass = EditorGUILayout.PasswordField("Alias Pass", _aliasPass);
                
                if (GUILayout.Button("💾 Save Keystore Settings"))
                {
                    SavePrefs();
                    Debug.Log("🔐 [BuildSettings] Keystore settings saved.");
                }

                GUILayout.Space(15);

                // ===== Build Buttons =====
                if (GUILayout.Button("Set Keystore Data", GUILayout.Height(35)))
                {
                    ConfigureKeystore();
                }
            }

            GUILayout.Space(15);

            // ===== Build Buttons =====
            if (GUILayout.Button("▶ Build Android APK", GUILayout.Height(35)))
            {
                CreateFullBuild();
            }

            GUILayout.Space(10);

            // ===== Addressables =====
#if ADDRESSABLES_PRESENT
            GUILayout.Label("Addressables", EditorStyles.boldLabel);

            if (GUILayout.Button("🧹 Clean Addressables", GUILayout.Height(25)))
            {
                CleanAddressables();
            }

            if (GUILayout.Button("📦 Build Addressables", GUILayout.Height(25)))
            {
                BuildAddressables();
            }
#else
            EditorGUILayout.HelpBox("Addressables package not found. Some features are disabled.", MessageType.Warning);

            if (GUILayout.Button("➕ Add ADDRESSABLES_PRESENT Define", GUILayout.Height(30)))
            {
                AddAddressablesDefine();
            }
#endif

            GUILayout.Space(10);
        }

        // ==================== BUILD ====================

        [MenuItem("Tools/Kinogoblin tools/Build", false, -100)]
        private static void CreateFullBuild()
        {
            ConfigureKeystore();
            BuildAPK();
        }

        private static void ConfigureKeystore()
        {
            if (!_useKeystore) return;

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = _keystorePath;
            PlayerSettings.Android.keystorePass = _keystorePass;
            PlayerSettings.Android.keyaliasName = _aliasName;
            PlayerSettings.Android.keyaliasPass = _aliasPass;

            Debug.Log("🔑 [BuildSettings] Keystore configured.");
        }

        private static void BuildAPK()
        {
            string apkName =
                $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion}_{PlayerSettings.Android.bundleVersionCode}.apk";
            string sanitizedApkName = string.Concat(apkName.Split(Path.GetInvalidFileNameChars()));

            if (!Directory.Exists(_buildPath))
                Directory.CreateDirectory(_buildPath);

            string apkFullPath = Path.Combine(_buildPath, sanitizedApkName);

            Debug.Log($"📦 [BuildSettings] Building APK: {apkFullPath}");

#if ADDRESSABLES_PRESENT
            CleanAddressables();
            BuildAddressables();
#endif

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => scene.path)
                    .ToArray(),
                locationPathName = apkFullPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"✅ [BuildSettings] APK successfully built: {apkFullPath}");
                if (_playBeepSound)
                    System.Media.SystemSounds.Beep.Play();
                if (_openFolderAfterBuild)
                    Application.OpenURL(Path.GetFullPath(_buildPath));
            }
            else
            {
                if (_playBeepSound)
                    System.Media.SystemSounds.Beep.Play();
                Debug.LogError("❌ [BuildSettings] APK build failed!");
            }
        }

#if ADDRESSABLES_PRESENT
        private static void CleanAddressables()
        {
            Debug.Log("🧹 [BuildSettings] Cleaning Addressables...");
            AddressableAssetSettings.CleanPlayerContent();
            Debug.Log("✅ [BuildSettings] Addressables cleaned!");
        }

        private static void BuildAddressables()
        {
            Debug.Log("📦 [BuildSettings] Building Addressables...");

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var builderInput = new AddressablesDataBuilderInput(settings);

            var addressablesBuildResult =
                settings.ActivePlayerDataBuilder.BuildData<AddressablesPlayerBuildResult>(builderInput);

            if (addressablesBuildResult == null || !string.IsNullOrEmpty(addressablesBuildResult.Error))
            {
                Debug.LogError($"❌ [BuildSettings] Building Addressables failed: " +
                               (addressablesBuildResult?.Error ?? "Unknown error"));
                if(_playBeepSound) 
                    System.Media.SystemSounds.Beep.Play();
                return;
            }

            Debug.Log("✅ [BuildSettings] Addressables built!");
        }
#endif

        // ==================== PREFS ====================
        private static void SavePrefs()
        {
            EditorPrefs.SetString(KeystorePathKey, _keystorePath);
            EditorPrefs.SetString(KeystorePassKey, _keystorePass);
            EditorPrefs.SetString(AliasNameKey, _aliasName);
            EditorPrefs.SetString(AliasPassKey, _aliasPass);
        }

        private static void LoadPrefs()
        {
            _keystorePath = EditorPrefs.GetString(KeystorePathKey, _keystorePath);
            _keystorePass = EditorPrefs.GetString(KeystorePassKey, _keystorePass);
            _aliasName = EditorPrefs.GetString(AliasNameKey, _aliasName);
            _aliasPass = EditorPrefs.GetString(AliasPassKey, _aliasPass);
        }

        // ==================== DEFINES ====================
        private static void AddAddressablesDefine()
        {
            var namedBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            if (!defines.Contains("ADDRESSABLES_PRESENT"))
            {
                defines = string.IsNullOrEmpty(defines) ? "ADDRESSABLES_PRESENT" : $"{defines};ADDRESSABLES_PRESENT";
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, defines);
                Debug.Log("➕ [BuildSettings] ADDRESSABLES_PRESENT define added.");
            }
            else
            {
                Debug.Log("ℹ [BuildSettings] ADDRESSABLES_PRESENT already exists.");
            }
        }
    }
}