using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoRelease
{
    /// <summary>
    /// 通过传递引用对象自动管理对象生命周期
    /// </summary>
    public class ResourceManager
    {
        private class AssetRef
        {
            public xasset.Asset Asset;
            public UnityEngine.Object Go;
        }

        private readonly List<AssetRef> _assets = new List<AssetRef>();

        public void Update()
        {
            UpdateRef();
        }
        
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="user">资源使用者（用来自动管理资源生命周期，必须传）</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string path, UnityEngine.Object user) where T : UnityEngine.Object
        {
            var asset = xasset.Asset.Load(path, typeof(T));
            var obj = asset.asset as T;
            AddRef(asset, user);

            return obj;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="user">资源使用者（用来自动管理资源生命周期，必须传）</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void LoadAsync<T>(string path, UnityEngine.Object user, Action<xasset.Asset,T> func) where T : UnityEngine.Object
        {
            var realFunc = func;
            var realUser = user;

            xasset.Asset.LoadAsync(path,typeof(T), (asset)=>
            {
                realFunc?.Invoke(asset, asset.asset as T);
                
                AddRef(asset, realUser);
            });
        }

        /// <summary>
        /// 同步加载prefab
        /// 加载prefab需要特殊处理，因为要实例化
        /// 加载完成后会实例化，外面不需要再实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public GameObject Load(string path, Transform parent) 
        {
            var asset = xasset.Asset.Load(path, typeof(GameObject));
            var inst = UnityEngine.Object.Instantiate(asset.asset as GameObject, parent);

            AddRef(asset, inst);

            return inst;
        }
        
        /// <summary>
        /// 异步加载prefab
        /// 加载prefab需要特殊处理，因为要实例化
        /// 加载完成后会实例化，外面不需要再实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="func"></param>
        public void LoadGoAsync(string path, Transform parent, Action<xasset.Asset, GameObject> func)
        {
            var realFunc = func;
            
            xasset.Asset.LoadAsync(path, typeof(GameObject), (asset)=>
            {
                var inst = UnityEngine.Object.Instantiate(asset.asset as GameObject, parent);
                
                realFunc?.Invoke(asset, inst);
                
                AddRef(asset, inst);
            });
        }
        
        private void AddRef(xasset.Asset asset, UnityEngine.Object go)
        {
            if (go == null)
            {
                return;
            }
            _assets.Add(new AssetRef { Asset = asset, Go = go});

            Debug.Log($"Add ref {asset} {go}");
        }

        private void UpdateRef()
        {
            for (int i = _assets.Count - 1; i >= 0; --i)
            {
                var assetRef = _assets[i];

                if (assetRef.Go == null)
                {
                    assetRef.Asset?.Release();
                    _assets.RemoveAt(i);
                    
                    Debug.Log($"Remove ref {i} {assetRef.Asset} {assetRef.Go}");
                }
            }
        }
    }
}
