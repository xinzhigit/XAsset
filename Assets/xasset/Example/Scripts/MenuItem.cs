using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace xasset.example
{
    [Serializable]
    public class MenuItem : MonoBehaviour
    {
        public Slider slider;
        public Text progress;
        public Text intro;
        public Text title;
        public ExampleScene scene;

        public void Enter()
        {
            StartCoroutine(GetDownloadSize());
        }

        private IEnumerator GetDownloadSize()
        {
            var getDownloadSize = Versions.GetDownloadSizeAsync(PreloadManager.GetPreloadAssets(scene));
            yield return getDownloadSize;
            if (getDownloadSize.changes.Count > 0)
            {
                var mb = PreloadManager.GetDownloadSizeMessageBox(getDownloadSize.downloadSize);
                yield return mb;
                if (mb.ok)
                {
                    PreloadManager.Instance.enabled = false;
                    slider.gameObject.SetActive(true);
                    var download = getDownloadSize.DownloadAsync(); 
                    while (download.status != OperationStatus.Success)
                    {
                        slider.value = download.progress;
                        var downloadedBytes = Utility.FormatBytes(download.downloadedBytes);
                        var max = Utility.FormatBytes(download.totalSize);
                        progress.text = $"{downloadedBytes}/{max}";
                        yield return null;
                        if (! download.isDone || download.status == OperationStatus.Success)
                            continue;
                        var messageBox = MessageBox.Show("Tips", "Download failed! Please check your network connection and try again.");
                        yield return messageBox;
                        if (messageBox.ok)
                        {
                            download.Retry();
                        }
                    }
                    progress.text = "Download Completed.";
                    PreloadManager.Instance.enabled = true;
                    yield break;
                }
            }

            PreloadManager.Instance.ShowProgress(Scene.LoadAsync(name));
        }

        public void Bind(MenuItemConfig config)
        {
            name = config.title;
            title.text = config.title;
            intro.text = config.desc;
            scene = config.scene;
        }
    }
}