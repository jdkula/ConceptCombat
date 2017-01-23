using UnityEngine;

namespace Game.UserInterface
{
    public class EnemyUserInterface : HUD
    {
        public RectTransform Rt;

        private Vector3 _offScreenLocation;
        private Vector3 _onScreenLocation;
        private Vector3 _velocity = Vector3.zero;

        private void Start()
        {
            _offScreenLocation = Rt.anchoredPosition;
            _onScreenLocation = new Vector3(Rt.anchoredPosition.x * -1, Rt.anchoredPosition.y, 0);
        }

        protected override void Update()
        {
            if (BattleMode)
            {
                Rt.anchoredPosition = Vector3.SmoothDamp(Rt.anchoredPosition, _onScreenLocation, ref _velocity, 0.5f);
            }
            else
            {
                Rt.anchoredPosition = Vector3.SmoothDamp(Rt.anchoredPosition, _offScreenLocation, ref _velocity, 0.5f);
            }

            base.Update();
        }
    }
}
