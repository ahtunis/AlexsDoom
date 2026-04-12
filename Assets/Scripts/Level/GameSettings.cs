using UnityEngine;

namespace AlexsDoom.Level
{
    /// <summary>
    /// Thin PlayerPrefs wrapper. Use anywhere — no MonoBehaviour needed.
    /// </summary>
    public static class GameSettings
    {
        private const string KeySensitivity = "MouseSensitivity";
        private const string KeyMusicVol    = "MusicVolume";
        private const string KeySFXVol      = "SFXVolume";

        public static float MouseSensitivity
        {
            get => PlayerPrefs.GetFloat(KeySensitivity, 2f);
            set { PlayerPrefs.SetFloat(KeySensitivity, Mathf.Clamp(value, 0.1f, 10f)); PlayerPrefs.Save(); }
        }

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(KeyMusicVol, 0.6f);
            set { PlayerPrefs.SetFloat(KeyMusicVol, Mathf.Clamp01(value)); PlayerPrefs.Save(); }
        }

        public static float SFXVolume
        {
            get => PlayerPrefs.GetFloat(KeySFXVol, 1f);
            set { PlayerPrefs.SetFloat(KeySFXVol, Mathf.Clamp01(value)); PlayerPrefs.Save(); }
        }
    }
}
