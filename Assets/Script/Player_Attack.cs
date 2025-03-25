using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    public GameObject AttackArea = default;
    private bool attacking = false;

    private float attackTime = 0.25f;
    private float timer = 0f;
    void Start()
    {
        AttackArea = transform.GetChild(0).gameObject;
        AttackArea.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
        if (attacking)
        {
            timer += Time.deltaTime;
            if (timer >= attackTime)
            {
                timer = 0f;
                attacking = false;
                AttackArea.SetActive(false);
            }
        }
    }

    void Attack()
    {
        attacking = true;
        AttackArea.SetActive(attacking);
    }
}
