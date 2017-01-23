using UnityEngine;

namespace Game
{
    public class Floor : MonoBehaviour
    {
        public GameObject Child;

        public void Battle(GameObject child)
        {
            GetComponent<SpriteRenderer>().color = new Color(255,0,0);
            Child = child;
        }

        public void ExitBattle()
        {
            GetComponent<SpriteRenderer>().color = new Color(255,255,255);
            Child = null;
        }
    }
}
