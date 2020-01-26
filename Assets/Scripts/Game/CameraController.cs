using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    /// <summary>
    /// Provides easy ways to lock onto and
    /// follow the player, as well as complete the
    /// transition from field to battle mode.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public GameObject player;

        public bool Following
        {
            get { return _following; }
            set
            {
                if (!_battlemode)
                    _following = value;
            }
        }

        private bool _following;

        private Vector3 _velocity;

        private bool _battlemode;
        private GameObject _enemy;
        private Vector3 _battlePosition;
        private Quaternion _battleRotation;

        private Quaternion _originalRotation;

        private void Start()
        {
            _originalRotation = transform.rotation;
        }

        private void Update()
        {
            if (Following)
            {
                transform.position = Vector3.SmoothDamp(transform.position,
                    player.transform.position + (Vector3.up * Constants.CameraDistance), ref _velocity,
                    Constants.CameraSlowness);
                transform.rotation = Quaternion.Slerp(transform.rotation, _originalRotation, Time.deltaTime * Constants.LerpTransitionSpeed);
            }

            else if (_battlemode)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _battlePosition, ref _velocity, 1);
                transform.rotation = Quaternion.Slerp(transform.rotation, _battleRotation, Time.deltaTime * Constants.LerpTransitionSpeed);
            }
        }

        /// <summary>
        /// Given the two battling, transitions the camera to point at them.
        /// </summary>
        /// <param name="player">The player (on the left)</param>
        /// <param name="enemy">The enemy (on the right)</param>
        public void Battle(Entity player, Entity enemy)
        {
            Following = false;
            _velocity = Vector3.zero;

            Vector3 averageCenter = (player.TargetPosition + enemy.TargetPosition) / 2;
            Vector3 playerToEnemyVector = player.TargetPosition - enemy.TargetPosition;
            Vector3 direction = -1 * Vector3.Cross(playerToEnemyVector, Vector3.down).normalized;

            _battlePosition = averageCenter + (direction * Constants.BattleDistanceXFactor +
                                               Vector3.up * Constants.BattleDistanceYFactor) *
                              GameManager.Instance.TileDimension * playerToEnemyVector.magnitude * 10 *
                              Mathf.Log(Constants.TileCountPerScreen) /
                              Mathf.Log(10 * Mathf.Sqrt(Constants.TileCountPerScreen));
            _battleRotation = Quaternion.LookRotation(-1 * (_battlePosition - averageCenter).normalized, Vector3.up);

            _battlemode = true;
        }

        public void ExitBattle()
        {
            _battlemode = false;
            Following = true;
        }
    }
}