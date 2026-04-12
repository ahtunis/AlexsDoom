using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Pickups
{
    public class HealthPickup : Pickup
    {
        [SerializeField] private int healAmount = 25;

        protected override void OnPickup(GameObject player)
        {
            player.GetComponent<PlayerHealth>()?.Heal(healAmount);
        }
    }
}
