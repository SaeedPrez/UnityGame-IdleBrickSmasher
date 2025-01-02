using UnityEngine;
using UnityEngine.InputSystem;

namespace Prez.V2.Managers
{
    public class InputManager : MonoBehaviour
    {
        private EventManager _event;
        private InputSystemActions _input;

        #region Bootstrap

        private void Awake()
        {
            _event = RefManager.Event;
            _input = new InputSystemActions();
        }

        private void OnEnable()
        {
            _input.Player.Enable();
            _input.Player.Move.performed += OnPlayerMovePerformed;
            _input.Player.Move.canceled += OnPlayerMovePerformed;
            _input.Player.BallAction1.performed += OnPlayerBallAction1Performed;
        }

        private void OnDisable()
        {
            _input.Player.Disable();
            _input.Player.Move.performed -= OnPlayerMovePerformed;
            _input.Player.Move.canceled -= OnPlayerMovePerformed;
            _input.Player.BallAction1.performed -= OnPlayerBallAction1Performed;
        }

        #endregion

        private void OnPlayerMovePerformed(InputAction.CallbackContext context)
        {
            _event.TriggerPlayerInput(context.ReadValue<Vector2>());
        }

        private void OnPlayerBallAction1Performed(InputAction.CallbackContext context)
        {
            _event.TriggerPlayerBallAction1();
        }
        
    }
}