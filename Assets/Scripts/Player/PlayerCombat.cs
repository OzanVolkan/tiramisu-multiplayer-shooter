using System;
using Managers;
using Photon.Pun;
using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Photon")] 
        [SerializeField] private PhotonView _photonView;

        [Header("Weapons")]
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private WeaponBase[] _weaponInstances;

        private WeaponBase _currentWeapon;

        private void OnEnable() => EventManager.AddHandler(GameEvent.OnGameOver, new Action(OnGameOver));

        private void OnDisable() => EventManager.RemoveHandler(GameEvent.OnGameOver, new Action(OnGameOver));

        private void Start() => SetDefaultWeapon();

        private void Update()
        {
            if (!_photonView.IsMine)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                var weaponIndex = Input.GetKeyDown(KeyCode.Alpha1) ? 0 : 1;
                var weaponType = weaponIndex == 0 ? "Kar98!" : "AK-47!";

                _photonView.RPC(nameof(HandleWeaponSwitching), RpcTarget.AllBuffered, weaponIndex);

                Debug.Log($"The weapon has been switched to {weaponType}");
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
        }

        private void SetDefaultWeapon()
        {
            HandleWeaponSwitching(0);
        }

        #endregion

        private void OnGameOver() => enabled = false;
    }
}