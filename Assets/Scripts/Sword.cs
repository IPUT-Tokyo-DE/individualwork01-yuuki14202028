using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    [Header("突き刺し設定")]
    public float thrustSpeed = 10f;       // 突き刺しのスピード
    public float returnSpeed = 5f;        // 元の位置に戻るスピード
    public float thrustDistance = 2f;     // 突き刺し距離
    
    private bool _isThrusting;     // 突き刺し中かどうか
    public bool canThrusting = true;
    
    private GameManager _gameManager;

    public Sprite shockWaveSprite;
    public Sprite sparkSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        canThrusting = true;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void MainHit(Collider2D other)
    {
         if (!_gameManager.inGame) return;
         if (other.gameObject.CompareTag("Player"))
         {
             other.gameObject.GetComponent<PlayerController>().Damage(4);
         } else if (other.gameObject.CompareTag("SwordGuard"))
         {
             transform.parent.GetComponent<Enemy>().Sacrifice();
             var cr = other.gameObject.GetComponentInParent<Sword>().DamageEffect();
             StartCoroutine(cr);
         }
    }

    private IEnumerator DamageEffect()
    {
        // 光の輪を作成（衝撃波効果）
        var shockwave = new GameObject("ShockwaveEffect")
        {
            transform =
            {
                position = transform.position
            }
        };
        var shockwaveRenderer = shockwave.AddComponent<SpriteRenderer>();
        
        // シンプルな円形スプライトを設定（実際のプロジェクトでは適切なスプライトを使用）
        shockwaveRenderer.sprite = shockWaveSprite;
        shockwaveRenderer.color = new Color(1f, 1f, 1f, 0.7f);
        
        // 衝撃波の拡大と消失
        const float shockwaveDuration = 0.3f;
        var elapsed = 0f;
        var initialScale = new Vector3(0.1f, 0.1f, 0.1f);
        var finalScale = new Vector3(2.0f, 2.0f, 2.0f);
        shockwave.transform.localScale = initialScale;
        
        while (elapsed < shockwaveDuration)
        {
            var t = elapsed / shockwaveDuration;
            
            // 拡大
            shockwave.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
            
            // フェードアウト
            var color = shockwaveRenderer.color;
            color.a = Mathf.Lerp(0.7f, 0.0f, t);
            shockwaveRenderer.color = color;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 衝撃波オブジェクトを削除
        Destroy(shockwave);
        
        // パーティクル効果（スパークのような小さな粒子）
        for (var i = 0; i < 10; i++)
        {
            var spark = new GameObject("Spark_" + i)
            {
                transform =
                {
                    position = transform.position
                }
            };
            var sparkRenderer = spark.AddComponent<SpriteRenderer>();
            
            // 小さな点のスプライトを設定
            sparkRenderer.sprite = sparkSprite;
            sparkRenderer.color = new Color(1f, 0.8f, 0.3f, 1f); // 黄色っぽい色
            spark.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            
            // ランダムな方向と速度
            var direction = Random.insideUnitCircle.normalized;
            var speed = Random.Range(1f, 3f);
            var lifetime = Random.Range(0.2f, 0.5f);
            
            StartCoroutine(MoveSpark(spark, direction, speed, lifetime));
        }
    }
    
    private static IEnumerator MoveSpark(GameObject spark, Vector2 direction, float speed, float lifetime)
    {
        var elapsed = 0f;
        var spriteRenderer = spark.GetComponent<SpriteRenderer>();
        
        while (elapsed < lifetime)
        {
            // 移動
            spark.transform.position += (Vector3)(direction * (speed * Time.deltaTime));
            
            // フェードアウト
            if (spriteRenderer)
            {
                var color = spriteRenderer.color;
                color.a = Mathf.Lerp(1.0f, 0.0f, elapsed / lifetime);
                spriteRenderer.color = color;
            }
            
            // サイズ減少
            spark.transform.localScale *= (1.0f - Time.deltaTime / lifetime);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // パーティクルを削除
        Destroy(spark);
    }

    public void GuardHit(Collider2D other)
    {
        if (_isThrusting)
        {
            _isThrusting = false;
        }
    }

    /// <summary>
    /// ターゲットの周囲の円周上のランダムな位置に剣を突き刺します
    /// </summary>
    /// <param name="target">ターゲットの位置</param>
    /// <param name="radius">円の半径</param>
    /// <returns>コルーチン</returns>
    public IEnumerator ThrustToRandomPositionAround(Transform target, float radius)
    {
        if (!canThrusting) yield break;

        _isThrusting = true;
        canThrusting = false;

        // 現在の位置と回転を保存
        var startPosition = transform.position;
        var startRotation = transform.rotation;

        // ターゲットの周囲の円周上のランダムな位置を計算（2D用）
        var randomAngle = Random.Range(0f, 360f);
        var radians = randomAngle * Mathf.Deg2Rad;
        var offset = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
        var targetPosition = target.position + new Vector3(offset.x, offset.y, 0);

        // 突き刺す方向を計算（2D用）
        var direction = (new Vector2(targetPosition.x, targetPosition.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
        
        // 向きを突き刺す方向に合わせる（2D用）- atan2を使用
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var targetRotation = Quaternion.Euler(0, 0, angle + 90); // -90は剣の向きによる調整

        // 向きを徐々に変える
        const float rotationTime = 0.2f;
        var elapsedTime = 0f;
        while (elapsedTime < rotationTime)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;

        // 突き刺す（2D用）
        var thrustDirection = new Vector3(direction.x, direction.y, 0);
        var thrustTarget = transform.position + thrustDirection * thrustDistance;
        elapsedTime = 0f;
        var thrustTime = thrustDistance / thrustSpeed;
        
        while (elapsedTime < thrustTime && _isThrusting)
        {
            transform.position = Vector3.Lerp(startPosition, thrustTarget, elapsedTime / thrustTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 少し待機
        yield return new WaitForSeconds(0.5f);
        
        // 元の位置に戻る
        elapsedTime = 0f;
        var returnTime = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(startPosition.x, startPosition.y)) / returnSpeed;
        
        while (elapsedTime < returnTime)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, elapsedTime / returnTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, startRotation, elapsedTime / returnTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        _isThrusting = false;
        canThrusting = true;
    }
}