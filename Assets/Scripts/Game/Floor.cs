using UnityEngine;

namespace Game
{
    /// <summary>
    /// Represents a floor tile. 
    /// </summary>
    public class Floor : MonoBehaviour
    {
        /// <summary>
        /// Updates floor for when a battle is taking place on it.
        /// </summary>
        public void Battle()
        {
            GetComponent<SpriteRenderer>().color = new Color(255,0,0);
        }

        /// <summary>
        /// Updates floor for when the battle finishes.
        /// </summary>
        public void ExitBattle()
        {
            GetComponent<SpriteRenderer>().color = new Color(255,255,255);
        }
    }
}
