using System;
using System.Collections;
using Game.UserInterface;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// General class representing an entity -- either an enemy or the player.
    /// </summary>
    public class Entity : MonoBehaviour
    {
        public Entity CurrentEnemy;
        private Quaternion _targetRotation;
        private Vector3 _targetScale;
        public bool BattleMode;
        public Vector3 TargetPosition;
        public string Name;
        public float Health;
        public AudioSource DeathSound;
        private bool _lost;

        public Vector3 EnemyVector;

        public float Difficulty;

        private Quaternion _originalRotation;
        private Vector3 _originalScale;

        public HUD AttatchedUserInterface;

        protected virtual void Start()
        {
            _originalRotation = transform.rotation;
            _targetScale = _originalScale = transform.localScale;
            _targetRotation = _originalRotation;
        }

        public virtual void ExitBattle()
        {
            _targetScale = _originalScale;
            _targetRotation = _originalRotation;
        }

        public void BattlePositions()
        {
            _targetScale = transform.localScale * Constants.EntityBattleScale;
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.up);
            Physics.Raycast(ray, out hit);
            TargetPosition = hit.transform.position + new Vector3(0, _targetScale.y / 2, 0);
            hit.transform.gameObject.GetComponent<Floor>().Battle();
        }

        /// <summary>
        /// Engages an enemy in battle, rotating this sprite so it stands up!
        /// </summary>
        /// <param name="enemy">The opponent</param>
        public virtual void Battle(Entity enemy)
        {
            CurrentEnemy = enemy;
            EnemyVector = TargetPosition - enemy.TargetPosition;
            Debug.DrawLine(TargetPosition, enemy.TargetPosition, Color.red, 1000f);
            Vector3 direction = Vector3.Cross(EnemyVector, Vector3.up).normalized;

            TargetPosition += EnemyVector.normalized * (GameManager.Instance.TileDimension / 4);

            _targetRotation = Quaternion.LookRotation(direction);
            BattleMode = true;

            AttatchedUserInterface.HitPointsTotal = Health;
            AttatchedUserInterface.HitPointsRemaining = Health;
        }

        protected void Update()
        {
            if (!_lost)
            {
                if (CurrentEnemy && BattleMode)
                {
                    transform.position = Vector3.Lerp(transform.position, TargetPosition,
                        Time.deltaTime * Constants.UILerpTransitionSpeed);
                }
                if (CurrentEnemy && Math.Abs(Health) < 0.01)
                {
                    CurrentEnemy.Win();
                    Lose();
                }

                transform.localScale = Vector3.Lerp(transform.localScale, _targetScale,
                    Time.deltaTime * Constants.UILerpTransitionSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation,
                    Time.deltaTime * Constants.UILerpTransitionSpeed);
            }

        }

        /// <summary>
        /// Damages the enemy for some amount of damage.
        /// </summary>
        public virtual void Attack(float damage)
        {
            if (CurrentEnemy)
            {
                CurrentEnemy.Damage(damage);
            }
        }

        /// <summary>
        /// Accepts some amount of damage from an enemy.
        /// </summary>
        public void Damage(float damage)
        {
            AttatchedUserInterface.Damage(damage);
            Health = Mathf.Clamp(Health - damage, 0, float.PositiveInfinity);
        }

        /// <summary>
        /// Exits the battle, having won it!
        /// Transitions back to the field view.
        /// </summary>
        public virtual void Win()
        {
            BattleMode = false;
            GameManager.Instance.ExitBattle(this, CurrentEnemy);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Collider>().enabled = true;
            
            // tells the floor below that the battle is done.
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.up);
            Physics.Raycast(ray, out hit);
            hit.transform.gameObject.GetComponent<Floor>().ExitBattle();
        }

        /// <summary>
        /// Causes this entity to die, having lost the battle.
        /// </summary>
        public virtual void Lose()
        {
            GetComponent<SpriteRenderer>().enabled = false;
            _lost = true;
            DeathSound.Play();
            GetComponent<Collider>().enabled = false;
            
            // tell the floor that the battle has finished
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.up);
            Physics.Raycast(ray, out hit);
            hit.transform.gameObject.GetComponent<Floor>().ExitBattle();
            
            StartCoroutine(DestroyWait());
        }

        private IEnumerator DestroyWait()
        {
            yield return new WaitForSecondsRealtime(DeathSound.clip.length);
            Destroy(gameObject);
        }

        public virtual void TakeTurn()
        {
        }

        public virtual void PassTurn()
        {
            CurrentEnemy.TakeTurn();
        }

        protected virtual void OnDestroy()
        {

        }
    }
}