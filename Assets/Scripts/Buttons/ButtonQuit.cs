using UnityEngine;

namespace Buttons
{
    public class ButtonQuit : MonoBehaviour
    {

        private PlayerInteractionButton _bpi;

        private void Start()
        {
            _bpi = gameObject.GetComponent<PlayerInteractionButton>();
            _bpi.Action = Click;
        }

        private void Click()
        {
            Application.Quit();
        }
    }
}
