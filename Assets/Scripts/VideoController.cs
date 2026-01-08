using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "Level1";

    private bool isEnding = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndVideo();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        EndVideo();
    }

    private void EndVideo()
    {
        if (isEnding) return;
        isEnding = true;

        videoPlayer.loopPointReached -= OnVideoFinished;

        if (videoPlayer.targetTexture != null)
        {
            RenderTexture.active = videoPlayer.targetTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }
        
        videoPlayer.Stop();

        Debug.Log("Загрузка сцены: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}