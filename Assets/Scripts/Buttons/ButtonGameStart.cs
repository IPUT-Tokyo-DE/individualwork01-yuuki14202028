using UnityEngine;

namespace Buttons
{
    public class ButtonGameStart : MonoBehaviour
    {

        private PlayerInteractionButton _bpi;
        private GameManager _gm;

        private void Start()
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
