using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Enemies
{
    /// <summary>
    /// Fast, lightly armoured melee enemy. Charges at the player when within range.
    /// Recommended inspector values: maxHealth=30, damage=8, attackRange=1.8, detectionRange=25.
    /// </summary>
    public class ImpEnemy : EnemyBase
    {
        [Header("Charge")]
        [SerializeField] private float chargeRange = 9f;
        [SerializeField] private float chargeSpeed = 11f;
        [SerializeField] private float walkSpeed = 4f;

        protected override void Awake()
        {
            base.Awake();
            Agent.speed = walkSpeed;
        }

        protected override void Chase()
        {
            float dist = Vector3.Distance(transform.position, Player.position);
            Agent.speed = dist <= chargeRange ? chargeSpeed : walkSpeed;
            Agent.SetDestination(Player.position);
        }

        protected override void Attack()
        {
            Player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
