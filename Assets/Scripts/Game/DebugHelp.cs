using UnityEngine;

namespace Game
{
    public class DebugHelp : MonoBehaviour
    {

        public CameraHelper cam;
        public GameObject go;

        // Use this for initialization
        void Start ()
        {
            Vector3[] bounds = cam.GetFustrumBounds(5);
            foreach (Vector3 v in bounds)
            {
                Instantiate(go, v, go.transform.rotation);
            }
            bounds = GameManager.Squarify(bounds, cam);
            foreach (Vector3 v in bounds)
            {
                Instantiate(go, v, go.transform.rotation);
            }

        }
	
        // Update is called once per frame
        void Update () {
		
        }
    }
}
