using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

public class CaptureAndSend : MonoBehaviour
{
    [Header("Capture")]
    public RenderTexture renderTexture;            // 미니카메라가 출력 중인 RT (필수)

    [Header("Input")]
    public InputActionReference sendAction;        // B 버튼

    [Header("Server")]
    public string serverUrl = "http://127.0.0.1:8000/detect";

    [Header("UI Spawning")]
    public Transform uiPoint;                      // XR Origin 하위 UIpoint
    public GameObject guideUIPrefab;               // GuideUI 프리팹

    private GuideUIController waitingUI;           // 분석중 UI
    private GuideUIController resultUI;            // 결과 UI
    private bool _busy = false;

    private Texture2D _readTex;

    [Serializable]
    public class DetectResponse { public string label; }

    [Header("Guide Contents")]
    public GuideUIController.GuideContent bullContent;
    public GuideUIController.GuideContent disneyContent;
    public GuideUIController.GuideContent unknownContent;

    private void OnEnable()
    {
        sendAction.action.performed += OnSendPerformed;
        sendAction.action.Enable();

       
        SwitchObjects.OnSwitched += ClearAllUIs;
    }

    private void OnDisable()
    {
        sendAction.action.performed -= OnSendPerformed;
        sendAction.action.Disable();

        SwitchObjects.OnSwitched -= ClearAllUIs;
    }

    private void OnSendPerformed(InputAction.CallbackContext _)
    {
        if (_busy) return;

        SpawnWaitingUI();
        StartCoroutine(CaptureAndUpload());
    }

    private void SpawnWaitingUI()
    {
        ClearAllUIs(); // 혹시 남아있던 UI 정리
        var go = Instantiate(guideUIPrefab, uiPoint.position, uiPoint.rotation);
        waitingUI = go.GetComponent<GuideUIController>();
        waitingUI.ShowWaiting(); // "결과를 기다리는 중..."
    }

    private IEnumerator CaptureAndUpload()
    {
        _busy = true;

        // --- 캡처 ---
        var prev = RenderTexture.active;
        RenderTexture.active = renderTexture;

        if (_readTex == null ||
            _readTex.width != renderTexture.width || _readTex.height != renderTexture.height)
        {
            if (_readTex != null) Destroy(_readTex);
            _readTex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        }

        _readTex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        _readTex.Apply();
        RenderTexture.active = prev;

        // --- 전송 ---
        byte[] png = _readTex.EncodeToPNG();
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", png, "capture.png", "image/png");

        using (var req = UnityWebRequest.Post(serverUrl, form))
        {
            req.timeout = 120;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                ShowResultUI(unknownContent);
            }
            else
            {
                ApplyLabel(req.downloadHandler.text);
            }
        }

        _busy = false;
    }

    private void ApplyLabel(string json)
    {
        try
        {
            var resp = JsonUtility.FromJson<DetectResponse>(json);
            var label = resp?.label ?? "";
            var content =
                label == "bull" ? bullContent :
                label == "disney" ? disneyContent :
                unknownContent;

            ShowResultUI(content);
        }
        catch
        {
            ShowResultUI(unknownContent);
        }
    }

    private void ShowResultUI(GuideUIController.GuideContent content)
    {
        // 분석중 UI 위치/회전 재사용 (없으면 uiPoint)
        Vector3 pos = waitingUI ? waitingUI.transform.position : uiPoint.position;
        Quaternion rot = waitingUI ? waitingUI.transform.rotation : uiPoint.rotation;

        if (waitingUI) Destroy(waitingUI.gameObject);
        if (resultUI) Destroy(resultUI.gameObject);

        var go = Instantiate(guideUIPrefab, pos, rot);
        resultUI = go.GetComponent<GuideUIController>();
        resultUI.ShowResult(content);
    }

    private void ClearAllUIs()
    {
        if (waitingUI) Destroy(waitingUI.gameObject);
        if (resultUI) Destroy(resultUI.gameObject);
        waitingUI = null;
        resultUI = null;
    }
}






