using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class GuideUIController : MonoBehaviour
{
    [Header("Prefab Refs (match your hierarchy)")]
    public TMP_Text titleText;        // BG/Text (제목)
    public TMP_Text descText;         // Media/Text (설명)
    public RawImage videoScreen;      // Media/Video (여기에 RawImage가 있어야 함)
    public VideoPlayer videoPlayer;   // Media/Video/Video Player
    public AudioSource audioSource;   // (선택) 프리팹 안/밖 아무 곳

    [System.Serializable]
    public class GuideContent
    {
        public string label;                // "bull" / "disney"
        public string titleKO;              // 제목
        [TextArea] public string descKO;    // 설명
        public VideoClip videoClip;         // 재생 영상
        public AudioClip ttsClip;           // (선택) TTS
    }

    public void ShowWaiting()
    {
        if (titleText) titleText.text = "분석 중";
        if (descText) descText.text = "결과를 기다리는 중...";
        StopMedia();
    }

    public void ShowResult(GuideContent c)
    {
        if (c == null)
        {
            if (titleText) titleText.text = "알 수 없음";
            if (descText) descText.text = "결과를 해석할 수 없습니다.";
            StopMedia();
            return;
        }

        if (titleText) titleText.text = c.titleKO;
        if (descText) descText.text = c.descKO;

        // Video
        if (videoPlayer && c.videoClip)
        {
            videoPlayer.clip = c.videoClip;
            if (videoScreen) videoScreen.enabled = true;
            videoPlayer.Play();
        }
        else
        {
            if (videoPlayer) videoPlayer.Stop();
            if (videoScreen) videoScreen.enabled = false;
        }

        // TTS
        if (audioSource && c.ttsClip)
        {
            audioSource.Stop();
            audioSource.clip = c.ttsClip;
            audioSource.Play();
        }
    }

    public void StopMedia()
    {
        if (videoPlayer) videoPlayer.Stop();
        if (audioSource) audioSource.Stop();
        if (videoScreen) videoScreen.enabled = false;
    }
}

