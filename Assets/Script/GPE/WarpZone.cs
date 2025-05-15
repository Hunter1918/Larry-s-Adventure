using UnityEngine;

public class WarpZone : MonoBehaviour
{
    public Transform teleportDestination;
    public GameObject Level;
    public GameObject Level_Next;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.transform);
            Level.SetActive(false);
        }
    }

    private void TeleportPlayer(Transform player)
    {
        if (teleportDestination != null)
        {
            player.position = teleportDestination.position;
            Level_Next.SetActive(true);
        }
    }
}