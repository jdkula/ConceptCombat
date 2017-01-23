using Game.UserInterface;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class Player : Entity
    {
        public Rigidbody Rb;
        public CameraGroup CamGroup;

        // Use this for initialization
        protected override void Start()
        {
            AttatchedUserInterface = GameManager.Instance.PUI;
            AttatchedUserInterface.HitPointsTotal = Health;
            AttatchedUserInterface.HitPointsRemaining = Health;
            base.Start();
        }

        // Update is called once per frame
        new void Update()
        {
            if (!BattleMode)
            {
                Vector3 movement = Vector3.zero;
                if (Input.GetKey("w") || Input.GetKey("up"))
                {
                    movement += Vector3.up;
                }
                if (Input.GetKey("a") || Input.GetKey("left"))
                {
                    movement += Vector3.left;
                }
                if (Input.GetKey("s") || Input.GetKey("down"))
                {
                    movement += Vector3.down;
                }
                if (Input.GetKey("d") || Input.GetKey("right"))
                {
                    movement += Vector3.right;
                }

                transform.Translate(movement * Time.deltaTime * GameManager.Instance.TileDimension *
                                    Constants.PlayerSpeedMultiplier);
            }
            base.Update();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<Enemy>())
            {
                Enemy.Freeze(other.gameObject);
                GameManager.Instance.EnterBattle(GetComponent<Player>(), other.gameObject.GetComponent<Enemy>());
                other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                other.gameObject.GetComponent<Collider>().enabled = false;
                Rb.isKinematic = true;
                GetComponent<Collider>().enabled = false;
            }
            if (other.gameObject.GetComponent<Door>())
            {
                GameManager.Instance.NextLevel(other.gameObject);
            }
        }

        public override void Win()
        {
            ((PlayerUserInterface) AttatchedUserInterface).XPIncrease(CurrentEnemy.Difficulty);
            base.Win();
        }

        public override void TakeTurn()
        {
            AttatchedUserInterface.Battle(this);
        }

        protected override void OnDestroy()
        {
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }

        public override void Attack(float damage)
        {
            if (CurrentEnemy)
            {
                CurrentEnemy.Damage(damage * Mathf.Clamp(
                                        ((PlayerUserInterface) AttatchedUserInterface).Level * Constants.LevelScale, 1,
                                        float.PositiveInfinity));
            }
        }
    }
}