using UnityEngine;

namespace AlexsDoom.Pickups
{
    /// <summary>
    /// Base class for all pickups. Bobs in place and triggers on player contact.
    /// Requires a trigger Collider on the GameObject.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class Pickup : MonoBehaviour
    {
        [Header("Bob")]
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.15f;
        [SerializeField] private float rotateSpeed = 90f;

        private Vector3 _startPos;

        private void Start()
        {
            _startPos = transform.position;
            GetComponent<Collider>().isTrigger = true;
        }

        private void Update()
        {
            transform.position = _startPos + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            OnPickup(other.gameObject);
            Destroy(gameObject);
        }

        protected abstract void OnPickup(GameObject player);
    }
}
