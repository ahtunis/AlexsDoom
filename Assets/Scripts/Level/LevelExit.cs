using UnityEngine;
using AlexsDoom.UI;

namespace AlexsDoom.Level
{
    /// <summary>
    /// Place a trigger Collider on this GameObject. When the player walks through,
    /// it either shows the LevelEndPanel (if assigned) or loads the next scene directly.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class LevelExit : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;
        [SerializeField] private LevelEndPanel levelEndPanel;

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (levelEndPanel != null)
            {
                float elapsed = GameManager.Instance != null
                    ? Time.time - GameManager.Instance.LevelStartTime
                    : 0f;

                int kills = GameManager.Instance != null
                    ? GameManager.Instance.EnemiesKilled
                    : 0;

                levelEndPanel.Show(kills, elapsed, nextSceneName);
            }
            else
            {
                GameManager.Instance?.LoadScene(nextSceneName);
            }
        }
    }
}
