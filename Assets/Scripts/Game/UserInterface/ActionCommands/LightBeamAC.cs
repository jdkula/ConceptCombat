using System.Collections;
using Game.UserInterface.BattleMenus;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Game.UserInterface.ActionCommands
{
    public class LightBeamAC : ActionCommand
    {
        private bool _shown;
        public GameObject KeyPanelPrefab;
        public AudioSource Correct;
        public AudioSource Fail;
        public AudioSource Perfect;
        public Image Timer;
        public Sprite Countdown;
        public Sprite FailCountdown;
        private const float CompletionTimeBase = 4f;
        private float _timeLeft;
        private float _startTime;
        private bool _clockStopped;

        private readonly string[] _greekLetters =
            new[]
            {
                "α", "β", "δ",
                "ε", "ζ", "η",
                "ι", "κ", "λ",
                "μ", "ν", "ο",
                "π", "ρ", "σ",
                "τ", "υ", "φ"
            };

        private readonly string[] _englishEquivs =
            new[]
            {
                "a", "b", "d",
                "e", "z", "e",
                "i", "k", "l",
                "m", "n", "o",
                "p", "r", "s",
                "t", "y", "f"
            };

        private GameObject[] _keys;
        private int[] _pressRequirements;

        private int _currentLetter;
        private int _numLetters;

        private AttackMenu _cachedAtkMenu;
        private bool _acceptingInput;

        private void Start()
        {
            GetComponentInChildren<Text>().text = Description;
            Timer.sprite = Countdown;
            _acceptingInput = true;

            _currentLetter = 0;
            Random rnd = new Random();
            float width = GetComponent<RectTransform>().sizeDelta.x - Constants.ActionCommandPadding;
            float hieght = GetComponent<RectTransform>().sizeDelta.y - Constants.ActionCommandPadding -
                           GetComponentInChildren<Text>().gameObject.GetComponent<RectTransform>().sizeDelta.y;
            _numLetters = rnd.Next(2,7);
            int rows = Mathf.CeilToInt((float) _numLetters / 5);
            float keyWidth = width / 5f;
            float keyHeight = hieght / rows;
            _startTime = _timeLeft = CompletionTimeBase + Mathf.Sqrt(_numLetters);
            _clockStopped = false;

            _keys = new GameObject[_numLetters];
            _pressRequirements = new int[_numLetters];

            for (int i = 0; i < _numLetters; i++)
            {
                GameObject go = Instantiate(KeyPanelPrefab, transform.position, Quaternion.identity, transform);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(keyWidth, keyHeight);
                rt.anchoredPosition = new Vector2(keyWidth / 2 + Constants.ActionCommandPadding + keyWidth * (i % 5),
                    keyHeight * rows / 2f - keyHeight / 2f - keyHeight * Mathf.FloorToInt(i / 5f));

                int keyToPress = rnd.Next(0, _greekLetters.Length);

                go.GetComponentInChildren<Text>().text = _greekLetters[keyToPress];

                _keys[i] = go;
                _pressRequirements[i] = keyToPress;
            }
        }

        public override void Show(Entity attatched)
        {
            Attatched = attatched;
            _cachedAtkMenu = GameObject.FindGameObjectWithTag("AttackButton").GetComponent<AttackMenu>();
            _shown = true;
            _cachedAtkMenu.Hide();
        }

        public override void Update()
        {
            if (_shown)
            {
                ActionCanvas.alpha = Mathf.Lerp(ActionCanvas.alpha, 1f,
                    Time.deltaTime * Constants.UILerpTransitionSpeed);

                if (Input.anyKeyDown && _acceptingInput)
                {
                    if (!(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) ||
                          Input.GetKeyDown(KeyCode.Mouse2)))
                    {
                        if (Input.GetKeyDown(_englishEquivs[_pressRequirements[_currentLetter]]))
                        {
                            _keys[_currentLetter].GetComponent<Image>().color = new Color(0, 255, 0, 100);
                            _keys[_currentLetter].GetComponentInChildren<Text>().text =
                                _englishEquivs[_pressRequirements[_currentLetter]];
                            _currentLetter++;
                            Correct.Play();
                            if (_currentLetter == _numLetters)
                            {
                                _clockStopped = true;
                                Perfect.Play();
                                StartCoroutine(SendAndDestroy());
                            }
                        }
                        else
                        {
                            _keys[_currentLetter].GetComponentInChildren<Text>().text =
                                _englishEquivs[_pressRequirements[_currentLetter]];
                            _clockStopped = true;
                            Fail.Play();
                            _keys[_currentLetter].GetComponent<Image>().color = new Color(255, 0, 0, 100);
                            StartCoroutine(SendAndDestroy());
                        }
                    }
                }

                if (!_clockStopped)
                {
                    Timer.fillAmount = Mathf.Lerp(Timer.fillAmount, _timeLeft / _startTime, Time.deltaTime * Constants.UILerpTransitionSpeed);
                    if (_timeLeft < 0)
                    {
                        _clockStopped = true;
                        _keys[_currentLetter].GetComponent<Image>().color = new Color(255, 0, 0, 100);
                        _keys[_currentLetter].GetComponentInChildren<Text>().text =
                            _englishEquivs[_pressRequirements[_currentLetter]];
                        Timer.fillAmount = 1;
                        Timer.sprite = FailCountdown;
                        Fail.Play();
                        StartCoroutine(SendAndDestroy());
                    }
                    else
                    {
                        _timeLeft -= Time.deltaTime;
                    }
                }
            }
            else
            {
                ActionCanvas.alpha = Mathf.Lerp(ActionCanvas.alpha, 0f,
                    Time.deltaTime * Constants.UILerpTransitionSpeed);
            }
        }

        public override IEnumerator SendAndDestroy()
        {
            _acceptingInput = false;
            Atk.UsesRemaining--;
            yield return new WaitForSeconds(1f);
            print("Damage - " + Atk.Damage * (_currentLetter / (float) _numLetters));
            Attatched.Attack(Atk.Damage * (_currentLetter / (float) _numLetters));
            _shown = false;
            foreach (GameObject key in _keys)
            {
                Destroy(key);
            }
            Start();

            //TODO: Animation!
            //_cachedAtkMenu.Battle();
            Attatched.PassTurn();
        }
    }
}