using UnityEngine;

namespace AlexsDoom.Player
{
    /// <summary>
    /// Attach to the Player. Bobs the weapon holder child when moving.
    /// Assign the weapon holder Transform in the inspector.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class WeaponBob : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private float bobFrequency = 8f;
        [SerializeField] private float bobAmplitudeY = 0.05f;
        [SerializeField] private float bobAmplitudeX = 0.025f;
        [SerializeField] private float returnSpeed = 10f;

        private CharacterController _cc;
        private Vector3 _restPos;
        private float _bobTimer;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }

        private void Start()
        {
            if (weaponHolder != null)
                _restPos = weaponHolder.localPosition;
        }

        private void Update()
        {
            if (weaponHolder == null) return;

            Vector3 flatVel = new Vector3(_cc.velocity.x, 0f, _cc.velocity.z);
            bool moving = flatVel.magnitude > 0.2f && _cc.isGrounded;

            if (moving)
            {
                _bobTimer += Time.deltaTime * bobFrequency;
                Vector3 bob = new Vector3(
                    Mathf.Sin(_bobTimer * 0.5f) * bobAmplitudeX,
                    Mathf.Abs(Mathf.Sin(_bobTimer)) * bobAmplitudeY,
                    0f);
                weaponHolder.localPosition = Vector3.Lerp(
                    weaponHolder.localPosition,
                    _restPos + bob,
                    returnSpeed * Time.deltaTime);
            }
            else
            {
                _bobTimer = 0f;
                weaponHolder.localPosition = Vector3.Lerp(
                    weaponHolder.localPosition,
                    _restPos,
                    returnSpeed * Time.deltaTime);
            }
        }
    }
}
