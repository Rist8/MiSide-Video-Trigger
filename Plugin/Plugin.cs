using BepInEx.Unity.IL2CPP.Utils;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

public class VideoTrigger : MonoBehaviour
{

    static Canvas canvas = null;
    static GameObject iconGamePrint = null;
    static VideoPlayer videoPlayer = null;
    static bool ready = false;

    private void Start()
	{
        SceneManager.sceneLoaded += ((UnityAction<Scene, LoadSceneMode>)OnSceneChanged);
        ConsoleMain.active = true;
    }

    void Update()
	{

        if (ConsoleMain.liteVersion)
		{
			ConsoleMain.liteVersion = false;
		}

        // Subscribe to the loopPointReached event to detect when the video ends
        if (ready && videoPlayer != null && videoPlayer.frameCount != 0
            && videoPlayer.frame == (long)videoPlayer.frameCount - 1)
        {
            Debug.Log(videoPlayer.frame + " " + videoPlayer.frameCount);
            videoPlayer.Stop();
            Component.Destroy(videoPlayer);
            GameObject.Destroy(canvas.GetComponentInChildren<RawImage>().gameObject);
            videoPlayer = null;
        }
    }

    void OnSceneChanged(Scene current, LoadSceneMode mode)
    {
        if(current.name == "Scene 14 - MobilePlayer")
        {
            PatchTrigger();
        }
    }

    static void PatchTrigger()
    {
        iconGamePrint = GameObject.Find("World/Quest/Quest 3 RealRoom/CanvasDisplay/Icon GamePrint");
        canvas = iconGamePrint.transform.parent.gameObject.GetComponent<Canvas>();
        iconGamePrint.GetComponent<Button>().onClick.AddListener((UnityAction)(() => PlayVideoOnCanvas()));
    }

    static void PlayVideoOnCanvas()
    {
        iconGamePrint.transform.Find("Text").gameObject.GetComponent<Text>().text = "ꜱ͛ɘ͓ᴄ͍ʀ͗ɇ͠ᴛ͜";
        if (canvas == null)
        {
            Debug.LogError("Canvas not found or not set!");
            return;
        }

        Debug.Log("Playing video on canvas");

        // Create a RawImage on the canvas if one doesn't already exist
        RawImage rawImage = canvas.GetComponentInChildren<RawImage>();
        if (rawImage == null)
        {
            GameObject rawImageObject = new GameObject("VideoRawImage");
            rawImageObject.transform.SetParent(canvas.transform, false);

            rawImage = rawImageObject.AddComponent<RawImage>();
            rawImage.rectTransform.anchorMin = Vector2.zero;
            rawImage.rectTransform.anchorMax = Vector2.one;
            rawImage.rectTransform.offsetMin = Vector2.zero;
            rawImage.rectTransform.offsetMax = Vector2.zero;
        }

        // Configure the VideoPlayer
        if (videoPlayer == null)
        {
            videoPlayer = canvas.gameObject.AddComponent<VideoPlayer>();
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;

        // Set the video URL (adjust as needed for your use case)
        string videoPath = System.IO.Path.Combine(PluginInfo.AssetsFolder, "video.mp4");
        videoPlayer.url = videoPath;

        // Render the video on the RawImage's texture
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;


        videoPlayer.Prepare();
        videoPlayer.Play();

        ready = true;
    }

}
