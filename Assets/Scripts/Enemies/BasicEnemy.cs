using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Enemies
{
    /// <summary>
    /// Melee enemy — chases the player and deals damage on contact.
    /// </summary>
    public class BasicEnemy : EnemyBase
    {
        protected override void Attack()
        {
            var health = Player.GetComponent<PlayerHealth>();
            health?.TakeDamage(damage);
        }
    }
}
