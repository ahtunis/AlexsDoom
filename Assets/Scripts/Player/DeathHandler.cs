using System.Collections;
using UnityEngine;
using AlexsDoom.Level;
using AlexsDoom.UI;

namespace AlexsDoom.Player
{
    /// <summary>
    /// Listens to PlayerHealth.OnDeath, disables player control, shows game-over UI,
    /// then restarts the level after a delay.
    /// </summary>
    [RequireComponent(typeof(PlayerHealth))]
    [RequireComponent(typeof(PlayerController))]
    public class DeathHandler : MonoBehaviour
    {
        [SerializeField] private float restartDelay = 3f;
        [SerializeField] private HUDController hud;

        private void Start()
        {
            GetComponent<PlayerHealth>().OnDeath.AddListener(HandleDeath);
        }

        private void HandleDeath()
        {
            GetComponent<PlayerController>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            hud?.ShowGameOver();
            StartCoroutine(RestartAfterDelay());
        }

        private IEnumerator RestartAfterDelay()
        {
            yield return new WaitForSeconds(restartDelay);
            GameManager.Instance?.RestartLevel();
        }
    }
}
