using UnityEngine;

public class WarpZone : MonoBehaviour
{
    //public Material newSkyboxMaterial; // Skybox material � changer
    public Transform teleportDestination; // Destination de t�l�portation du joueur


    private void OnCollisionEnter2D(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //ChangeSkybox();
            TeleportPlayer(other.transform); // Passer le transform du joueur � la fonction
        }
    }

    /*private void ChangeSkybox()
    {
        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox = newSkyboxMaterial;
        }
    }*/

    private void TeleportPlayer(Transform player)
    {
        if (teleportDestination != null)
        {
            player.position = teleportDestination.position;
        }
    }
}
