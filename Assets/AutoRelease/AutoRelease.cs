using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using xasset;

namespace AutoRelease
{
    public class AutoRelease : MonoBehaviour
    {
        [SerializeField] private Button btnLoad;

        [SerializeField] private Button btnDestroyLogo;

        [SerializeField] private Image imageLogo;

        private class AssetRef
        {
            public Asset Asset;
            public GameObject Go;
        }

        private readonly List<AssetRef> _assets = new List<AssetRef>();

        private void Awake()
        {
            btnLoad.onClick.AddListener(OnLoad);
            btnDestroyLogo.onClick.AddListener(OnDestroyLogo);
        }

        private IEnumerator Start()
        {
            // Versions.VerifyMode = verifyMode;
            
            var operation = Versions.InitializeAsync();
            yield return operation;
        }

        private void Update()
        {
            UpdateRef();
        }

        private void OnLoad()
        {
            Asset.LoadAsync("Assets/xasset/Example/Arts/Textures/Logo.png", typeof(Sprite), asset =>
            {
                if (imageLogo == null)
                {
                    return;
                }

                imageLogo.sprite = asset.Get<Sprite>();
                
                AddRef(asset, imageLogo.gameObject);
            });
        }

        private void OnDestroyLogo()
        {
            UnityEngine.Object.Destroy(imageLogo.gameObject);
        }

        private void AddRef(Asset asset, GameObject go)
        {
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
