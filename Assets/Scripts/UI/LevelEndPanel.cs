using System.Collections;
using TMPro;
using UnityEngine;
using AlexsDoom.Level;

namespace AlexsDoom.UI
{
    /// <summary>
    /// Shown by LevelExit when the player reaches the end of a level.
    /// Assign a panel with KillCount and TimeTaken TextMeshPro labels.
    /// Wire the Next Level and Main Menu buttons to the public methods.
    /// </summary>
    public class LevelEndPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI killCountText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private float autoAdvanceDelay = 0f; // 0 = wait for button

        private string _nextScene;

        private void Start()
        {
            panel?.SetActive(false);
        }

        public void Show(int kills, float elapsedSeconds, string nextScene)
        {
            _nextScene = nextScene;

            int minutes = Mathf.FloorToInt(elapsedSeconds / 60f);
            int seconds = Mathf.FloorToInt(elapsedSeconds % 60f);

            if (killCountText != null) killCountText.text = $"Kills: {kills}";
            if (timeText != null)      timeText.text       = $"Time:  {minutes:00}:{seconds:00}";

            panel?.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;

            if (autoAdvanceDelay > 0f)
                StartCoroutine(AutoAdvance(autoAdvanceDelay));
        }

        public void OnNextLevel()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.LoadScene(_nextScene);
        }

        public void OnMainMenu()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.LoadMainMenu();
        }

        private IEnumerator AutoAdvance(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            OnNextLevel();
        }
    }
}
