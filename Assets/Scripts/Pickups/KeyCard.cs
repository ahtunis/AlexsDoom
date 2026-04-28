using UnityEngine;
using AlexsDoom.Level;
using AlexsDoom.Player;

namespace AlexsDoom.Pickups
{
    public class KeyCard : Pickup
    {
        [SerializeField] private KeyCardColor color;

        protected override void OnPickup(GameObject player)
        {
            player.GetComponent<KeyInventory>()?.AddKey(color);
        }
    }
}
