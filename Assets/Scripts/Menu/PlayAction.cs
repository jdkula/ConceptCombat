using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class PlayAction : MonoBehaviour {

        public void OnClick()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
