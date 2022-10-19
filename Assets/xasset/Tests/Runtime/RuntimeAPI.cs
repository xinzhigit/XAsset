using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace xasset.tests
{
    public class RuntimeAPI
    {
        [UnityTest]
        public IEnumerator InitializeAsync()
        {
            var initialize = Versions.InitializeAsync();
            yield return initialize;
            Assert.AreEqual(initialize.status, OperationStatus.Success);
            Download.FtpPassword = string.Empty;
            Download.FtpUserID = string.Empty;
            Updater.maxUpdateTimeSlice = float.MaxValue;
            Bundle.customLoader = null;
        }

        [UnityTest]
        public IEnumerator LoadAssetWithSubAssetsAsync()
        {
            yield return InitializeAsync();
            const string path = "Assets/xasset/Example/Arts/Textures/igg.png";
            var asset = Asset.LoadWithSubAssetsAsync(path, typeof(Sprite));
            asset.completed += a =>
            {
                Debug.Log($"Completed:{a.pathOrURL}");
                Assert.NotNull(a.subAssets);
            };
            asset.Release();
            yield return asset;
            yield return new WaitForSeconds(1);
            Debug.Log(asset.status);
            asset = Asset.LoadWithSubAssets(path, typeof(Sprite));
            Debug.Log(asset.status);
            Assert.NotNull(asset.subAssets);
            yield return new WaitForSeconds(3);
            asset.Release();
            yield return new WaitForSeconds(3);
        }

        [UnityTest]
        public IEnumerator ClearAsync()
        {
            yield return InitializeAsync();
            var clear = Versions.ClearAsync();
            Assert.True(!clear.isDone);
            yield return clear;
            Assert.True(clear.isDone);
            Assert.True(string.IsNullOrEmpty(clear.error));
        }

        [UnityTest]
        public IEnumerator UpdateAsync()
        {
            yield return InitializeAsync();
            Versions.ClearDownload();
            Downloader.DownloadURL = "http://127.0.0.1/Bundles/";
            var check = Versions.CheckUpdateAsync();
            yield return check;
            if (check.downloadSize > 0) yield return check.DownloadAsync();
            var getDownloadSize = Versions.GetDownloadSizeAsync();
            yield return getDownloadSize;
            yield return getDownloadSize.DownloadAsync();
        }

        [UnityTest]
        public IEnumerator LoadAssetFromAsyncToSync()
        {
            yield return InitializeAsync();
            for (var i = 0; i < 3; i++)
            {
                var asset = Asset.LoadAsync("Assets/xasset/Example/Prefabs/Children2.prefab", typeof(GameObject));
                asset.completed += a =>
                {
                    Debug.Log($"Completed:{a.pathOrURL}");
                    Assert.NotNull(a.asset);
                };
                asset.Release();
                asset = Asset.Load("Assets/xasset/Example/Prefabs/Children2.prefab", typeof(GameObject));
                Assert.NotNull(asset.asset);
                asset.Release();
            }

            yield return new WaitUntil(() => Loadable.Unused.Count == 0 && Loadable.Loading.Count == 0);
        }

        [UnityTest]
        public IEnumerator LoadScene()
        {
            yield return InitializeAsync();
            var scene = Scene.LoadAsync("Assets/xasset/Example/Arts/Scenes/Menu.unity");
            scene.allowSceneActivation = false;
            var assetNames = new[]
            {
                "Assets/xasset/Example/Arts/Prefabs/ButtonMenu.prefab",
                "Assets/xasset/Example/Arts/Prefabs/Children.prefab",
                "Assets/xasset/Example/Arts/Prefabs/Children2.prefab",
                "Assets/xasset/Example/Arts/Prefabs/Plane.prefab"
            };

            var assets = new List<Asset>();
            foreach (var assetName in assetNames) assets.Add(Asset.LoadAsync(assetName, typeof(GameObject)));
            scene.completed += s => { Debug.Log($"Completed:{s.pathOrURL}"); };
            yield return scene;
            yield return new WaitUntil(() => assets.TrueForAll(a => a.isDone));
            yield return new WaitForSeconds(3);
            scene.allowSceneActivation = true;
        }
    }
}