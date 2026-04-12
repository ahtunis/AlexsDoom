using UnityEngine;

namespace AlexsDoom.Level
{
    /// <summary>
    /// Place a trigger Collider on this GameObject. When the player walks through,
    /// it loads the next scene via GameManager.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class LevelExit : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            GameManager.Instance?.LoadScene(nextSceneName);
        }
    }
}
