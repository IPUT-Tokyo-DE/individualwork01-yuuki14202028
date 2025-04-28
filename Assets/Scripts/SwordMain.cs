using System;
using UnityEngine;

public class SwordMain : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        transform.parent.GetComponent<Sword>().MainHit(other);
    }
    
}
