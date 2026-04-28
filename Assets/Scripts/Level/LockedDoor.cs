using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Level
{
    /// <summary>
    /// A Door that requires a specific KeyCard colour to open.
    /// Plays a "locked" sound cue if the player interacts without the right key.
    /// </summary>
    public class LockedDoor : Door
    {
        [Header("Lock")]
        [SerializeField] private KeyCardColor requiredKey;
        [SerializeField] private AudioClip lockedSound;

        private bool _unlocked;
        private AudioSource _audio;

        protected override void Awake()
        {
            base.Awake();
            _audio = GetComponent<AudioSource>();
        }

        protected override void Update()
        {
            if (_unlocked)
            {
                base.Update();
                return;
            }

            if (!Input.GetKeyDown(KeyCode.E)) return;

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            if (Vector3.Distance(transform.position, player.transform.position) > 3f) return;

            var inv = player.GetComponent<KeyInventory>();
            if (inv != null && inv.HasKey(requiredKey))
            {
                _unlocked = true;
                ForceOpen();
            }
            else
            {
                if (_audio != null && lockedSound != null)
                    _audio.PlayOneShot(lockedSound);
            }
        }
    }
}
