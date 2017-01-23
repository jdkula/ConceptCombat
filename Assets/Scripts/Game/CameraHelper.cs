using UnityEngine;

namespace Game
{
    public class CameraHelper : MonoBehaviour
    {
        public Camera CameraInstance;
        public Transform CameraTransformInstance;
        public GameObject CameraGroup;
        public bool Following;


        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public float GetFustrumHeight(float distance)
        {
            return 2.0f * distance * Mathf.Tan(CameraInstance.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        public float GetFustrumWidth(float distance)
        {
            return GetFustrumHeight(distance) * CameraInstance.aspect;
        }

        public Vector3[] GetFustrumBounds(float distance)
        {
            float height = GetFustrumHeight(distance);
            float width = GetFustrumWidth(distance);
            float x = CameraTransformInstance.position.x;
            float y = CameraTransformInstance.position.y;
            float z = CameraTransformInstance.position.z;

            Vector3 topLeft = new Vector3(x - width / 2, y - distance, z + height / 2);
            Vector3 topRight = new Vector3(x + width / 2, y - distance, z + height / 2);
            Vector3 botLeft = new Vector3(x - width / 2, y - distance, z - height / 2);
            Vector3 botRight = new Vector3(x + width / 2, y - distance, z - height / 2);

            return new[] {topRight, topLeft, botLeft, botRight};
        }
    }
}