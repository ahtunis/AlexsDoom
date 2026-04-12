using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Enemies
{
    /// <summary>
    /// Slow, heavy melee enemy with a ground-slam AOE attack.
    /// Recommended inspector values: maxHealth=200, damage=25, attackRange=3, detectionRange=20.
    /// </summary>
    public class DemonEnemy : EnemyBase
    {
        [Header("Slam")]
        [SerializeField] private float slamRadius = 3f;
        [SerializeField] private float walkSpeed = 2f;

        [Header("FX")]
        [SerializeField] private ParticleSystem slamFX;

        protected override void Awake()
        {
            base.Awake();
            Agent.speed = walkSpeed;
        }

        protected override void Attack()
        {
            slamFX?.Play();

            // AOE — damages all players in slam radius (handles multiplayer too)
            Collider[] hits = Physics.OverlapSphere(transform.position, slamRadius);
            foreach (var hit in hits)
            {
                var ph = hit.GetComponentInParent<PlayerHealth>();
                ph?.TakeDamage(damage);
            }
        }
    }
}
