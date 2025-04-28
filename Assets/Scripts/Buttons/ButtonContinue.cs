using UnityEngine;
using UnityEngine.SceneManagement;

namespace Buttons
{
    public class ButtonContinue : MonoBehaviour
    {

        private GameManager _gm;
        private PlayerInteractionButton _bpi;
    
        void Start()
        {
            _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            _bpi = gameObject.GetComponent<PlayerInteractionButton>();
            _bpi.Action = Click;
        }

        private void Click()
        {
            _gm.GameStart();
        }
    }
}
