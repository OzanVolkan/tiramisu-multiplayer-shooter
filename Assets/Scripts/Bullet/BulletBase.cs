using System;
using System.Collections;
using UnityEngine;

namespace Bullet
{
    public abstract class BulletBase : MonoBehaviour
    {
        protected int Damage { get; set; }
        protected float SpeedMultiplier { get; set; }

        public void Move()
        {
            transform.Translate(transform.right * (Time.deltaTime * SpeedMultiplier * 10f));
        }

        private void Update()
        {
            Move();
        }
    }
}
