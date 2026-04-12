using UnityEngine;
using UnityEngine.AI;

namespace AlexsDoom.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected int maxHealth = 50;
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float attackRange = 2f;
        [SerializeField] protected float detectionRange = 20f;
        [SerializeField] protected float attackCooldown = 1.5f;

        protected int CurrentHealth;
        protected NavMeshAgent Agent;
        protected Transform Player;
        private float _nextAttackTime;

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            CurrentHealth = maxHealth;
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        protected virtual void Update()
        {
            if (Player == null) return;

            float dist = Vector3.Distance(transform.position, Player.position);

            if (dist <= detectionRange)
                Chase();

            if (dist <= attackRange && Time.time >= _nextAttackTime)
            {
                _nextAttackTime = Time.time + attackCooldown;
                Attack();
            }
        }

        protected virtual void Chase()
        {
            Agent.SetDestination(Player.position);
        }

        protected abstract void Attack();

        public virtual void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
                Die();
        }

        protected virtual void Die()
        {
            // Override for death FX, drops, etc.
            Destroy(gameObject);
        }
    }
}
