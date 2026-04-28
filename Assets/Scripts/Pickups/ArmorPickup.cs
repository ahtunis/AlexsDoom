using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Pickups
{
    public class ArmorPickup : Pickup
    {
        [SerializeField] private int armorAmount = 50;

        protected override void OnPickup(GameObject player)
        {
            player.GetComponent<PlayerHealth>()?.AddArmor(armorAmount);
        }
    }
}
