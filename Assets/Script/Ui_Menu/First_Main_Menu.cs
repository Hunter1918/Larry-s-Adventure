using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class First_Main_Menu : MonoBehaviour
{
    public GameObject Canva;
    public Animator animator;
    void Start()
    {
        Time.timeScale = 1f;
        Canva.SetActive(true);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Canva.SetActive(false);
            animator.SetTrigger("StartAnim");
        }
    }
}