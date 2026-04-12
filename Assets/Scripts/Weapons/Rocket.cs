using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.Weapons
{
    /// <summary>
    /// Fired by RocketLauncher. Travels forward, explodes on impact with radial damage.
    /// Does NOT use a trigger — uses OnCollisionEnter so it reacts to world geometry too.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Rocket : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private int directDamage = 100;
        [SerializeField] private int maxSplashDamage = 80;
        [SerializeField] private float splashRadius = 5f;

        [Header("Movement")]
        [SerializeField] private float speed = 22f;
        [SerializeField] private float lifetime = 8f;

        [Header("FX")]
        [SerializeField] private GameObject explosionFXPrefab;

        private bool _exploded;

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision col)
        {
            if (_exploded) return;
            Explode();
        }

        private void Explode()
        {
            _exploded = true;

            if (explosionFXPrefab != null)
                Instantiate(explosionFXPrefab, transform.position, Quaternion.identity);

            Collider[] hits = Physics.OverlapSphere(transform.position, splashRadius);
            foreach (var hit in hits)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                float falloff = 1f - Mathf.Clamp01(dist / splashRadius);
                int splashAmt = Mathf.RoundToInt(maxSplashDamage * falloff);

                var enemy = hit.GetComponentInParent<Enemies.EnemyBase>();
                if (enemy != null)
                    enemy.TakeDamage(splashAmt);

                var player = hit.GetComponentInParent<PlayerHealth>();
                if (player != null)
                    player.TakeDamage(splashAmt);
            }

            Destroy(gameObject);
        }
    }
}
