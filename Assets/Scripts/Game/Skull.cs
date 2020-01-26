using System;
using UnityEngine;
using Random = System.Random;

namespace Game
{
    /// <summary>
    /// Represents the skull enemy, and their movement.
    /// </summary>
    public class Skull : Enemy
    {
        private Random _rnd;
        public Rigidbody Rb;
        public Transform Tf;

        private Vector3 _pausedVelocity = Vector3.zero;
        private int _wait = -1;

        // Use this for initialization
        protected override void Start()
        {
            _rnd = new Random(DateTime.Now.Millisecond + Mathf.RoundToInt(UnityEngine.Random.value * 10000));
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (!BattleMode)
            {
                if (_wait > 60)
                {
                    _wait--;
                }
                else if (_wait == 60)
                {
                    int xy = _rnd.Next(0, 2);
                    int velX = 0;
                    int velZ = 0;
                    if (xy == 0)
                    {
                        velX = _rnd.Next(-1, 2);
                        if (velX == 0)
                        {
                            velZ = _rnd.Next(0, 2);
                            if (velZ == 0) velZ = -1;
                        }
                    }

                    if (xy == 1)
                    {
                        velZ = _rnd.Next(-1, 2);
                        if (velZ == 0)
                        {
                            velX = _rnd.Next(0, 2);
                            if (velX == 0) velX = -1;
                        }
                    }

                    Vector3 newVel = new Vector3(velX, 0, velZ) * Constants.SkullSpeed * _rnd.Next(1, 11);
                    Rb.velocity = newVel;
                    _wait--;
                }
                else if (_wait < 60 && _wait >= 0)
                {
                    _wait--;
                }
            }

            base.Update();
        }

        public void OnCollisionEnter()
        {
            if (!BattleMode)
            {
                if (_wait == -1)
                {
                    Rb.velocity = Vector3.zero;
                    Ray ray = new Ray(Tf.position, Vector3.up);
                    RaycastHit hit;
                    Physics.Raycast(ray, out hit);
                    Tf.position = Vector3.Lerp(Tf.position, hit.transform.position,
                        Constants.LerpTransitionSpeed * Time.deltaTime);
                    _wait = 120;
                }
            }
            else
            {
            }
        }

        public override void Sleep()
        {
            _wait = -1;
            _pausedVelocity = Rb.velocity;
            Rb.velocity = Vector3.zero;
            Rb.Sleep();
            GetComponent<BoxCollider>().enabled = false;
        }

        public override void Wake()
        {
            GetComponent<BoxCollider>().enabled = true;
            Rb.WakeUp();
            Rb.velocity = _pausedVelocity;
            _pausedVelocity = Vector3.zero;
        }
    }
}