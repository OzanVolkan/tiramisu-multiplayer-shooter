using System;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        #region PlayerMaterialValues

        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _blueMat;
        [SerializeField] private Material _redMat;

        #endregion
        
        private readonly int _maxHealth = 10;
        private int _health;

        private void Start()
        {
            _health = _maxHealth;
        }

        private void OnEnable()
        {
            EventManager.AddHandler(GameEvent.OnHitTarget, new Action<int>(TakeDamage));
        }

        private void OnDisable()
        {
            EventManager.RemoveHandler(GameEvent.OnHitTarget, new Action<int>(TakeDamage));
        }

        private void TakeDamage(int damage)
        {
            _health = Mathf.Clamp(_health - damage, 0, _maxHealth);
            
            if (_health <= 0)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            Debug.Log("Game Over!");
        }

        [PunRPC]
        public void SetPlayerMat(string team)
        {
            Material[] mats = new Material[1];

            if (team == GameManager.Instance.TeamBlue)
            {
                mats[0] = _blueMat;
            }
            else if (team == GameManager.Instance.TeamRed)
            {
                mats[0] = _redMat;
            }

            _renderer.materials = mats;
        }
    }
}
