using System.Collections;
using Game.UserInterface.BattleMenus;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Game.UserInterface.ActionCommands
{
    /// <summary>
    /// Defines the action command for a side bump,
    /// which involves quickly clicking the left/right/up/down
    /// arrow key.
    /// </summary>
    public class BumpSideAC : ActionCommand
    {
        private bool _shown;
        private AttackMenu _cachedAttackMenu;
        private float _targetOpacity;
        private SpriteRenderer _entityRenderer;
        private Vector3 _correctionVector;
        private const float HintDuration = 0.5f;
        private Direction _direction;
        private bool _acceptingInput;
        private float _distanceThreshold;
        private bool _animFinished;
        private Image _successIndicator;
        private Color _originalColor;

        public AudioSource HitSound;
        public AudioSource BlockSound;

        private void Awake()
        {
            _distanceThreshold = GameManager.Instance.TileDimension / 8;
            _successIndicator = GetComponentInChildren<SuccessIndicator>().GetComponent<Image>();
            _originalColor = _successIndicator.color;
        }

        private void Start()
        {
            _successIndicator.color = _originalColor;
            _shown = false;
            _targetOpacity = 255f;
            Attatched = null;
            _entityRenderer = null;
            _correctionVector = new Vector3();
            GetComponentInChildren<Text>().text = Description;
            _acceptingInput = true;
            _animFinished = false;
            _cachedAttackMenu = GameObject.FindGameObjectWithTag("AttackButton").GetComponent<AttackMenu>();

        }

        public override void Show(Entity attatched)
        {
            _shown = true;
            Attatched = attatched;
            _entityRenderer = Attatched.GetComponent<SpriteRenderer>();
            _correctionVector = Attatched.EnemyVector.normalized * (GameManager.Instance.TileDimension / 4);
            switch (new Random().Next(0, 4))
            {
                case 0:
                    _direction = Direction.North;
                    break;
                case 1:
                    _direction = Direction.East;
                    break;
                case 2:
                    _direction = Direction.South;
                    break;
                case 3:
                    _direction = Direction.West;
                    break;
            }
            Anim1();
        }

        private void Anim1()
        {
            Attatched.TargetPosition = GetTargetPosition(Attatched.TargetPosition, false);
            _targetOpacity = 0f;
            StartCoroutine(Anim2());
        }

        private IEnumerator Anim2()
        {
            yield return new WaitForSeconds(HintDuration);
            Attatched.TargetPosition = GetTargetPosition(Attatched.TargetPosition, true);
            StartCoroutine(Anim3());
        }

        private IEnumerator Anim3()
        {
            Attatched.TargetPosition = Attatched.transform.position =
                GetTargetPosition(Attatched.TargetPosition - Attatched.EnemyVector - _correctionVector, false);
            yield return new WaitForSeconds(HintDuration * 4);
            StartCoroutine(Anim4());
        }

        private IEnumerator Anim4()
        {
            _targetOpacity = 1f;
            Attatched.TargetPosition = GetTargetPosition(Attatched.TargetPosition, true);
            yield return new WaitForSeconds(HintDuration);
            Anim5();
        }

        private void Anim5()
        {
            Attatched.TargetPosition = Attatched.TargetPosition + Attatched.EnemyVector + _correctionVector;
            _animFinished = true;
        }

        private Vector3 GetTargetPosition(Vector3 original, bool subtract)
        {
            switch (_direction)
            {
                case Direction.South:
                    return original + Quaternion.AngleAxis(90, Vector3.up) *
                           Attatched.EnemyVector.normalized * GameManager.Instance.TileDimension *
                           Constants.ActionCommandHintMagnitude * (subtract ? -1f : 1f);
                case Direction.West:
                    return original + Quaternion.AngleAxis(180, Vector3.up) *
                           Attatched.EnemyVector.normalized * GameManager.Instance.TileDimension *
                           Constants.ActionCommandHintMagnitude * (subtract ? -1f : 1f);
                case Direction.North:
                    return original + Quaternion.AngleAxis(270, Vector3.up) *
                           Attatched.EnemyVector.normalized * GameManager.Instance.TileDimension *
                           Constants.ActionCommandHintMagnitude * (subtract ? -1f : 1f);
                case Direction.East:
                    return original + Quaternion.AngleAxis(0, Vector3.up) *
                           Attatched.EnemyVector.normalized * GameManager.Instance.TileDimension *
                           Constants.ActionCommandHintMagnitude * (subtract ? -1f : 1f);
                default:
                    return original;
            }
        }

        public override void Update()
        {
            if (_shown)
            {
                ActionCanvas.alpha = Mathf.Lerp(ActionCanvas.alpha, 1f, Constants.UILerpTransitionSpeed * Time.deltaTime);
                Color transitionColor = new Color(_entityRenderer.color.r, _entityRenderer.color.g,
                    _entityRenderer.color.b,
                    Mathf.Lerp(_entityRenderer.color.a, _targetOpacity, Time.deltaTime * Constants.UILerpTransitionSpeed));
                _entityRenderer.color = transitionColor;
                bool correctDirection = false;
                if (Input.anyKeyDown && _acceptingInput)
                {
                    if (!(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) ||
                          Input.GetKeyDown(KeyCode.Mouse2)))
                    {
                        switch (_direction)
                        {
                            case Direction.South:
                                correctDirection = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
                                break;
                            case Direction.East:
                                correctDirection = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
                                break;
                            case Direction.North:
                                correctDirection = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
                                break;
                            case Direction.West:
                                correctDirection = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
                                break;
                        }
                        if ((correctDirection && (Attatched.transform.position - Attatched.CurrentEnemy.TargetPosition)
                             .magnitude > _distanceThreshold) && !_animFinished)
                            Succeed();
                        else
                            Fail();
                    }
                }

                if (_animFinished && _acceptingInput)
                    Fail();

            }
            else
            {
                ActionCanvas.alpha = Mathf.Lerp(ActionCanvas.alpha, 0f, Constants.UILerpTransitionSpeed * Time.deltaTime);
            }
        }

        private void Fail()
        {
            HitSound.Play();
            _acceptingInput = false;
            GetComponentInChildren<SuccessIndicator>().GetComponent<Image>().color = Color.red;
            Success = 1f; // Measuring success of the enemy
            StartCoroutine(SendAndDestroy());
        }

        private void Succeed()
        {
            BlockSound.Play();
            _acceptingInput = false;
            GetComponentInChildren<SuccessIndicator>().GetComponent<Image>().color = Color.green;
            Success = 0.2f;
            StartCoroutine(SendAndDestroy());
        }

        public override IEnumerator SendAndDestroy()
        {
            yield return new WaitForSeconds(1f);
            Attatched.Attack(Atk.Damage * Success);
            _cachedAttackMenu.Battle();
            Start();
        }
    }
}