using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using xasset;
using Object = UnityEngine.Object;

namespace AutoRelease
{
    public class AutoRelease : MonoBehaviour
    {
        [SerializeField] private Button btnLoadLogo;

        [SerializeField] private Button btnDestroyLogo;
        
        [SerializeField] private Button btnLoadPPTJ;

        [SerializeField] private Button btnDestroyPPTJ;
        
        [SerializeField] private Button btnLoadPlane;

        [SerializeField] private Button btnDestroyPlane;

        [SerializeField] private Image imageLogo;
        
        [SerializeField] private Image imagePPTJ;

        private ResourceManager _resourceManager;

        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        
        private void Awake()
        {
            _resourceManager = new ResourceManager();
            
            btnLoadLogo.onClick.AddListener(OnLoadLogo);
            btnDestroyLogo.onClick.AddListener(OnDestroyLogo);
            
            btnLoadPPTJ.onClick.AddListener(OnLoadPPTJ);
            btnDestroyPPTJ.onClick.AddListener(OnDestroyPPTJ);
            
            btnLoadPlane.onClick.AddListener(OnLoadPlane);
            btnDestroyPlane.onClick.AddListener(OnDestroyPlane);
        }

        private IEnumerator Start()
        {
            // Versions.VerifyMode = verifyMode;
            
            var operation = Versions.InitializeAsync();
            yield return operation;
        }

        private void Update()
        {
            _resourceManager.Update();
        }

        private void OnLoadLogo()
        {
            _resourceManager.LoadAsync<Sprite>("Assets/xasset/Example/Arts/Textures/Logo.png", imageLogo,
                (asset, sprite) =>
                {
                    imageLogo.sprite = sprite;
                });
        }

        private void OnDestroyLogo()
        {
            UnityEngine.Object.Destroy(imageLogo);
        }
        
        private void OnLoadPPTJ()
        {
            imagePPTJ.sprite = _resourceManager.Load<Sprite>("Assets/xasset/Example/Arts/Textures/PPJT.png", imagePPTJ);
        }
        
        private void OnDestroyPPTJ()
        {
            UnityEngine.Object.Destroy(imagePPTJ);
        }

        private void OnLoadPlane()
        {
            _resourceManager.LoadGoAsync("Assets/xasset/Example/Arts/Prefabs/Plane.prefab", transform, (asset, go) =>
            {
                _gameObjects.Add(go);
                
                Debug.Log($"On load game object {_gameObjects.Count}");
            });
        }

        private void OnDestroyPlane()
        {
            if (_gameObjects.Count == 0)
            {
                return;
            }

            var destroyIndex = _gameObjects.Count - 1;
            var destroyGo = _gameObjects[destroyIndex];
            _gameObjects.RemoveAt(destroyIndex);
            
            UnityEngine.Object.Destroy(destroyGo);
            
            Debug.Log($"On destroy game object {_gameObjects.Count}");
        }
    }
}
