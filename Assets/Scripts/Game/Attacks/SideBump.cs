using System;

namespace Game.Attacks
{
    public class SideBump : Attack
    {
        protected new void Awake()
        {
            Name = "Side Bump";
            Description = "Knock your foe in its side!";
            ActionCommandName = "Side Bump AC";
            Damage = 5;
            Accuracy = 1f;
            Uses = Int32.MaxValue;
            base.Awake();
        }
    }
}