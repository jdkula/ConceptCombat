using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    public abstract class Enemy : Entity
    {
        public static ArrayList EnemyList = new ArrayList();


        public abstract void Sleep();
        public abstract void Wake();

        protected override void OnDestroy()
        {
            EnemyList = new ArrayList();
        }

        public static void Freeze(GameObject initiator)
        {
            foreach (GameObject go in EnemyList)
            {
                if (!go.Equals(initiator))
                    go.GetComponent<Enemy>().Sleep();
            }
        }

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