using System;
using Managers;
using Photon.Pun;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
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
    }
}
