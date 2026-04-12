using UnityEngine;
using AlexsDoom.Level;
using AlexsDoom.Player;

namespace AlexsDoom.UI
{
    /// <summary>
    /// Attach to the Canvas alongside HUDController.
    /// Assign a pausePanel GameObject — it will be toggled on Escape.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        private bool _paused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Don't allow pausing when the player is already dead
                var pc = FindFirstObjectByType<PlayerController>();
                if (pc != null && !pc.enabled) return;

                SetPaused(!_paused);
            }
        }

        public void SetPaused(bool paused)
        {
            _paused = paused;
            Time.timeScale = _paused ? 0f : 1f;
            Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
            pausePanel?.SetActive(_paused);
        }

        public void Resume() => SetPaused(false);

        public void RestartLevel()
        {
            SetPaused(false);
            GameManager.Instance?.RestartLevel();
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.LoadMainMenu();
        }

        public void QuitGame() => GameManager.Instance?.QuitGame();
    }
}
