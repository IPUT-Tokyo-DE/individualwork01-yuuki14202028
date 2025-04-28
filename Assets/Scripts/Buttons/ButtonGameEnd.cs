using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttons
{
    public class ButtonGameEnd : MonoBehaviour
    {

        private PlayerInteractionButton _bpi;
    
        void Start()
        {
            _bpi = gameObject.GetComponent<PlayerInteractionButton>();
            _bpi.Action = Click;
        }

        void Click()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
