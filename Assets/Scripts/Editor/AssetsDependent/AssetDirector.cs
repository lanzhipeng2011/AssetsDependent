﻿using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
namespace Assets.Dependent { 
    public class AssetDirector:IDisposable
    {
        static private AssetDirector _director;
        static public AssetDirector Director {
            get {
                if (null == _director) {
                    _director = new AssetDirector();
                }
                return _director;
            }
        }
        /// <summary>
        /// 总的资源节点
        /// </summary>
        
        [MenuItem("Dependent/收集资源 #c")]
        public static void CollectAssets() {
            Director.CollectAssets(Application.dataPath);
        }
        /// <summary>
        /// 资源收集接口 1.收集要打包的资源 2.创建资源节点并收集资源依赖文件
        /// </summary>
        /// <param name="root"></param>
        private void CollectAssets(string root) {
            try
            {
                AssetNode.Clean();
                var assets = Directory.GetFiles(root.Substring(root.Length - 6), @"*", SearchOption.AllDirectories).Where(s =>
                {
                    return AssetUtil.IsVaild(s);
                });
                var assetList = assets.ToList();
                int nCount = assetList.Count;
                for (int index = 0; index < nCount; index++)
                {
                    assetList[index] = assetList[index].Replace("\\", "/");
                    EditorUtility.DisplayProgressBar("路径转换", assetList[index], index * 1f / nCount);
                }
                AssetNode.Create(assetList, (p, f) =>
                {
                    EditorUtility.DisplayProgressBar("节点创建", f, p);
                });
                AssetNode.CollectRes((p, f) =>
                {
                    EditorUtility.DisplayProgressBar("收集依赖资源", f, p);
                });
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }
        public void Dispose()
        {
            AssetNode.Clean();
        }
    }
}
