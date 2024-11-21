using System;
using Photon.Pun;
using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        
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
            if(!_photonView.IsMine)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                int weaponIndex = Input.GetKeyDown(KeyCode.Alpha1) ? 0 : 1;
                _photonView.RPC(nameof(HandleWeaponSwitching), RpcTarget.AllBuffered, weaponIndex);
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space))
            {
                _photonView.RPC(nameof(HandleShooting), RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
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

        [PunRPC]
        private void HandleWeaponSwitching(int index)
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
            HandleWeaponSwitching(0);
        }

        #endregion
    }
}