using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float maxHp;
    [SerializeField] private float damageMinVelocity;

    private float currentHp;
    private bool isDead = false;

    public ParticleSystem deathParticles;
    public Animator animator;

    public static event Action OnMonsterDied;

    private void Awake()
    {
        currentHp = maxHp;
        animator = GetComponent<Animator>();
    }

    public void Damaged(float hitDamage)
    {
        if (isDead) return; // 이미 죽은 상태라면 아무 작업도 하지 않음

        currentHp -= hitDamage;

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            animator.Play("SlimeDamaged");
        }
    }

    private void Die()
    {
        if (isDead) return; // Die가 중복 호출되지 않도록 확인

        isDead = true; // 죽음 상태 설정
        /*OnMonsterDied?.Invoke(); // 이벤트 호출
        Debug.Log("Monster died and event invoked");*/
        StartCoroutine(DestroySlime());
    }

    IEnumerator DestroySlime()
    {
        animator.Play("SlimeDead");
        yield return new WaitForSeconds(2f);
        OnMonsterDied?.Invoke(); // 이벤트 호출
        Debug.Log("Monster died and event invoked");
        Instantiate(deathParticles, transform.position, Quaternion.identity).Play();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (isDead) return; // 이미 죽은 상태라면 충돌 처리하지 않음

        float hitVelocity = other.relativeVelocity.magnitude;

        if (hitVelocity > damageMinVelocity)
        {
            Damaged(hitVelocity);
        }
    }
}
