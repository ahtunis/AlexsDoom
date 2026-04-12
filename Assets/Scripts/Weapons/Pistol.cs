using UnityEngine;
using AlexsDoom.Weapons;

namespace AlexsDoom.Weapons
{
    public class Pistol : WeaponBase
    {
        [Header("Raycast")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float range = 100f;
        [SerializeField] private LayerMask hitMask = ~0;

        [Header("FX")]
        [SerializeField] private ParticleSystem muzzleFlash;

        protected override void Fire()
        {
            currentAmmo--;

            if (muzzleFlash != null)
                muzzleFlash.Play();

            PlaySound(fireSound);

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask))
            {
                var enemy = hit.collider.GetComponentInParent<Enemies.EnemyBase>();
                enemy?.TakeDamage(damage);
            }
        }
    }
}
