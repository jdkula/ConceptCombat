using UnityEngine;

namespace Game
{
    /// <summary>
    /// Abstract definition of an Attack
    /// with an attached Action Command.
    /// </summary>
    public abstract class Attack : MonoBehaviour
    {
        public string Name;
        public string Description;
        public float Damage;
        public float Accuracy;
        public int Uses;
        public Animation Animation;
        public string ActionCommandName;
        public int UsesRemaining;

        private ActionCommand _cachedCommand;

        public ActionCommand GetActionCommand()
        {
            if (_cachedCommand) return _cachedCommand;
            _cachedCommand = GameObject.Find(ActionCommandName).GetComponent<ActionCommand>();
            _cachedCommand.SetupAttack(this);
            return GetActionCommand();
        }

        protected void Awake()
        {
            UsesRemaining = Uses;
        }
    }
}
