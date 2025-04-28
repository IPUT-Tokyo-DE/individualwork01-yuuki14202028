using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GaugeController : MonoBehaviour
{
    // 体力ゲージ（表面の常に見える部分）
    private RectTransform _gaugeRect;
    // 猶予ゲージ（体力が減ったとき一瞬見える部分）
    private RectTransform _graceGaugeRect;

    // 最大HP
    public int health;
    public int maxHealth;
    // HP1あたりの幅
    private float _deltaHealth;
    // 体力ゲージが減った後裏ゲージが減るまでの待機時間
    private const float WaitingTime = 0.5f;

    private void Awake() {
        // スプライトの幅を最大HPで割ってHP1あたりの幅を”_HP1”に入れておく
        _gaugeRect = transform.Find("Gauge").gameObject.GetComponent<RectTransform>();
        _graceGaugeRect = transform.Find("GraceGauge").gameObject.GetComponent<RectTransform>();
        _deltaHealth = _gaugeRect.sizeDelta.x / maxHealth;
    }

    // 攻撃力をそれぞれのボタンで設定
    public void Damage(int attack)
    {
        health -= attack;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
        
        StartCoroutine(DamageCoroutine(health * _deltaHealth));
    }

    public void Heal(int heal)
    {
        health += heal;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
        
        StartCoroutine(DamageCoroutine(health * _deltaHealth));
    }
    
    // 体力ゲージを減らすコルーチン
    private IEnumerator DamageCoroutine(float damage){
        // 体力ゲージの幅と高さをVector2で取り出す(Width,Height)
        var nowSafes = _gaugeRect.sizeDelta;
        // 体力ゲージの幅からダメージ分の幅を引く
        nowSafes.x = damage;
        // 体力ゲージに計算済みのVector2を設定する
        _gaugeRect.sizeDelta = nowSafes;

        // ”_waitingTime”秒待つ
        yield return new WaitForSeconds(WaitingTime);
        // 猶予ゲージに計算済みのVector2を設定する
        _graceGaugeRect.sizeDelta = nowSafes;
    }
}