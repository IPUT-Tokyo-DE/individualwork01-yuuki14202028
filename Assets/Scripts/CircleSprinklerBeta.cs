using UnityEngine;

public class CircleSprinklerBeta : MonoBehaviour
{

    private int _count;
    public int maxCount = 20;

    public Bullet bullet; 
    public int bulletCount = 10;
    private int _bulletIndex;
    public float bulletSpeed = 1f;
    public float bulletCircleStartTheta;
        
    
    void FixedUpdate()
    {
        _count -= 1;
        if (_count % maxCount == 0)
        {
            var theta = bulletCircleStartTheta + 2 * Mathf.PI * _bulletIndex / bulletCount;
            var fixPos = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)); 
            Bullet newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            newBullet.speed = fixPos;
            newBullet.speedFactor = bulletSpeed;

            _bulletIndex++;
            if (_bulletIndex % bulletCount == 0)
            {
                _bulletIndex = 0;
            }
        }
    }
    
}
