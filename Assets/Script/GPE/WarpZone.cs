using UnityEngine;

public class WarpZone : MonoBehaviour
{
    public Transform teleportDestination; 


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.transform); 
        }
    }

    private void TeleportPlayer(Transform player)
    {
        if (teleportDestination != null)
        {
            player.position = teleportDestination.position;
        }
    }
}
