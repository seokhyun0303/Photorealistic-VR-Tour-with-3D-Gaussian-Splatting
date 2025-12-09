using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class CaptureAndSend2 : MonoBehaviour
{
    [Header("Capture Source")]
    public RenderTexture renderTexture;          // MiniCamera에 연결된 RenderTexture

    [Header("Input")]
    public InputActionReference sendAction;      // B 버튼 액션

    [Header("Server")]
    public string serverUrl = "http://127.0.0.1:8000/detect";

    [Header("UI / Screen")]
    public Transform uiPoint;                    // XR Origin 자식인 UIpoint
    public GameObject screenPrefab;              // IKEA 프리팹(IKEA > BG > Media > Video 구조)
    public TMP_Text debugText;                   // 선택: 디버그용 텍스트

    // 캡처용 버퍼
    private Texture2D _readTex;

    [Serializable]
    private class IkeaResponse
    {
        public string url;
        public string image;    // 상품 이미지 URL
    }

    private void OnEnable()
    {
        if (sendAction != null)
        {
            sendAction.action.performed += OnSendPerformed;
            sendAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (sendAction != null)
        {
            sendAction.action.performed -= OnSendPerformed;
            sendAction.action.Disable();
        }
    }

    private void OnSendPerformed(InputAction.CallbackContext ctx)
    {
        // B 키 누를 때마다 캡처 + 서버로 전송 + UI 생성
        StartCoroutine(CaptureAndUploadCoroutine());
    }

    private IEnumerator CaptureAndUploadCoroutine()
    {
        if (renderTexture == null)
        {
            Log("[ERROR] RenderTexture is null");
            yield break;
        }

        // 1) RenderTexture → Texture2D 캡쳐
        var prev = RenderTexture.active;
        RenderTexture.active = renderTexture;

        if (_readTex == null ||
            _readTex.width != renderTexture.width ||
            _readTex.height != renderTexture.height)
        {
            if (_readTex != null) Destroy(_readTex);
            _readTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        }

        _readTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        _readTex.Apply();
        RenderTexture.active = prev;

        byte[] png = _readTex.EncodeToPNG();

        // 2) 서버로 이미지 전송
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", png, "capture.png", "image/png");

        using (UnityWebRequest req = UnityWebRequest.Post(serverUrl, form))
        {
            req.timeout = 60;
            Log("[INFO] Sending capture to server...");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Log($"[ERROR] Upload failed: {req.error}");
                yield break;
            }

            string json = req.downloadHandler.text;
            Log($"[INFO] Server response: {json}");

            IkeaResponse resp = null;
            try
            {
                resp = JsonUtility.FromJson<IkeaResponse>(json);
            }
            catch (Exception e)
            {
                Log($"[ERROR] JSON parse failed: {e.Message}");
                yield break;
            }

            if (resp == null || string.IsNullOrEmpty(resp.image))
            {
                Log("[WARN] Response has no image url");
                yield break;
            }

            // 3) 응답에서 받은 상품 이미지 URL로 텍스쳐 다운받고 UI 생성
            StartCoroutine(DownloadImageAndSpawnScreen(resp));
        }
    }

    private IEnumerator DownloadImageAndSpawnScreen(IkeaResponse resp)
    {
        Log("[INFO] Downloading product image...");

        using (UnityWebRequest texReq = UnityWebRequestTexture.GetTexture(resp.image))
        {
            texReq.timeout = 60;
            yield return texReq.SendWebRequest();

            if (texReq.result != UnityWebRequest.Result.Success)
            {
                Log($"[ERROR] Image download failed: {texReq.error}");
                yield break;
            }

            Texture2D tex = DownloadHandlerTexture.GetContent(texReq);
            if (tex == null)
            {
                Log("[ERROR] Downloaded texture is null");
                yield break;
            }

            // 4) UI 프리팹 인스턴스 생성 (기존 것들은 그대로 둠)
            if (screenPrefab == null || uiPoint == null)
            {
                Log("[ERROR] screenPrefab or uiPoint is null");
                yield break;
            }

            GameObject ui = Instantiate(screenPrefab, uiPoint.position, uiPoint.rotation);
            ui.name = "IKEA_Screen_" + DateTime.Now.ToString("HHmmss");

            // 5) 프리팹 내부에서 BG/Media/Video 를 찾아 RawImage.texture 에 할당
            Transform videoTf = ui.transform.Find("BG/Media/Video");
            if (videoTf == null)
            {
                Log("[ERROR] Cannot find BG/Media/Video in screen prefab");
                yield break;
            }

            RawImage videoRaw = videoTf.GetComponent<RawImage>();
            if (videoRaw == null)
            {
                Log("[ERROR] RawImage component missing on BG/Media/Video");
                yield break;
            }

            videoRaw.texture = tex;
            videoRaw.color = Color.white; // 혹시 색이 투명하게 되어 있으면 보이도록

            // 6) CanvasGroup 페이드-인 추가
            CanvasGroup cg = ui.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = ui.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            StartCoroutine(FadeInCanvasGroup(cg, 0.5f));   // 0.5초 동안 서서히 등장

            Log("[OK] IKEA screen spawned with product image");
        }
    }

    // CanvasGroup 알파를 0 → 1로 서서히 올리는 코루틴
    private IEnumerator FadeInCanvasGroup(CanvasGroup cg, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            cg.alpha = k;
            yield return null;
        }

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private void Log(string msg)
    {
        Debug.Log("[CaptureAndSend2] " + msg);
        if (debugText != null)
        {
            debugText.text = msg;
        }
    }
}









