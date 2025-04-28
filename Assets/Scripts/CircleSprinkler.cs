using UnityEngine;

public class CircleSprinkler : MonoBehaviour
{

    private int _count;
    public int maxCount = 200;

    public Bullet bullet; 
    public int bulletCount = 10;
    public float bulletSpeed = 1f;
    public float bulletCircleStartTheta = 0f;

    private void FixedUpdate()
    {
        _count -= 1;
        if (_count % maxCount != 0) return;
        for (var i = 0; i < bulletCount; i++)
        { 
            var theta = bulletCircleStartTheta + 2 * Mathf.PI * i / bulletCount;
            var fixPos = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            var newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            newBullet.speed = fixPos;
            newBullet.speedFactor = bulletSpeed;
        }
    }
    
}
