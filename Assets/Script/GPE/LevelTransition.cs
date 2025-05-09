using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [Header("GameObjects à désactiver")]
    public GameObject[] objectsToDisable;

    [Header("GameObjects à activer")]
    public GameObject[] objectsToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            foreach (GameObject obj in objectsToEnable)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }
}