using System.Collections;
using Game.UserInterface.BattleMenus;
using UnityEngine;

namespace Game
{
    public abstract class ActionCommand : MonoBehaviour
    {
        /// <summary>
        /// Description of what the action command will require the user to do.
        /// </summary>
        public string Description;

        /// <summary>
        /// Some ActionCommands might only work for mobile or PC platforms.
        /// </summary>
        public Platform[] SupportedPlatforms;

        /// <summary>
        /// The CanvasGroup to control this ActionCommand.
        /// </summary>
        public CanvasGroup ActionCanvas;

        /// <summary>
        /// The success value, which determines damage output.
        /// </summary>
        protected float Success;

        /// <summary>
        /// The attatched entity, which will recieve success data when this finishes.
        /// </summary>
        protected Entity Attatched;

        /// <summary>
        /// The attatched Attack
        /// </summary>
        protected Attack Atk;


        /// <summary>
        /// Shows the UI of the Action Command.
        /// </summary>
        /// <param name="atkMenu">For hiding and showing the attack menu</param>
        /// <param name="attatched">The attacking Entity</param>
        public abstract void Show(Entity attatched);

        public abstract void Update();

        /// <summary>
        /// Sends success data and destroys this UI.
        /// </summary>
        public abstract IEnumerator SendAndDestroy();

        /// <summary>
        /// Sets up the Attack
        /// </summary>
        /// <param name="atk">The Attack</param>
        public void SetupAttack(Attack atk)
        {
            Atk = atk;
        }

    }
}