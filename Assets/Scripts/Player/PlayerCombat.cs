using System;
using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private WeaponBase[] _weaponInstances;

        private WeaponBase _currentWeapon;

        private int _weaponIndex;

        private void Start()
        {
            SetDefaultWeapon();
        }

        private void Update()
        {
            HandleWeaponSwitching();
            HandleShooting();
        }

        private void HandleShooting()
        {
            if (_currentWeapon == null) return;

            switch (_currentWeapon)
            {
                case Kar98 when Input.GetKeyDown(KeyCode.Space):
                case Ak47 when Input.GetKey(KeyCode.Space):
                    _currentWeapon.Fire();
                    break;
            }
        }

        #region WeaponSelection

        private void HandleWeaponSwitching()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && _weaponIndex != 0)
            {
                ActivateWeapon(0);
                Debug.Log("Kar98 selected!");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && _weaponIndex != 1)
            {
                ActivateWeapon(1);
                Debug.Log("AK-47 selected!");
            }
        }

        private void ActivateWeapon(int index)
        {
            if (index < 0 || index >= _weapons.Length) return;

            for (int i = 0; i < _weapons.Length; i++)
            {
                _weapons[i].SetActive(i == index);
                _currentWeapon = _weaponInstances[index];
            }

            _weaponIndex = index;
        }

        private void SetDefaultWeapon()
        {
            ActivateWeapon(0);
        }

        #endregion
    }
}