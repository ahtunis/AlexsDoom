using UnityEngine;

namespace AlexsDoom.Weapons
{
    public class RocketLauncher : WeaponBase
    {
        [Header("Projectile")]
        [SerializeField] private Rocket rocketPrefab;
        [SerializeField] private Transform firePoint;

        [Header("FX")]
        [SerializeField] private ParticleSystem muzzleFlash;

        protected override void Fire()
        {
            currentAmmo--;
            muzzleFlash?.Play();
            PlaySound(fireSound);

            if (rocketPrefab != null && firePoint != null)
                Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
