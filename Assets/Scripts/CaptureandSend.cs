using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class CaptureandSend : MonoBehaviour
{
    [Header("Capture Source")]
    [Tooltip("MiniCam에 꽂혀있는 RenderTexture")]
    public RenderTexture renderTexture;

    [Header("Input")]
    [Tooltip("Right secondaryButton(B)에 바인딩된 액션")]
    public InputActionReference sendAction;

    [Header("Server")]
    public string serverUrl = "http://127.0.0.1:8000/upload"; // FastAPI 엔드포인트

    // 임시 버퍼
    private Texture2D _readTex;

    void OnEnable()
    {
        if (sendAction != null)
        {
            sendAction.action.performed += OnSendPerformed;
            sendAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (sendAction != null)
        {
            sendAction.action.performed -= OnSendPerformed;
            sendAction.action.Disable();
        }
    }

    private void OnSendPerformed(InputAction.CallbackContext ctx)
    {
        if (renderTexture == null)
        {
            Debug.LogWarning("[CaptureAndSend] RenderTexture가 비어 있음");
            return;
        }
        StartCoroutine(CaptureAndUploadCoroutine());
    }

    private IEnumerator CaptureAndUploadCoroutine()
    {
        // 1) RT -> Texture2D 복사
        var prev = RenderTexture.active;
        RenderTexture.active = renderTexture;

        if (_readTex == null || _readTex.width != renderTexture.width || _readTex.height != renderTexture.height)
        {
            if (_readTex != null) Destroy(_readTex);
            _readTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        }

        _readTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
        _readTex.Apply(false, false);

        RenderTexture.active = prev;

        // 2) PNG 인코딩
        byte[] png = _readTex.EncodeToPNG();

        // 3) multipart/form-data 업로드
        WWWForm form = new WWWForm();
        string filename = $"capture_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png";
        form.AddBinaryData("file", png, filename, "image/png");

        // (선택) 메타데이터 예시: 카메라 포즈
        form.AddField("pose_json", PoseToJson());

        using (UnityWebRequest req = UnityWebRequest.Post(serverUrl, form))
        {
            req.timeout = 10; // 초
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[CaptureAndSend] Upload failed: {req.error}");
            }
            else
            {
                // 예: {"landmark":"황소상","score":0.97}
                string json = req.downloadHandler.text;
                Debug.Log($"[CaptureAndSend] Server response: {json}");

                // TODO: json 파싱해서 UI 가이드 매핑 호출
                // e.g., GuideUI.ShowFor(landmarkName);
            }
        }
    }

    // 선택: 카메라(혹은 손 앵커)의 포즈를 같이 보낼 때
    private string PoseToJson()
    {
        // 예시: XR Origin의 RightHand Anchor 포즈를 기록하고 싶다면
        // 여기서 해당 Transform을 Serialize
        var t = transform; // 필요 시 다른 Transform 참조
        var data = new
        {
            pos = new { x = t.position.x, y = t.position.y, z = t.position.z },
            rot = new { x = t.rotation.eulerAngles.x, y = t.rotation.eulerAngles.y, z = t.rotation.eulerAngles.z }
        };
        return JsonUtility.ToJson(data);
    }
}

