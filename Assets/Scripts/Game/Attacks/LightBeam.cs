using System;

namespace Game.Attacks
{
    public class LightBeam : Attack
    {
        protected new void Awake()
        {
            Name = "Light Beam";
            Description = "Shoots a beam of light at your foe";
            ActionCommandName = "Light Beam AC";
            Damage = 20;
            Accuracy = 0.7f;
            Uses = Int32.MaxValue;
            base.Awake();
        }
    }
}