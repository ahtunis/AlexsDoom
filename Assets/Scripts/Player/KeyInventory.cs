using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AlexsDoom.Level;

namespace AlexsDoom.Player
{
    /// <summary>
    /// Tracks which key cards the player is carrying.
    /// </summary>
    public class KeyInventory : MonoBehaviour
    {
        public UnityEvent<KeyCardColor> OnKeyCollected;

        private readonly HashSet<KeyCardColor> _keys = new HashSet<KeyCardColor>();

        public void AddKey(KeyCardColor color)
        {
            if (_keys.Add(color))
                OnKeyCollected?.Invoke(color);
        }

        public bool HasKey(KeyCardColor color) => _keys.Contains(color);
    }
}
