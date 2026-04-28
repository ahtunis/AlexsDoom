using UnityEngine;
using AlexsDoom.UI;

namespace AlexsDoom.Weapons
{
    /// <summary>
    /// Sits on the Player. Handles fire input, weapon switching (scroll / 1-9 keys),
    /// and keeps the HUD ammo display in sync.
    /// </summary>
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private WeaponBase[] weapons;
        [SerializeField] private HUDController hud;

        private int _currentIndex;

        private WeaponBase CurrentWeapon =>
            weapons != null && weapons.Length > 0 ? weapons[_currentIndex] : null;

        private void Start()
        {
            EquipWeapon(0);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
                CurrentWeapon?.TryFire();

            HandleWeaponSwitch();

            if (CurrentWeapon != null)
                hud?.UpdateAmmo(CurrentWeapon.CurrentAmmo, CurrentWeapon.MaxAmmo);
        }

        private void HandleWeaponSwitch()
        {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll > 0f)
                EquipWeapon((_currentIndex + 1) % weapons.Length);
            else if (scroll < 0f)
                EquipWeapon((_currentIndex - 1 + weapons.Length) % weapons.Length);

            int limit = Mathf.Min(weapons.Length, 9);
            for (int i = 0; i < limit; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    EquipWeapon(i);
            }
        }

        private void EquipWeapon(int index)
        {
            if (weapons == null || weapons.Length == 0) return;
            foreach (var w in weapons)
                if (w != null) w.gameObject.SetActive(false);

            _currentIndex = index;
            if (weapons[_currentIndex] != null)
                weapons[_currentIndex].gameObject.SetActive(true);
        }
    }
}
