using TMPro;
using UnityEngine;
using AlexsDoom.Player;
using AlexsDoom.Level;

namespace AlexsDoom.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI armorText;
        [SerializeField] private TextMeshProUGUI ammoText;
        [SerializeField] private TextMeshProUGUI killCountText;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;

        private PlayerHealth _playerHealth;

        private void Start()
        {
            _playerHealth = FindObjectOfType<PlayerHealth>();
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged.AddListener(UpdateHealth);
                _playerHealth.OnArmorChanged.AddListener(UpdateArmor);
                UpdateHealth(_playerHealth.CurrentHealth);
                UpdateArmor(_playerHealth.CurrentArmor);
            }

            if (GameManager.Instance != null)
                GameManager.Instance.OnKillCountChanged += UpdateKillCount;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnKillCountChanged -= UpdateKillCount;
        }

        private void UpdateHealth(int value) => healthText.text = $"HP: {value}";
        private void UpdateArmor(int value) => armorText.text = $"AR: {value}";
        public void UpdateAmmo(int current, int max) => ammoText.text = $"{current} / {max}";
        public void UpdateKillCount(int kills) { if (killCountText != null) killCountText.text = $"Kills: {kills}"; }

        public void ShowGameOver()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }
}
