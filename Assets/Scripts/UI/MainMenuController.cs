using UnityEngine;
using AlexsDoom.Level;

namespace AlexsDoom.UI
{
    /// <summary>
    /// Attach to a Canvas in the MainMenu scene.
    /// Wire Play and Quit buttons to the public methods.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        public void OnPlayButton() => GameManager.Instance?.StartGame();
        public void OnQuitButton() => GameManager.Instance?.QuitGame();
    }
}
