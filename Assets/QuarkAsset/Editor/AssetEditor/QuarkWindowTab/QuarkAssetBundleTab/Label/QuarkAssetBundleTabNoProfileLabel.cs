﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace Quark.Editor
{
    public class QuarkAssetBundleTabNoProfileLabel
    {
        AssetBundleBuildProfileData profileData;
        string[] buildHandlers;
        public const string LabelDataName = "QuarkAsset_AssetBundleTabNoProfileLabelData.json";
        bool isAesKeyInvalid = false;
        public bool IsAesKeyInvalid
        {
            get { return isAesKeyInvalid; }
        }
        public void OnEnable(string[] buildHandlers)
        {
            this.buildHandlers = buildHandlers;
            GetTabData();
        }
        public void OnGUI()
        {
            DrawBuildSetting();
            DrawBuildType();
            DrawAESEncryption();
            DrawBuildPathLabel();
            DrawCopyToStreamingLabel();
        }
        public void OnDisable()
        {
            SaveTabData();
        }
        public void Reset()
        {
            profileData = new AssetBundleBuildProfileData();
        }
        public QuarkBuildParams GetBuildParams()
        {
            var buildOption = QuarkBuildController.GetBuildAssetBundleOptions(profileData.AssetBundleCompressType,
                profileData.DisableWriteTypeTree,
                profileData.DeterministicAssetBundle,
                profileData.ForceRebuildAssetBundle,
                profileData.IgnoreTypeTreeChanges);
            var buildParams = new QuarkBuildParams()
            {
                AssetBundleOutputPath = profileData.AssetBundleOutputPath,
                BuildPath = profileData.BuildPath,
                AssetBundleCompressType = profileData.AssetBundleCompressType,
                BuildAssetBundleOptions = buildOption,
                BuildTarget = profileData.BuildTarget,
                CopyToStreamingAssets = profileData.CopyToStreamingAssets,
                ClearStreamingAssetsDestinationPath = profileData.ClearStreamingAssetsDestinationPath,
                StreamingRelativePath = profileData.StreamingRelativePath,
                BuildType = profileData.BuildType,
                BuildVersion = profileData.BuildVersion,
                InternalBuildVersion = profileData.InternalBuildVersion,
                AssetBundleNameType = profileData.AssetBundleNameType,
                UseOffsetEncryptionForAssetBundle = profileData.UseOffsetEncryptionForAssetBundle,
                EncryptionOffsetForAssetBundle = profileData.EncryptionOffsetForAssetBundle,
                UseAesEncryptionForManifest = profileData.UseAesEncryptionForManifest,
                AesEncryptionKeyForManifest = profileData.AesEncryptionKeyForManifest,
                ForceRemoveAllAssetBundleNames = profileData.ForceRemoveAllAssetBundleNames,
                BuildHandlerName = profileData.BuildHandlerName
            };
            return buildParams;
        }
        void GetTabData()
        {
            try
            {
                profileData = QuarkEditorUtility.GetData<AssetBundleBuildProfileData>(LabelDataName);
                var buildHandlerMaxIndex = buildHandlers.Length - 1;
                if (profileData.QuarBuildHandlerIndex > buildHandlerMaxIndex)
                {
                    profileData.QuarBuildHandlerIndex = buildHandlerMaxIndex;
                }
            }
            catch
            {
                profileData = new AssetBundleBuildProfileData();
                QuarkEditorUtility.SaveData(LabelDataName, profileData);
            }
        }
        void SaveTabData()
        {
            QuarkEditorUtility.SaveData(LabelDataName, profileData);
        }
        void DrawAESEncryption()
        {
            profileData.UseAesEncryptionForManifest = EditorGUILayout.ToggleLeft("Aes encryption for buildInfo and manifest", profileData.UseAesEncryptionForManifest);
            if (profileData.UseAesEncryptionForManifest)
            {
                EditorGUILayout.LabelField("BuildInfo AES encryption key, key should be 16,24 or 32 bytes long");
                profileData.AesEncryptionKeyForManifest = EditorGUILayout.TextField("AESKey", profileData.AesEncryptionKeyForManifest);

                var aesKeyStr = profileData.AesEncryptionKeyForManifest;
                var aesKeyLength = System.Text.Encoding.UTF8.GetBytes(aesKeyStr).Length;
                EditorGUILayout.LabelField($"Current key length is:{aesKeyLength}");
                if (aesKeyLength != 16 && aesKeyLength != 24 && aesKeyLength != 32 && aesKeyLength != 0)
                {
                    EditorGUILayout.HelpBox("Key should be 16,24 or 32 bytes long", MessageType.Error);
                }
                GUILayout.Space(16);
            }
            profileData.UseOffsetEncryptionForAssetBundle = EditorGUILayout.ToggleLeft("Offset encryption for asserBundle", profileData.UseOffsetEncryptionForAssetBundle);
            if (profileData.UseOffsetEncryptionForAssetBundle)
            {
                EditorGUILayout.LabelField("AssetBundle encryption offset");
                profileData.EncryptionOffsetForAssetBundle = EditorGUILayout.IntField("Encryption offset", profileData.EncryptionOffsetForAssetBundle);
                if (profileData.EncryptionOffsetForAssetBundle < 0)
                    profileData.EncryptionOffsetForAssetBundle = 0;
            }
        }
        void DrawCopyToStreamingLabel()
        {
            GUILayout.BeginVertical();
            {
                profileData.CopyToStreamingAssets = EditorGUILayout.ToggleLeft("CopyToStreamingAssets", profileData.CopyToStreamingAssets);
                if (profileData.CopyToStreamingAssets)
                {
                    profileData.ClearStreamingAssetsDestinationPath = EditorGUILayout.ToggleLeft("Clear streaming assets destination path", profileData.ClearStreamingAssetsDestinationPath);

                    var streamingRelativePath = profileData.StreamingRelativePath.Trim();
                    if (string.IsNullOrEmpty(streamingRelativePath))
                    {
                        GUILayout.Label("Assets/StreamingAssets/[ Nullable ]");
                    }
                    else
                    {
                        GUILayout.Label($"Assets/StreamingAssets/{streamingRelativePath}");
                    }
                    profileData.StreamingRelativePath = EditorGUILayout.TextField("StreamingRelativePath", streamingRelativePath);
                }
            }
            GUILayout.EndVertical();
        }
        void DrawBuildType()
        {
            profileData.BuildType = (QuarkBuildType)EditorGUILayout.EnumPopup("Build type", profileData.BuildType);
            profileData.AssetBundleNameType = (AssetBundleNameType)EditorGUILayout.EnumPopup("Bundle name type", profileData.AssetBundleNameType);
            profileData.BuildVersion = EditorGUILayout.TextField("Build version", profileData.BuildVersion?.Trim());

            switch (profileData.BuildType)
            {
                case QuarkBuildType.Full:
                    {
                        profileData.InternalBuildVersion = EditorGUILayout.IntField("Internal build version", profileData.InternalBuildVersion);
                        if (profileData.InternalBuildVersion < 0)
                            profileData.InternalBuildVersion = 0;
                        profileData.AssetBundleOutputPath = Path.Combine(profileData.BuildPath, profileData.BuildVersion, profileData.BuildTarget.ToString(), $"{profileData.BuildVersion}_{profileData.InternalBuildVersion}").Replace("\\", "/");
                    }
                    break;
                case QuarkBuildType.Incremental:
                    {
                        profileData.AssetBundleOutputPath = Path.Combine(profileData.BuildPath, profileData.BuildVersion, profileData.BuildTarget.ToString(), profileData.BuildVersion).Replace("\\", "/");
                    }
                    break;
            }
        }
        void DrawBuildPathLabel()
        {
            GUILayout.BeginHorizontal();
            {
                profileData.BuildPath = EditorGUILayout.TextField("Build path", profileData.BuildPath.Trim());
                if (GUILayout.Button("Browse", GUILayout.MaxWidth(128f)))
                {
                    BrowseFolder();
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Build full path", profileData.AssetBundleOutputPath);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Open build Path", GUILayout.MaxWidth(128f)))
                {
                    var path = profileData.AssetBundleOutputPath;
                    if (!Directory.Exists(path))
                    {
                        EditorUtility.RevealInFinder(Application.dataPath);
                    }
                    else
                    {
                        EditorUtility.RevealInFinder(path);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        void DrawBuildSetting()
        {
            profileData.ForceRemoveAllAssetBundleNames = EditorGUILayout.ToggleLeft("Force remove all assetbundle names before build", profileData.ForceRemoveAllAssetBundleNames);
            GUILayout.Space(16);

            profileData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", profileData.BuildTarget);
            profileData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", profileData.AssetBundleCompressType);

            profileData.QuarBuildHandlerIndex = EditorGUILayout.Popup("Build handler", profileData.QuarBuildHandlerIndex, buildHandlers);
            var index = profileData.QuarBuildHandlerIndex;
            if (buildHandlers.Length > 0 && index < buildHandlers.Length)
            {
                profileData.BuildHandlerName = buildHandlers[index];
            }

            profileData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", profileData.ForceRebuildAssetBundle);
            profileData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", profileData.DisableWriteTypeTree);
            if (profileData.DisableWriteTypeTree)
                profileData.IgnoreTypeTreeChanges = false;

            profileData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", profileData.DeterministicAssetBundle);
            profileData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", profileData.IgnoreTypeTreeChanges);
            if (profileData.IgnoreTypeTreeChanges)
                profileData.DisableWriteTypeTree = false;
            GUILayout.Space(16);

        }
        void BrowseFolder()
        {
            var newPath = EditorUtility.OpenFolderPanel("Bundle Folder", profileData.BuildPath, string.Empty);
            if (!string.IsNullOrEmpty(newPath))
            {
                profileData.BuildPath = newPath.Replace("\\", "/");
            }
        }
    }
}
