using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexsDoom.Level
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string firstLevelScene = "Level01";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartGame() => SceneManager.LoadScene(firstLevelScene);

        public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        public void LoadMainMenu() => SceneManager.LoadScene(mainMenuScene);

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
