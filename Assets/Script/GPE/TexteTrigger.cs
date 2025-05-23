using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TexteTrigger : MonoBehaviour
{
    public GameObject Texte;
    public BoxCollider2D boxCollider;


    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            Texte.SetActive(true);
        }


    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Texte.SetActive(false);
    }
}
