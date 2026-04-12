using UnityEngine;

namespace AlexsDoom.Weapons
{
    public class Shotgun : WeaponBase
    {
        [Header("Spread")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private int pellets = 8;
        [SerializeField] private float spreadDegrees = 5f;
        [SerializeField] private float range = 50f;
        [SerializeField] private LayerMask hitMask = ~0;

        [Header("FX")]
        [SerializeField] private ParticleSystem muzzleFlash;

        protected override void Fire()
        {
            currentAmmo--;

            if (muzzleFlash != null)
                muzzleFlash.Play();

            PlaySound(fireSound);

            for (int i = 0; i < pellets; i++)
            {
                float halfSpread = spreadDegrees * 0.5f;
                Vector3 spread = new Vector3(
                    Random.Range(-halfSpread, halfSpread),
                    Random.Range(-halfSpread, halfSpread),
                    0f);

                Quaternion rotation = Quaternion.Euler(spread) * playerCamera.transform.rotation;
                Vector3 dir = rotation * Vector3.forward;

                if (Physics.Raycast(playerCamera.transform.position, dir, out RaycastHit hit, range, hitMask))
                {
                    var enemy = hit.collider.GetComponentInParent<Enemies.EnemyBase>();
                    enemy?.TakeDamage(damage);
                }
            }
        }
    }
}
