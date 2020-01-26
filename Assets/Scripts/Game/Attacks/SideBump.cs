using System;

namespace Game.Attacks
{
    /// <summary>
    /// Represents the enemy attack.
    /// </summary>
    public class SideBump : Attack
    {
        protected new void Awake()
        {
            Name = "Side Bump";
            Description = "Knock your foe in its side!";
            ActionCommandName = "Side Bump AC";
            Damage = 5;
            Accuracy = 1f;
            Uses = 100;
            base.Awake();
        }
    }
}