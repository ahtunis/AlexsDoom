using UnityEngine;

namespace AlexsDoom.Enemies
{
    /// <summary>
    /// Keeps distance from the player and lobs projectiles.
    /// </summary>
    public class RangedEnemy : EnemyBase
    {
        [Header("Ranged")]
        [SerializeField] private EnemyProjectile projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float preferredRange = 10f;

        protected override void Update()
        {
            if (Player == null) return;

            float dist = Vector3.Distance(transform.position, Player.position);

            if (dist <= detectionRange)
            {
                // Back away if too close, otherwise hold position and face player
                if (dist < preferredRange)
                    Agent.SetDestination(transform.position + (transform.position - Player.position).normalized * 2f);
                else
                    Agent.SetDestination(transform.position); // stay put

                transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
            }

            if (dist <= attackRange && Time.time >= NextAttackTime)
            {
                NextAttackTime = Time.time + attackCooldown;
                Attack();
            }
        }

        protected override void Attack()
        {
            if (projectilePrefab == null || firePoint == null) return;

            Vector3 dir = (Player.position + Vector3.up * 0.5f - firePoint.position).normalized;
            Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        }
    }
}
