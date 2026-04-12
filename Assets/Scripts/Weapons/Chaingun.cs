using UnityEngine;

namespace AlexsDoom.Weapons
{
    /// <summary>
    /// Rapid-fire hitscan weapon with a spin-up mechanic.
    /// Fire rate accelerates the longer you hold the trigger.
    /// </summary>
    public class Chaingun : WeaponBase
    {
        [Header("Raycast")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float range = 80f;
        [SerializeField] private LayerMask hitMask = ~0;

        [Header("Spin-up")]
        [SerializeField] private float slowFireRate = 0.25f;
        [SerializeField] private float fastFireRate = 0.07f;
        [SerializeField] private float spinUpDuration = 1.2f;
        [SerializeField] private float spinDownDuration = 0.6f;

        [Header("FX")]
        [SerializeField] private ParticleSystem muzzleFlash;

        private float _spinProgress; // 0 = cold, 1 = fully spun up
        private float _lastFireAttempt;

        private void Update()
        {
            // Spin down when not firing
            if (Time.time - _lastFireAttempt > 0.05f)
                _spinProgress = Mathf.MoveTowards(_spinProgress, 0f, Time.deltaTime / spinDownDuration);
        }

        public override void TryFire()
        {
            _spinProgress = Mathf.MoveTowards(_spinProgress, 1f, Time.deltaTime / spinUpDuration);
            _lastFireAttempt = Time.time;
            fireRate = Mathf.Lerp(slowFireRate, fastFireRate, _spinProgress);
            base.TryFire();
        }

        protected override void Fire()
        {
            currentAmmo--;
            muzzleFlash?.Play();
            PlaySound(fireSound);

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
                hit.collider.GetComponentInParent<Enemies.EnemyBase>()?.TakeDamage(damage);
        }
    }
}
