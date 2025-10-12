using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnFollowCamera : MonoBehaviour
{
    [Header("References")]
    [Tooltip("오른손 컨트롤러 하위의 빈 오브젝트 (CamFollowAnchor)")]
    public Transform followTarget;

    [Tooltip("소환할 카메라 프리팹")]
    public GameObject cameraPrefab;

    [Tooltip("A 버튼 (Right primaryButton)에 바인딩된 InputActionReference")]
    public InputActionReference toggleAction;

    [Header("Follow Options")]
    [Tooltip("부드럽게 따라가고 싶으면 > 0으로 설정 (초당 보간 비율)")]
    public float followLerp = 0f;

    private GameObject _spawned;
    private bool _isShown = false;

    void OnEnable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed += OnTogglePerformed;
            toggleAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (toggleAction != null)
        {
            toggleAction.action.performed -= OnTogglePerformed;
            toggleAction.action.Disable();
        }
    }

    private void OnTogglePerformed(InputAction.CallbackContext ctx)
    {
        ToggleCamera();
    }

    public void ToggleCamera()
    {
        if (!_isShown)
        {
            ShowCamera();
        }
        else
        {
            HideCamera();
        }
    }

    private void ShowCamera()
    {
        if (cameraPrefab == null || followTarget == null) return;

        // 프리팹 인스턴스 생성 후, 앵커의 자식으로 두면 포즈 유지가 가장 간단합니다.
        _spawned = Instantiate(cameraPrefab, followTarget.position, followTarget.rotation);
        _spawned.transform.SetParent(followTarget, worldPositionStays: true);

        // 로컬 기준으로 정확히 포개고 싶으면 다음 두 줄 사용:
        _spawned.transform.localPosition = Vector3.zero;
        _spawned.transform.localRotation = Quaternion.identity;

        _isShown = true;
    }

    private void HideCamera()
    {
        if (_spawned != null)
        {
            Destroy(_spawned);
            _spawned = null;
        }
        _isShown = false;
    }

    void LateUpdate()
    {
        // 부모-자식으로 묶었기 때문에 보통 별도 갱신이 필요 없습니다.
        // 만약 부모-자식으로 두고 싶지 않거나, 부드러운 보간을 쓰고 싶다면 아래를 사용하세요.
        if (_isShown && _spawned != null && followTarget != null && followLerp > 0f)
        {
            _spawned.transform.position = Vector3.Lerp(
                _spawned.transform.position, followTarget.position, 1f - Mathf.Exp(-followLerp * Time.deltaTime));
            _spawned.transform.rotation = Quaternion.Slerp(
                _spawned.transform.rotation, followTarget.rotation, 1f - Mathf.Exp(-followLerp * Time.deltaTime));
        }
    }
}

