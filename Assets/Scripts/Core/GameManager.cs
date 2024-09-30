using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prez.Core
{
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        }
    }
}