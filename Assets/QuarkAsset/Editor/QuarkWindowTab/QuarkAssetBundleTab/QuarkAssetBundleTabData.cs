﻿using System;
using System.IO;
using UnityEditor;
namespace Quark.Editor
{
    [Serializable]
    internal class QuarkAssetBundleTabData
    {
        public BuildTarget BuildTarget;
        public string BuildVersion;
        public int InternalBuildVersion;
        public string BuildPath;
        public string AssetBundleOutputPath;
        public bool ClearOutputFolders;
        public bool CopyToStreamingAssets;
        public string StreamingRelativePath;
        public AssetBundleNameType AssetBundleNameType;
        /// <summary>
        /// AB资源压缩类型；
        /// </summary>
        public AssetBundleCompressType AssetBundleCompressType;
        /// <summary>
        /// 不会在AssetBundle中包含类型信息;
        /// </summary>
        public bool DisableWriteTypeTree;
        /// <summary>
        /// 使用存储在Asset Bundle中的对象的id的哈希构建Asset Bundle;
        /// </summary>
        public bool DeterministicAssetBundle;
        /// <summary>
        /// 强制重建Asset Bundles;
        /// </summary>
        public bool ForceRebuildAssetBundle;
        /// <summary>
        /// 执行增量构建检查时忽略类型树更改;
        /// </summary>
        public bool IgnoreTypeTreeChanges;
        /// <summary>
        /// 使用偏移加密；
        /// </summary>
        public bool UseOffsetEncryptionForAssetBundle;
        /// <summary>
        /// 加密偏移量；
        /// </summary>
        public int EncryptionOffsetForAssetBundle;

        /// <summary>
        /// 使用对称加密对build信息进行加密;
        /// </summary>
        public bool UseAesEncryptionForManifest;
        /// <summary>
        /// 对称加密的密钥；
        /// </summary>
        public string AesEncryptionKeyForManifest;

        public BuildAssetBundleOptions BuildAssetBundleOptions;

        public string QuarkBuildHandlerName;

        public int QuarkBuildHandlerIndex;
        /// <summary>
        /// 是否增量打包
        /// </summary>
        public bool IncrementalBuild;
        public QuarkAssetBundleTabData()
        {
            BuildTarget = BuildTarget.StandaloneWindows;
            AssetBundleCompressType = AssetBundleCompressType.ChunkBasedCompression_LZ4;
            BuildPath = Path.Combine(Path.GetFullPath("."), "AssetBundles", "QuarkAsset").Replace("\\", "/");
            ClearOutputFolders = true;
            CopyToStreamingAssets = false;
            AssetBundleNameType = AssetBundleNameType.DefaultName;
            UseOffsetEncryptionForAssetBundle = false;
            EncryptionOffsetForAssetBundle = 32;
            UseAesEncryptionForManifest = false;
            AesEncryptionKeyForManifest = "QuarkAssetAesKey";
            BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
            BuildVersion = "0_0_1";
            StreamingRelativePath = BuildVersion;
            QuarkBuildHandlerName = QuarkConstant.NONE;
            QuarkBuildHandlerIndex = 0;
            ForceRebuildAssetBundle = false;
            DisableWriteTypeTree = false;
            DeterministicAssetBundle = false;
            IgnoreTypeTreeChanges = false;
        }
    }
}