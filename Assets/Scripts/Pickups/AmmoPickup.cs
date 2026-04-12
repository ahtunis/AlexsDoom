using UnityEngine;
using AlexsDoom.Weapons;

namespace AlexsDoom.Pickups
{
    public class AmmoPickup : Pickup
    {
        [SerializeField] private int ammoAmount = 10;

        protected override void OnPickup(GameObject player)
        {
            foreach (var weapon in player.GetComponentsInChildren<WeaponBase>())
                weapon.AddAmmo(ammoAmount);
        }
    }
}
