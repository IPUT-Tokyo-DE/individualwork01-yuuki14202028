using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameManager manager;
    public float moveSpeed = 0.1f;
    public int maxHealth = 20;
    public int health = 20;
    
    public float jumpForce = 6f;
    private int _jumpCount = 1;
    public int maxJumpCount = 1;
    
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    
    private TextMeshProUGUI _healthText;
    private bool _isGravity;
    
    private ScreenShock _screenShock;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _healthText = GameObject.Find("HPAmount").GetComponent<TextMeshProUGUI>();
        _healthText.text = "" + health + " / " + maxHealth;
        SetGravity(false);
        _screenShock = GameObject.Find("MainCamera").GetComponent<ScreenShock>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetGravity(!_isGravity);
        }

        if (_jumpCount > 0 && _isGravity && Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _jumpCount -= 1;
        }
        
    }

    private void FixedUpdate()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var pos = transform.position;
        if (!_isGravity)
        {
            pos.y += vertical * moveSpeed;
        }
        pos.x += horizontal * moveSpeed;
        transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _jumpCount = maxJumpCount;
        }
    }

    public void SetGravity(bool gravity)
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _isGravity = gravity;
        _rigidbody2D.gravityScale = gravity ? 1f : 0f;
        _spriteRenderer.color = gravity ? new Color(0f, 0f, 1f, 1f) : new Color(1f, 0f, 0f, 1f);
    }

    public void Damage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        } else if (health > maxHealth)
        {
            health = maxHealth;
        }
        _healthText.text = "" + health + " / " + maxHealth;
        var cameraShake = _screenShock.CameraShake(0.1f, 0.1f, 6, 0.02f);
        StartCoroutine(cameraShake);
        if (health <= 0)
        {
            SetGravity(false);
            manager.GameOver();
        }
    }

    public void Heal(int heal)
    {
        health += heal;
        if (health > maxHealth)
        {
            health = maxHealth;
        } else if (health < 0)
        {
            health = 0;
        }
        _healthText.text = "" + health + " / " + maxHealth;
    }
    
    public int GetHealth() => health;
}
