using System;
using TMPro;
using UnityEngine;

public class PlayerInteractionButton : MonoBehaviour
{

    private bool _isHover;
    private TextMeshProUGUI _textMeshPro;
    public Action Action;

    private void Start()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
        _textMeshPro.color = Color.cyan;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isHover)
        {
            Action?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _isHover = true;
        _textMeshPro.color = Color.Lerp(Color.cyan, Color.black, 0.5f);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _isHover = false;
        _textMeshPro.color = Color.cyan;
    }
}

