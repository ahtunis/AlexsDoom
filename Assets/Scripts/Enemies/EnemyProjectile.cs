using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Enemies
{
    /// <summary>
    /// Fired by RangedEnemy. Requires a Rigidbody and a trigger Collider.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class EnemyProjectile : MonoBehaviour
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 5f;

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearVelocity = transform.forward * speed;
            GetComponent<Collider>().isTrigger = true;
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
