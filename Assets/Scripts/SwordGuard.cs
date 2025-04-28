using System;
using UnityEngine;

public class SwordGuard : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        transform.parent.GetComponent<Sword>().GuardHit(other);
    }
    
}
