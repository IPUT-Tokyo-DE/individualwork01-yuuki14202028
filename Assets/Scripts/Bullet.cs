using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public Vector2 speed;
    public float speedFactor = 1f;
    public Vector2 acceleration;
    public float accelerationFactor = 1f;
    
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        speed += acceleration * (accelerationFactor * 0.05f);
        var pos = transform.position;
        pos.x += speed.x * speedFactor * 0.05f;
        pos.y += speed.y * speedFactor * 0.05f;
        
        var eulerAngles = transform.eulerAngles;
        eulerAngles.z = Mathf.Atan2(speed.normalized.y, speed.normalized.x) * Mathf.Rad2Deg - 90f;
        transform.eulerAngles = eulerAngles;
        transform.position = pos;
        
        if (pos.x > 12 || pos.y > 6 || pos.x < -12 || pos.y < -6)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_gameManager.inGame) return;
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.GetComponent<PlayerController>().Damage(1);
        Destroy(gameObject);
    }
    
}

