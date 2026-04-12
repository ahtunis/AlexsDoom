using UnityEngine;

namespace AlexsDoom.Weapons
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Weapon Stats")]
        [SerializeField] protected string weaponName;
        [SerializeField] protected int damage;
        [SerializeField] protected float fireRate = 0.5f;
        [SerializeField] protected int maxAmmo = 50;
        [SerializeField] protected int currentAmmo;

        [Header("Audio")]
        [SerializeField] protected AudioClip fireSound;
        [SerializeField] protected AudioClip emptySound;

        protected AudioSource AudioSource;
        private float _nextFireTime;

        public int CurrentAmmo => currentAmmo;
        public int MaxAmmo => maxAmmo;
        public string WeaponName => weaponName;

        protected virtual void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            currentAmmo = maxAmmo;
        }

        public virtual void TryFire()
        {
            if (Time.time < _nextFireTime) return;

            if (currentAmmo <= 0)
            {
                PlaySound(emptySound);
                return;
            }

            _nextFireTime = Time.time + fireRate;
            Fire();
        }

        protected abstract void Fire();

        public virtual void AddAmmo(int amount)
        {
            currentAmmo = Mathf.Min(maxAmmo, currentAmmo + amount);
        }

        protected void PlaySound(AudioClip clip)
        {
            if (AudioSource != null && clip != null)
                AudioSource.PlayOneShot(clip);
        }
    }
}
