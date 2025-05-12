using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class First_Main_Menu : MonoBehaviour
{
    public GameObject Canva;
    void Start()
    {
        Canva.SetActive(true);
    }

    void Update()
    {
        if (Input.anyKeyDown)
            Canva.SetActive(false);
            // start animation
    }
}
