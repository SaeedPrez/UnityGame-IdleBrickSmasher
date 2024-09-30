using UnityEngine;
using UnityEngine.InputSystem;

namespace Prez.Core
{
    public class InputManager : MonoBehaviour
    {
        private InputSystemActions _input;

        private void Awake()
        {
            _input = new InputSystemActions();
        }

        private void OnEnable()
        {
            _input.Player.Enable();
            _input.Player.Move.performed += OnPlayerMovePerformed;
            _input.Player.Move.canceled += OnPlayerMovePerformed;
            _input.Player.ReleaseBall.performed += OnPlayerReleaseBallPerformed;
        }
        
        private void OnDisable()
        {
            _input.Player.Disable();
            _input.Player.Move.performed -= OnPlayerMovePerformed;
            _input.Player.Move.canceled -= OnPlayerMovePerformed;
            _input.Player.ReleaseBall.performed -= OnPlayerReleaseBallPerformed;
        }
        
        private void OnPlayerMovePerformed(InputAction.CallbackContext context)
        {
            EventManager.I.RaisePlayerInput(context.ReadValue<Vector2>());
        }
        
        private void OnPlayerReleaseBallPerformed(InputAction.CallbackContext context)
        {
            EventManager.I.RaisePlayerInputReleaseBall();
        }

    }
}