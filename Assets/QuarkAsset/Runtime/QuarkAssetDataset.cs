﻿using System;
using UnityEngine;
using System.Collections.Generic;
namespace Quark.Asset
{
    /// <summary>
    /// QuarkAssetDataset用于在Editor Runtime快速开发时使用；
    /// build之后需配合AB资源使用；
    /// </summary>
    [Serializable]
    public sealed class QuarkAssetDataset : ScriptableObject, IDisposable, IQuarkLoaderData
    {
        Dictionary<string, List<QuarkAssetObject>> assetBundleDict;
        [SerializeField]
        List<QuarkBundleInfo> quarkBundleInfoList;
        [SerializeField]
        List<QuarkAssetObject> quarkAssetObjectList;
        [SerializeField]
        List<string> quarkAssetExts;
        public int QuarkAssetCount { get { return QuarkAssetObjectList.Count; } }
        /// <summary>
        /// 包含的路径；
        /// <see cref="QuarkBundleInfo"/>
        /// </summary>
        public List<QuarkBundleInfo> QuarkBundleInfoList
        {
            get
            {
                if (quarkBundleInfoList == null)
                    quarkBundleInfoList = new List<QuarkBundleInfo>();
                return quarkBundleInfoList;
            }
        }
        public List<QuarkAssetObject> QuarkAssetObjectList
        {
            get
            {
                if (quarkAssetObjectList == null)
                    quarkAssetObjectList = new List<QuarkAssetObject>();
                return quarkAssetObjectList;
            }
        }
        /// <summary>
        /// AB名===AB中的资源；
        /// </summary>
        public Dictionary<string, List<QuarkAssetObject>> AssetBundleDict
        {
            get
            {
                if (assetBundleDict == null)
                    assetBundleDict = new Dictionary<string, List<QuarkAssetObject>>();
                return assetBundleDict;
            }
        }
        public List<string> QuarkAssetExts
        {
            get
            {
                if (quarkAssetExts == null)
                    quarkAssetExts = new List<string>();
                return quarkAssetExts;
            }
        }
        public void Dispose()
        {
            quarkAssetObjectList?.Clear();
            quarkBundleInfoList?.Clear();
            assetBundleDict?.Clear();
        }
    }
}