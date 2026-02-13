using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityTutorial.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField, Range(10, 500)] private float JumpFactor = 260f;
        [SerializeField] private float Dis2Ground = 0.8f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;
        
        private Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        private Animator _animator;
        private bool _grounded = false;
        private bool _hasAnimator;
        
        private int _xVelHash;
        private int _yVelHash;
        private int _jumpHash;
        private int _groundedHash;
        private int _fallingHash;
        
        private float _xRotation;
        private const float _walkSpeed = 2f;
        private const float _runSpeed = 6f;
        private Vector2 _currentVelocity;
        
        void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();
            
            _xVelHash = Animator.StringToHash("XVelocity");
            _yVelHash = Animator.StringToHash("YVelocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundedHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        
        private void FixedUpdate()
        {
            SampleGround();
            Move();
            HandleJump();
        }
        
        private void LateUpdate()
        {
            CamMovements();
        }
        
        private void Move()
        {
            if (!_hasAnimator) return;

            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
            if (_inputManager.Move == Vector2.zero) targetSpeed = 0;

            _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
            _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

            if (_grounded)
            {
                var xVelDifference = _currentVelocity.x - _playerRigidbody.linearVelocity.x;
                var zVelDifference = _currentVelocity.y - _playerRigidbody.linearVelocity.z;

                _playerRigidbody.AddForce(transform.TransformDirection(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);
            }
            else
            {
                _playerRigidbody.AddForce(transform.TransformDirection(new Vector3(_currentVelocity.x * AirResistance, 0, _currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
            }

            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }
        
        private void CamMovements()
        {
            if (!_hasAnimator) return;
            if (Cursor.lockState != CursorLockMode.Locked) return;
            
            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;
            Camera.position = CameraRoot.position;
            
            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
        }
        
        private void HandleJump()
        {
            if (!_hasAnimator) return;
            if (!_inputManager.Jump) return;
            if (!_grounded) return;
            
            _animator.SetTrigger(_jumpHash);
            _inputManager.ResetJump();
        }

        public void JumpAddForce()
        {
            // Called from Animation Event in Jump animation
            _playerRigidbody.AddForce(-_playerRigidbody.linearVelocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            _animator.ResetTrigger(_jumpHash);
        }

        public void OnLand()
        {
            // Called from Animation Event in Landing animation
            // This is called when the landing animation completes
            Debug.Log("Landed!");
        }

        private void SampleGround()
        {
            if (!_hasAnimator) return;
            
            RaycastHit hitInfo;
            
            if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, Dis2Ground + 0.1f, GroundCheck))
            {
                _grounded = true;
            }
            else
            {
                _grounded = false;
            }
            
            // Update animator states
            _animator.SetBool(_groundedHash, _grounded);
            _animator.SetBool(_fallingHash, !_grounded);
        }
    }
}