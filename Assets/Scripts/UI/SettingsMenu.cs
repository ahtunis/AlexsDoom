using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AlexsDoom.Level;

namespace AlexsDoom.UI
{
    /// <summary>
    /// Settings panel — can live inside the PauseMenu or the Main Menu.
    /// Wire the three sliders and (optionally) label texts in the inspector.
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Value Labels (optional)")]
        [SerializeField] private TextMeshProUGUI sensitivityLabel;
        [SerializeField] private TextMeshProUGUI musicLabel;
        [SerializeField] private TextMeshProUGUI sfxLabel;

        private void OnEnable()
        {
            // Populate sliders from saved values when the panel opens
            if (sensitivitySlider != null) sensitivitySlider.value = GameSettings.MouseSensitivity;
            if (musicVolumeSlider != null) musicVolumeSlider.value = GameSettings.MusicVolume;
            if (sfxVolumeSlider   != null) sfxVolumeSlider.value   = GameSettings.SFXVolume;
            RefreshLabels();
        }

        public void OnSensitivityChanged(float value)
        {
            GameSettings.MouseSensitivity = value;
            RefreshLabels();
        }

        public void OnMusicVolumeChanged(float value)
        {
            GameSettings.MusicVolume = value;
            AudioManager.Instance?.SetVolume(value);
            RefreshLabels();
        }

        public void OnSFXVolumeChanged(float value)
        {
            GameSettings.SFXVolume = value;
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            if (sensitivityLabel != null) sensitivityLabel.text = $"{GameSettings.MouseSensitivity:F1}";
            if (musicLabel != null)       musicLabel.text       = $"{GameSettings.MusicVolume * 100f:F0}%";
            if (sfxLabel != null)         sfxLabel.text         = $"{GameSettings.SFXVolume   * 100f:F0}%";
        }
    }
}
