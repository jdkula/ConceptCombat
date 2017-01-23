using Game.UserInterface.BattleMenus;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UserInterface
{
    public class PlayerUserInterface : HUD
    {
        public Image ExperienceBar;
        public Text LevelText;
        public GameObject BattleMenu;

        private float _currentXp;
        private float _levelXp = 1;
        public int Level = 1;

        void Start()
        {

        }

        protected override void Update()
        {
            base.Update();

            ExperienceBar.fillAmount = Mathf.Lerp(ExperienceBar.fillAmount, _currentXp / _levelXp, Time.deltaTime * Constants.LerpTransitionSpeed);
            LevelText.text = Level.ToString();
        }

        public new void Battle(Entity attatched)
        {
            base.Battle(attatched);

            BattleMenu.GetComponentInChildren<AttackMenu>().Battle();

        }

        public void XPIncrease(float amount)
        {
            _currentXp += amount;
            while (_currentXp >= _levelXp)
            {
                Level++;
                _currentXp -= _levelXp;
                _levelXp = _levelXp * Constants.LevelModifier;
            }
        }

        public override void ExitBattle()
        {
            BattleMenu.GetComponentInChildren<AttackMenu>().ExitBattle();
        }

    }
}