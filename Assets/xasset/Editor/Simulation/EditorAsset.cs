using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace xasset.editor
{
    public class EditorAsset : Asset
    {
        private EditorDependencies _dependencies;

        protected override void OnLoad()
        {
            _dependencies = new EditorDependencies {pathOrURL = pathOrURL};
            _dependencies.Load();
        }

        protected override void OnUnload()
        {
            base.OnUnload(); 
            _dependencies.Release();
            if (_dependencies.unused)
                if (type != typeof(GameObject) && !(asset is GameObject))
                {
                    if (isSubAssets)
                        foreach (var subAsset in subAssets)
                            Resources.UnloadAsset(subAsset);
                    else
                        Resources.UnloadAsset(asset);
                    _updateUnloadUnusedAssets = true;
                }

            subAssets = null;
            asset = null;
        }

        protected override void OnUpdate()
        {
            if (status != LoadableStatus.Loading) return;

            FinishLoad();
        }


        private void FinishLoad()
        {
            if (isSubAssets)
            {
                subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(pathOrURL);
                Finish();
            }
            else
            {
                OnLoaded(AssetDatabase.LoadAssetAtPath(pathOrURL, type));
            }
        }

        public override void LoadImmediate()
        {
            if (isDone) return;
            FinishLoad();
        }

        internal static EditorAsset Create(string path, Type type)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            return new EditorAsset
            {
                pathOrURL = path,
                type = type
            };
        }
    }
}