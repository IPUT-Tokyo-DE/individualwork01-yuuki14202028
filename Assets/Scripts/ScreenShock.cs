using System.Collections;
using UnityEngine;

public class ScreenShock : MonoBehaviour
{
    
    public IEnumerator CameraShake(float x, float y, int count, float duration)
    {
        for (var i=0; i<count; i++)
        {
            transform.Translate(x, y, 0);
            x *= -1;
            y *= -1;
            yield return new WaitForSeconds(duration);
        }
    }
    
}
