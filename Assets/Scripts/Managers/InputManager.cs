using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private InputActions m_inputActions;

    // Input actions
    private InputAction m_playerMovementInput;
    private InputAction m_playerAttackInput;

    protected override void Awake()
    {
        base.Awake();
        InitializeInputActions();
    }

    private void InitializeInputActions()
    {
        m_inputActions = new InputActions();

        m_playerMovementInput = m_inputActions.GameInput.Movement;
        m_playerMovementInput.Enable();

        m_playerAttackInput = m_inputActions.GameInput.Attack;
        m_playerAttackInput.Enable();
    }

    public InputAction PlayerMoveInput()
    {
        return m_playerMovementInput;
    }

    public InputAction PlayerAttackInput()
    {
        return m_playerAttackInput;
    }

    public Vector2 GetMovementVector2Normalized()
    {
        Vector2 movement = m_playerMovementInput.ReadValue<Vector2>();
        return movement;
    }
}
