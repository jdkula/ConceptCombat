using UnityEngine;
using UnityEngine.UI;

namespace Game.UserInterface.BattleMenus
{
    public class AttackMenu : UserInterface
    {
        private bool _attackMode;
        private Vector2 _offScreenPosition;
        private Vector2 _onScreenPosition;
        private Vector2 _onScreenSizeDelta;
        private Vector2 _offScreenSizeDelta;
        private RectTransform _rt;


        private Vector3 _scaleVelocity;
        private Vector3 _positionVelocity;

        public GameObject AttackInfoMenu;
        public RectTransform BattleMenuRt;
        public CanvasGroup TextCanvas;
        public CanvasGroup AttackListCanvas;
        public CanvasGroup BackCanvas;
        public Button ThisButton;

        private Attack[] _attacks;
        private bool _battleMode;
        private Vector2 _attackPosition;


        public void Attack()
        {
            print("Attack Mode Engaged");
            _attackMode = true;
        }

        public void Back()
        {
            _attackMode = false;
        }

        private void Start()
        {
            _rt = GetComponent<RectTransform>();
            TextCanvas = GetComponentInChildren<Text>().gameObject.GetComponent<CanvasGroup>();

            _offScreenPosition = _rt.anchoredPosition;
            _onScreenSizeDelta = BattleMenuRt.sizeDelta;
            _offScreenSizeDelta = _rt.sizeDelta;
            _onScreenPosition = new Vector2(_offScreenPosition.x * -1, _offScreenPosition.y);
            _attackPosition = new Vector2(_offScreenPosition.x * -1,
                _offScreenPosition.y - _onScreenSizeDelta.y / 2.7f);


            GameObject attackList = GameObject.FindGameObjectWithTag("Attack List");
            _attacks = GameManager.Instance.Player.GetComponents<Attack>();

            float buttonHeight = attackList.GetComponent<RectTransform>().sizeDelta.y /
                                 (_attacks.Length + Constants.AttackInfoButtonPadding);

            float attackBackHeight = BackCanvas.gameObject.GetComponent<RectTransform>().sizeDelta.y;

            for (int i = 0; i < _attacks.Length; i++)
            {
                GameObject infoObject = Instantiate(AttackInfoMenu,
                    Vector3.zero,
                    Quaternion.identity, attackList.transform);
                    RectTransform newRt = infoObject.GetComponent<RectTransform>();
                        newRt.anchoredPosition = new Vector2(0,
                    -1 * buttonHeight * 2 * i - newRt.sizeDelta.y / 2 - attackBackHeight);

                infoObject.GetComponent<AttackInfoMenu>().Atk = _attacks[i];
            }
        }

        public void Battle()
        {
            _battleMode = true;
        }

        public void ExitBattle()
        {
            _battleMode = false;
        }

        public void Hide()
        {
            ExitBattle();
            Back();
        }

        public void Unhide()
        {
            Battle();
            Attack();
        }

        private void Update()
        {
            if (_battleMode && !_attackMode)
            {
                _rt.anchoredPosition = Vector3.SmoothDamp(_rt.anchoredPosition, _onScreenPosition,
                    ref _positionVelocity, Constants.AttackMenuAnimationDuration);
            }

            if (!_battleMode)
            {
                _rt.anchoredPosition = Vector3.SmoothDamp(_rt.anchoredPosition, _offScreenPosition,
                    ref _positionVelocity, Constants.AttackMenuAnimationDuration);
            }
            if (_attackMode)
            {
                _rt.sizeDelta = Vector3.SmoothDamp(_rt.sizeDelta, _onScreenSizeDelta, ref _scaleVelocity,
                    Constants.AttackMenuAnimationDuration);
                _rt.anchoredPosition = Vector3.SmoothDamp(_rt.anchoredPosition, _attackPosition,
                    ref _positionVelocity, Constants.AttackMenuAnimationDuration);
                TextCanvas.alpha = Mathf.Lerp(TextCanvas.alpha, 0f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                BackCanvas.alpha = Mathf.Lerp(BackCanvas.alpha, 1f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                BackCanvas.interactable = true;
                BackCanvas.blocksRaycasts = true;
                AttackListCanvas.alpha = Mathf.Lerp(AttackListCanvas.alpha, 1f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                AttackListCanvas.interactable = true;
                AttackListCanvas.blocksRaycasts = true;
                ThisButton.interactable = false;
            }
            if (!_attackMode)
            {
                _rt.sizeDelta = Vector3.SmoothDamp(_rt.sizeDelta, _offScreenSizeDelta, ref _scaleVelocity,
                    Constants.AttackMenuAnimationDuration);
                TextCanvas.alpha = Mathf.Lerp(TextCanvas.alpha, 1f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                BackCanvas.alpha = Mathf.Lerp(BackCanvas.alpha, 0f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                BackCanvas.interactable = false;
                BackCanvas.blocksRaycasts = false;
                AttackListCanvas.alpha = Mathf.Lerp(AttackListCanvas.alpha, 0f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                AttackListCanvas.interactable = false;
                AttackListCanvas.blocksRaycasts = false;
                ThisButton.interactable = true;
            }
        }
    }
}