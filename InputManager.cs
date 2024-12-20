using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    private InputAction _mousePositionAction;
    private InputAction _mouseAction;

    public static Vector2 MousePosition;
    public static bool WasLeftMouseButtonPressed;
    public static bool WasLeftMouseButtonReleased;
    public static bool IsLeftMousePressed;

    // Nota: código compatible con inputs desde PC y Android

    // Awake: obtención de la fuente de input del jugador
    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _mousePositionAction = PlayerInput.actions["MousePosition"];
        _mouseAction = PlayerInput.actions["Mouse"];
    }

    // Update: obtención de acciones del ratón / pantalla táctil
    private void Update()
    {
        MousePosition = _mousePositionAction.ReadValue<Vector2>();

        WasLeftMouseButtonPressed = _mouseAction.WasPressedThisFrame();
        WasLeftMouseButtonReleased = _mouseAction.WasReleasedThisFrame();
        IsLeftMousePressed = _mouseAction.IsPressed();
    }
}
