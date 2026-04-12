using UnityEngine;
using UnityEngine.UI;
using AlexsDoom.Player;

namespace AlexsDoom.UI
{
    /// <summary>
    /// Full-screen red flash on damage. Requires a UI Image on the same GameObject
    /// (stretched to fill the screen, Raycast Target OFF).
    /// Self-subscribes to PlayerHealth — no wiring needed beyond adding to the Canvas.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.45f);
        [SerializeField] private float flashDuration = 0.35f;

        private Image _image;
        private float _flashTimer;
        private int _prevHealth;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = Color.clear;
            _image.raycastTarget = false;
        }

        private void Start()
        {
            var ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null)
            {
                _prevHealth = ph.CurrentHealth;
                ph.OnHealthChanged.AddListener(OnHealthChanged);
            }
        }

        private void OnHealthChanged(int newHealth)
        {
            if (newHealth < _prevHealth)
            {
                _flashTimer = flashDuration;
                _image.color = flashColor;
            }
            _prevHealth = newHealth;
        }

        private void Update()
        {
            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                float alpha = Mathf.Clamp01(_flashTimer / flashDuration) * flashColor.a;
                _image.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            }
        }
    }
}
