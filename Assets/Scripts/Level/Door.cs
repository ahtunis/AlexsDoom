using UnityEngine;

namespace AlexsDoom.Level
{
    /// <summary>
    /// Classic Doom-style sliding door. Slides up when the player presses E within range,
    /// then auto-closes after a delay. Attach to any door mesh GameObject.
    /// </summary>
    public class Door : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float openHeight = 3f;
        [SerializeField] private float openSpeed = 3f;

        [Header("Behaviour")]
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private float autoCloseDelay = 5f;

        [Header("Audio")]
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;

        private enum State { Closed, Opening, Open, Closing }

        private State _state = State.Closed;
        private Vector3 _closedPos;
        private Vector3 _openPos;
        private float _closeTimer;
        private Transform _player;
        private AudioSource _audio;

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            _closedPos = transform.position;
            _openPos = _closedPos + Vector3.up * openHeight;
        }

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            HandleInput();
            Slide();
            TickAutoClose();
        }

        private void HandleInput()
        {
            if (_state != State.Closed || _player == null) return;
            if (Vector3.Distance(transform.position, _player.position) > interactRange) return;
            if (Input.GetKeyDown(KeyCode.E))
                BeginOpen();
        }

        private void BeginOpen()
        {
            _state = State.Opening;
            PlaySound(openSound);
        }

        private void BeginClose()
        {
            _state = State.Closing;
            PlaySound(closeSound);
        }

        private void Slide()
        {
            if (_state == State.Opening)
            {
                transform.position = Vector3.MoveTowards(transform.position, _openPos, openSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, _openPos) < 0.01f)
                {
                    transform.position = _openPos;
                    _state = State.Open;
                    _closeTimer = autoCloseDelay;
                }
            }
            else if (_state == State.Closing)
            {
                transform.position = Vector3.MoveTowards(transform.position, _closedPos, openSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, _closedPos) < 0.01f)
                {
                    transform.position = _closedPos;
                    _state = State.Closed;
                }
            }
        }

        private void TickAutoClose()
        {
            if (_state != State.Open) return;
            _closeTimer -= Time.deltaTime;
            if (_closeTimer <= 0f)
                BeginClose();
        }

        private void PlaySound(AudioClip clip)
        {
            if (_audio != null && clip != null)
                _audio.PlayOneShot(clip);
        }

        /// <summary>Called by LockedDoor subclass once the key requirement is satisfied.</summary>
        protected void ForceOpen() => BeginOpen();
    }
}
