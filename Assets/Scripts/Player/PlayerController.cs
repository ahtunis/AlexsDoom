using UnityEngine;
using AlexsDoom.Level;

namespace AlexsDoom.Player
{
    /// <summary>
    /// Handles first-person player movement and look — Doom-style.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float gravity = -20f;

        [Header("Look")]
        [SerializeField] private float mouseSensitivity = 2f; // overridden at runtime by GameSettings
        [SerializeField] private Transform cameraRoot;

        private CharacterController _cc;
        private Vector3 _velocity;
        private float _verticalLook;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            mouseSensitivity = GameSettings.MouseSensitivity;
        }

        private void Update()
        {
            HandleMovement();
            HandleLook();
        }

        private void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 move = transform.right * h + transform.forward * v;
            move = move.normalized * moveSpeed;

            if (_cc.isGrounded && _velocity.y < 0f)
                _velocity.y = -2f;

            _velocity.y += gravity * Time.deltaTime;
            _cc.Move((move + new Vector3(0, _velocity.y, 0)) * Time.deltaTime);
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            _verticalLook -= mouseY;
            _verticalLook = Mathf.Clamp(_verticalLook, -80f, 80f);
            if (cameraRoot != null)
                cameraRoot.localRotation = Quaternion.Euler(_verticalLook, 0f, 0f);
        }
    }
}
