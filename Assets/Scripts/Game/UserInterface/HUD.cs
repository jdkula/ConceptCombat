using UnityEngine;
using UnityEngine.UI;

namespace Game.UserInterface
{
    public class HUD : UserInterface
    {
        public Image HitPointsBar;
        public bool BattleMode;

        public float HitPointsRemaining;
        public float HitPointsTotal;

        // Use this for initialization
        void Start () {

        }

        // Update is called once per frame
        protected virtual void Update ()
        {
            if (float.IsNaN(HitPointsBar.fillAmount)) HitPointsBar.fillAmount = 1;
            HitPointsBar.fillAmount = Mathf.Lerp(HitPointsBar.fillAmount, HitPointsRemaining / HitPointsTotal, Time.deltaTime * Constants.UILerpTransitionSpeed);
        }

        public void Battle(Entity attatched)
        {
            HitPointsRemaining = attatched.Health;
            HitPointsTotal = attatched.Health;
            BattleMode = true;
        }

        public virtual void ExitBattle()
        {
            BattleMode = false;
        }

        public void Damage(float damage)
        {
            HitPointsRemaining = Mathf.Clamp(HitPointsRemaining - damage, 0, float.PositiveInfinity);
            print(HitPointsRemaining);
        }
    }
}