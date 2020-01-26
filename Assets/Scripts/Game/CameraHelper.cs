using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    /// <summary>
    /// Helps with centering the camera by getting its width/height/size at a given distance.
    /// </summary>
    public class CameraHelper : MonoBehaviour
    {
        public Camera cameraInstance;
        public Transform cameraTransformInstance;

        public float GetFustrumHeight(float distance)
        {
            return 2.0f * distance * Mathf.Tan(cameraInstance.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        public float GetFustrumWidth(float distance)
        {
            return GetFustrumHeight(distance) * cameraInstance.aspect;
        }

        public Vector3[] GetFustrumBounds(float distance)
        {
            float height = GetFustrumHeight(distance);
            float width = GetFustrumWidth(distance);
            float x = cameraTransformInstance.position.x;
            float y = cameraTransformInstance.position.y;
            float z = cameraTransformInstance.position.z;

            Vector3 topLeft = new Vector3(x - width / 2, y - distance, z + height / 2);
            Vector3 topRight = new Vector3(x + width / 2, y - distance, z + height / 2);
            Vector3 botLeft = new Vector3(x - width / 2, y - distance, z - height / 2);
            Vector3 botRight = new Vector3(x + width / 2, y - distance, z - height / 2);

            return new[] {topRight, topLeft, botLeft, botRight};
        }
    }
}