using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    private readonly List<Sword> _swords = new();
    private PlayerController _player;
    
    private int _count;
    public int maxCount = 600;
    
    public float orbitSpeed = 4f;
    public float radius = 6f;

    public int hp = 10;
    public int maxHp = 10;
    
    private float _invincibleTime;
    public float invincibleDuration = 1.5f;
    private bool IsInvincible => _invincibleTime > 0f;
    
    private GameManager _gameManager;
    private TextMeshProUGUI _message;
    private GaugeController _gauge;
    
    
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _swords.AddRange(gameObject.GetComponentsInChildren<Sword>());
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _message = transform.Find("Canvas/Message").GetComponent<TextMeshProUGUI>();
        _gauge = transform.Find("Canvas/HealthGauge").GetComponent<GaugeController>();

        hp = maxHp;
        
        foreach (var (sword, i) in _swords.Select((item, index) => (item, index)))
        {
            var angle = (Time.time * orbitSpeed + 360f * i / _swords.Count) % 360 * Mathf.Deg2Rad; // 各剣に90度ずつずらした角度を割り当て

            var x = _player.transform.position.x + Mathf.Cos(angle) * radius;
            var y = _player.transform.position.y + Mathf.Sin(angle) * radius;

            // 剣の位置を更新
            sword.transform.position = new Vector3(x, y, sword.transform.position.z);

            // 剣が向きを更新（剣の先端がプレイヤーの方を向くように）
            var direction = new Vector2(_player.transform.position.x - x, _player.transform.position.y - y).normalized;
            var rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            sword.transform.rotation = Quaternion.Euler(0, 0, rotationAngle + 90);
        }
        
    }

    private void Update()
    {
        switch (_invincibleTime)
        {
            // 無敵時間のカウントダウン
            case > 0:
                _invincibleTime -= Time.deltaTime;
                break;
            case < 0 when hp > 0:
                _message.text = "わ！";
                break;
        }
    }
    
    private void FixedUpdate()
    {
        _count += _gameManager.inGame && _swords.All(sword => sword.canThrusting) ? 1 : 0;
        if (_count >= maxCount)
        {
            _count = 0;
            foreach (var coroutine in _swords.Select(sword => sword.ThrustToRandomPositionAround(_player.transform, 1f)))
            {
                StartCoroutine(coroutine);
            }
        } else if (_count != 0)
        {
            foreach (var (sword, i) in _swords.Select((item, index) => (item, index)))
            {
                // 各剣に少しずつ異なる角度と半径を割り当てて自然な動きを実現
                var angle = (Time.time * orbitSpeed + (360f * i / _swords.Count)) % 360 * Mathf.Deg2Rad; // 各剣に90度ずつずらした角度を割り当て
                var individualRadius = radius + Mathf.Sin(Time.time + i) * 0.5f; // 少し揺らぎを加える
                                    
                var x = _player.transform.position.x + Mathf.Cos(angle) * individualRadius;
                var y = _player.transform.position.y + Mathf.Sin(angle) * individualRadius;
                                    
                // 剣の位置を更新
                var targetPosition = new Vector3(x, y, sword.transform.position.z);
                sword.transform.position = Vector3.Lerp(sword.transform.position, targetPosition, Time.deltaTime * 2.0f);
                                    
                // 剣が向きを更新（剣の先端がプレイヤーの方を向くように）
                var direction = new Vector2(_player.transform.position.x - x, _player.transform.position.y - y).normalized;
                var rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(0, 0, rotationAngle + 90);
                sword.transform.rotation = Quaternion.Slerp(sword.transform.rotation, targetRotation, Time.deltaTime * 3.0f);
            }
        }
    }

    public void Sacrifice()
    {
        // 無敵状態の場合はダメージを受けない
        if (IsInvincible)
            return;
            
        hp -= 1;
        _gauge.Damage(1);
        _message.text = "いたい！";
        
        // ダメージを受けたら無敵時間を設定
        _invincibleTime = invincibleDuration;
        
        
        if (hp <= 0)
        {
            _message.text = "うわぁぁぁ";
            StartCoroutine(DeathEffect());
        }
    }
    
    private IEnumerator DeathEffect()
    {
        // ゲームマネージャーに通知
        _gameManager.inGame = false;
        
        // 剣の動きを止める
        foreach (var sword in _swords)
        {
            sword.canThrusting = false;
        }
        
        // 敵キャラクターを揺らす
        var duration = 1.5f;
        var elapsed = 0f;
        var originalPosition = transform.position;
        var originalScale = transform.localScale;
        
        while (elapsed < duration)
        {
            // 揺れ効果
            var xOffset = Random.Range(-0.2f, 0.2f);
            var yOffset = Random.Range(-0.2f, 0.2f);
            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);
            
            // 回転効果
            transform.Rotate(0, 0, Random.Range(-5f, 5f));
            
            // 経過時間を更新
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 剣を飛び散らせる
        foreach (var sword in _swords)
        {
            StartCoroutine(ScatterSword(sword));
        }
        
        // 敵を縮小させながら消滅
        elapsed = 0f;
        duration = 2.0f;
        while (elapsed < duration)
        {
            var scale = Mathf.Lerp(1.0f, 0.0f, elapsed / duration);
            transform.localScale = originalScale * scale;
            
            // フェードアウト効果
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                var color = spriteRenderer.color;
                color.a = Mathf.Lerp(1.0f, 0.0f, elapsed / duration);
                spriteRenderer.color = color;
            }
            
            // 回転効果を加速
            transform.Rotate(0, 0, Time.deltaTime * 180f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 敵を破壊
        Destroy(gameObject);
        _gameManager.GameClear();
    }
    
    private static IEnumerator ScatterSword(Sword sword)
    {
        if (!sword) yield break;
        
        // ランダムな方向と速度を設定
        var direction = Random.insideUnitCircle.normalized;
        var speed = Random.Range(5f, 10f);
        var rotationSpeed = Random.Range(180f, 360f);
        
        const float duration = 1.5f;
        var elapsed = 0f;
        
        while (elapsed < duration)
        {
            // 剣を移動
            sword.transform.position += (Vector3)(direction * (speed * Time.deltaTime));
            
            // 剣を回転
            sword.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            // 速度を徐々に遅くする
            speed = Mathf.Lerp(speed, 0, Time.deltaTime * 2f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
