using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchObjects : MonoBehaviour
{
    [Header("Objects to Toggle")]
    public GameObject objectA;
    public GameObject objectB;

    [Header("Input")]
    public InputActionReference toggleAction; // Left X ¹öÆ°

    private bool isAActive = true;

   
    public static event Action OnSwitched;

    private void OnEnable()
    {
        toggleAction.action.performed += OnToggle;
        toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleAction.action.performed -= OnToggle;
        toggleAction.action.Disable();
    }

    private void Start()
    {
        SetActiveState(isAActive);
    }

    private void OnToggle(InputAction.CallbackContext ctx)
    {
        isAActive = !isAActive;
        SetActiveState(isAActive);

      
        OnSwitched?.Invoke();
    }

    private void SetActiveState(bool aActive)
    {
        if (objectA) objectA.SetActive(aActive);
        if (objectB) objectB.SetActive(!aActive);
    }
}


