using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    public abstract class Enemy : Entity
    {
        public static ArrayList EnemyList = new ArrayList();

        /// <summary>
        /// Puts this enemy to "sleep" i.e. during battle
        /// </summary>
        public abstract void Sleep();
        
        /// <summary>
        /// "wakes" this enemy i.e. once a battle has finished.
        /// </summary>
        public abstract void Wake();

        protected override void OnDestroy()
        {
            EnemyList.Remove(gameObject);
        }

        /// <summary>
        /// Freezes all enemies, except the initiator (used when entring battle)
        /// </summary>
        /// <param name="initiator">The enemy that shouldn't be frozen.</param>
        public static void Freeze(GameObject initiator)
        {
            foreach (GameObject go in EnemyList)
            {
                if (!go.Equals(initiator))
                    go.GetComponent<Enemy>().Sleep();
            }
        }

        /// <summary>
        /// Unfreezes/"wakes" all enemies.
        /// </summary>
        public static void Unfreeze()
        {
            foreach (GameObject go in EnemyList)
            {
                go.GetComponent<Enemy>().Wake();
            }
        }

        protected new virtual void Start()
        {
            AttatchedUserInterface = GameManager.Instance.EUI;
            base.Start();
        }

        protected new virtual void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Takes this enemy's turn in battle
        /// </summary>
        public override void TakeTurn()
        {
            if (Health > 0)
            {
                Attack[] attacks = GetComponents<Attack>();
                Attack randAtack = attacks[new Random().Next(0, attacks.Length)];

                randAtack.GetActionCommand().Show(this);
            }
        }
    }
}