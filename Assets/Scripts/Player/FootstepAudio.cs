using UnityEngine;

namespace AlexsDoom.Player
{
    /// <summary>
    /// Plays randomised footstep sounds while the player is grounded and moving.
    /// Requires an AudioSource on the same GameObject (separate from the weapon AudioSource).
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FootstepAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField] private float stepInterval = 0.42f;
        [SerializeField] private float minSpeed = 0.5f;

        private CharacterController _cc;
        private AudioSource _audio;
        private float _stepTimer;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _audio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            Vector3 flatVel = new Vector3(_cc.velocity.x, 0f, _cc.velocity.z);
            bool moving = _cc.isGrounded && flatVel.magnitude > minSpeed;

            if (moving)
            {
                _stepTimer -= Time.deltaTime;
                if (_stepTimer <= 0f)
                {
                    PlayStep();
                    _stepTimer = stepInterval;
                }
            }
            else
            {
                _stepTimer = 0f;
            }
        }

        private void PlayStep()
        {
            if (footstepClips == null || footstepClips.Length == 0) return;
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
            _audio.PlayOneShot(clip);
        }
    }
}
