using UnityEngine;
using UnityEngine.Events;

namespace AlexsDoom.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int maxArmor = 200;

        public int CurrentHealth { get; private set; }
        public int CurrentArmor { get; private set; }

        public UnityEvent<int> OnHealthChanged;
        public UnityEvent<int> OnArmorChanged;
        public UnityEvent OnDeath;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            CurrentArmor = 0;
        }

        public void TakeDamage(int amount)
        {
            if (CurrentArmor > 0)
            {
                int absorbed = Mathf.Min(CurrentArmor, amount / 2);
                CurrentArmor -= absorbed;
                amount -= absorbed;
                OnArmorChanged?.Invoke(CurrentArmor);
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth);

            if (CurrentHealth <= 0)
                OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        public void AddArmor(int amount)
        {
            CurrentArmor = Mathf.Min(maxArmor, CurrentArmor + amount);
            OnArmorChanged?.Invoke(CurrentArmor);
        }
    }
}
