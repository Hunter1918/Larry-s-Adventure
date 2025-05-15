using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeVaEtVient : MonoBehaviour
{
    public Transform pointDeDepart;
    public Transform pointDArrivee;
    public float vitesse = 2f;

    private Vector3 destination;

    void Start()
    {
        destination = pointDeDepart.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, vitesse * Time.deltaTime);

        if (transform.position == destination)
        {
            if (destination == pointDeDepart.position)
            {
                destination = pointDArrivee.position;
            }
            else
            {
                destination = pointDeDepart.position;
            }
        }
    }
}