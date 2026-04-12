using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlexsDoom.Level
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string firstLevelScene = "Level01";

        public int EnemiesKilled { get; private set; }
        public float LevelStartTime { get; private set; }

        public event System.Action<int> OnKillCountChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LevelStartTime = Time.time;
        }

        public void RegisterKill()
        {
            EnemiesKilled++;
            OnKillCountChanged?.Invoke(EnemiesKilled);
        }

        public void StartGame() => SceneManager.LoadScene(firstLevelScene);

        public void RestartLevel()
        {
            EnemiesKilled = 0;
            LevelStartTime = Time.time;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void BeginLevel()
        {
            EnemiesKilled = 0;
            LevelStartTime = Time.time;
        }

        public void LoadMainMenu() => SceneManager.LoadScene(mainMenuScene);

        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

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
