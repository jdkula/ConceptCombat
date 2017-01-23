using UnityEngine;
using UnityEngine.UI;

namespace Game.UserInterface.BattleMenus
{
    public class AttackInfoMenu : UserInterface
    {
        private bool _open;
        private Vector3 _openSizeDelta;
        private Vector3 _closedSizeDelta;
        private Vector3 _openPosition;
        private Vector3 _closedPosition;

        private Vector3 _sizeVelocity;
        private Vector3 _positionVelocity;

        public CanvasGroup BackCanvas;
        public RectTransform Rt;
        public Button ThisButton;
        public CanvasGroup InstructionTextCanvas;
        public AttackMenu AtkMenu;
        public Attack Atk;


        public void Open()
        {
            if (!_open)
                _open = true;
            else
                DoAttack();
        }

        public void Close()
        {
            _open = false;
        }

        public void DoAttack()
        {
            if (_open)
            {
                Atk.GetActionCommand().Show(GameManager.Instance.Player.GetComponent<Entity>());
                Close();
            }
        }

        private void Start()
        {
            AtkMenu = GameObject.FindGameObjectWithTag("AttackButton").GetComponent<AttackMenu>();
            foreach (Text txt in GetComponentsInChildren<Text>())
            {
                switch (txt.text)
                {
                    case "[AtkName]":
                        txt.text = Atk.Name;
                        break;
                    case "[Desc]":
                        txt.text =
                            "Accuracy - " + Atk.Accuracy + "\n"
                            + "Damage - " + Atk.Damage + "\n"
                            + "Uses - " + Atk.UsesRemaining + "/" + Atk.Uses + "\n"
                            + Atk.Description + "\n\n"
                            + "Action Command Description: " + "\n"
                            + Atk.GetActionCommand().Description;
                        break;
                }
            }
            RectTransform battleMenuRt = GetComponentInParent<RectTransform>()
                .gameObject.GetComponentInParent<AttackMenu>()
                .BattleMenuRt;

            _openSizeDelta = battleMenuRt.sizeDelta;
            _closedSizeDelta = Rt.sizeDelta;
            _openPosition = new Vector2(0, _openSizeDelta.y / -2);
            _closedPosition = Rt.anchoredPosition;
        }

        private void Update()
        {
            if (_open)
            {
                Rt.anchoredPosition = Vector3.SmoothDamp(Rt.anchoredPosition, _openPosition, ref _positionVelocity,
                    Constants.AttackMenuAnimationDuration);
                Rt.sizeDelta = Vector3.SmoothDamp(Rt.sizeDelta, _openSizeDelta, ref _sizeVelocity,
                    Constants.AttackMenuAnimationDuration);

                BackCanvas.alpha = Mathf.Lerp(BackCanvas.alpha, 1f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                BackCanvas.interactable = true;
                BackCanvas.blocksRaycasts = true;
                InstructionTextCanvas.alpha = Mathf.Lerp(InstructionTextCanvas.alpha, 1f,
                    Time.deltaTime * Constants.UILerpTransitionSpeed);
            }
            else
            {
                Rt.anchoredPosition = Vector3.SmoothDamp(Rt.anchoredPosition, _closedPosition, ref _positionVelocity,
                    Constants.AttackMenuAnimationDuration);
                Rt.sizeDelta = Vector3.SmoothDamp(Rt.sizeDelta, _closedSizeDelta, ref _sizeVelocity,
                    Constants.AttackMenuAnimationDuration);

                BackCanvas.alpha = Mathf.Lerp(BackCanvas.alpha, 0f, Time.deltaTime * Constants.UILerpTransitionSpeed);
                BackCanvas.interactable = false;
                BackCanvas.blocksRaycasts = false;

                InstructionTextCanvas.alpha = Mathf.Lerp(InstructionTextCanvas.alpha, 0f,
                    Time.deltaTime * Constants.UILerpTransitionSpeed);
            }
        }
    }
}