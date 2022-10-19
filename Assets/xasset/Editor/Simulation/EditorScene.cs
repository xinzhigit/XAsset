using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace xasset.editor
{
    /// <summary>
    ///     编辑器场景实现类
    /// </summary>
    public class EditorScene : Scene
    {
        private EditorDependencies _dependencies;
        internal static Scene Create(string assetPath, bool additive = false)
        {
            if (!File.Exists(assetPath))
            {
                throw new FileNotFoundException(assetPath);
            }

            var scene = new EditorScene
            {
                pathOrURL = assetPath,
                loadSceneMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single
            };
            return scene;
        }

        protected override void OnLoad()
        {
            _dependencies = new EditorDependencies {pathOrURL = pathOrURL};
            _dependencies.Load();
            PrepareToLoad();
            var parameters = new LoadSceneParameters
            {
                loadSceneMode = loadSceneMode
            };
            if (mustCompleteOnNextFrame)
            {
                EditorSceneManager.LoadSceneInPlayMode(pathOrURL, parameters);
                Finish();
            }
            else
            {
                LoadSceneAsync(EditorSceneManager.LoadSceneAsyncInPlayMode(pathOrURL, parameters));
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _dependencies.Release();
        }
    }
}