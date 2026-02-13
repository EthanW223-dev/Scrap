using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace UnityTutorial.PlayerControl
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput PlayerInput;
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool Jump { get; private set; }
        
        private InputActionMap _curentMap;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _runAction;
        private InputAction _jumpAction;
        
        private void Awake()
        {
            _curentMap = PlayerInput.currentActionMap;
            _moveAction = _curentMap.FindAction("Move");
            _lookAction = _curentMap.FindAction("Look");
            _runAction = _curentMap.FindAction("Run");
            _jumpAction = _curentMap.FindAction("Jump");
            
            _moveAction.performed += OnMove;
            _lookAction.performed += OnLook;
            _runAction.performed += OnRun;
            _jumpAction.performed += onJump;
            
            _moveAction.canceled += OnMove;
            _lookAction.canceled += OnLook;
            _runAction.canceled += OnRun;
            _jumpAction.canceled += onJump;
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }
        
        private void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }
        
        private void OnRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }
        
        private void onJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
        }
        
        public void ResetJump()
        {
            Jump = false;
        }
        
        private void OnEnable()
        {
            _curentMap.Enable();
        }
        
        private void OnDisable()
        {
            _curentMap.Disable();
        }
    }
}