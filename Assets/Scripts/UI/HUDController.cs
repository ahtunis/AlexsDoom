using TMPro;
using UnityEngine;
using AlexsDoom.Player;

namespace AlexsDoom.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI armorText;
        [SerializeField] private TextMeshProUGUI ammoText;

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
        }

        private void UpdateHealth(int value) => healthText.text = $"HP: {value}";
        private void UpdateArmor(int value) => armorText.text = $"AR: {value}";
        public void UpdateAmmo(int current, int max) => ammoText.text = $"{current} / {max}";
    }
}
