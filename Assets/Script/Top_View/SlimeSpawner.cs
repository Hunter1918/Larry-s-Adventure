using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    public GameObject[] slimePrefabs;

    public void SpawnRandomSlime(Vector2 position)
    {
        int index = Random.Range(0, slimePrefabs.Length);
        Instantiate(slimePrefabs[index], position, Quaternion.identity);
    }
}
