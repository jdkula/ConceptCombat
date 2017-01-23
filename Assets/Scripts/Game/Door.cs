using UnityEngine;

namespace Game
{
    public class Door : MonoBehaviour
    {
        public Sprite OpenSprite;
        public Sprite ClosedSprite;

        public void Open()
        {
            GetComponent<SpriteRenderer>().sprite = OpenSprite;
        }
    }
}
