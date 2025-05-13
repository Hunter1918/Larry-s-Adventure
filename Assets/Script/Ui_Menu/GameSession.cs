using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    public bool menuAlreadyShown = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
