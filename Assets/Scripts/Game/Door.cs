using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    /// <summary>
    /// Represents the door that wins the game!
    /// </summary>
    public class Door : MonoBehaviour
    {
        public Sprite openSprite;

        public void Open()
        {
            GetComponent<SpriteRenderer>().sprite = openSprite;
        }
    }
}
