using System;

namespace Game.Attacks
{
    /// <summary>
    /// Represents the "light beam" attack
    /// the player can use
    /// </summary>
    public class LightBeam : Attack
    {
        protected new void Awake()
        {
            Name = "Light Beam";
            Description = "Shoots a beam of light at your foe";
            ActionCommandName = "Light Beam AC";
            Damage = 20;
            Accuracy = 0.7f;
            Uses = 20;
            base.Awake();
        }
    }
}