using UnityEngine;
using Cinemachine;

public class AdvancedCamera2D : MonoBehaviour
{
    // ===== VARIABLES PUBLIQUES =====
    [Header("Références")]
    public Transform player; 
    public CinemachineVirtualCamera virtualCam; 

    [Header("Vitesse et Suivi")]
    public float smoothSpeed = 5f;
    public float horizontalLag = 0.2f; 

    [Header("Offsets")]
    public float verticalOffset = 2f;
    public float horizontalOffset = 2f;

    // ===== VARIABLES PRIVÉES =====
    [SerializeField] private Vector3 velocity = Vector3.zero;
    //private float defaultZoom;
    //private CinemachineBasicMultiChannelPerlin noise;
    //private bool isShaking = false;
    private float playerDirection = 1f;

    /*void Start()
    {
        if (virtualCam != null)
        {
            noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null) noise.m_AmplitudeGain = 0f; 

            defaultZoom = virtualCam.m_Lens.OrthographicSize;
        }
    }*/

    void Update()
    {
        if (player == null || virtualCam == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null && Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            playerDirection = Mathf.Sign(rb.velocity.x); 
        }

        Vector3 targetPosition = new Vector3(
            Mathf.Lerp(transform.position.x, player.position.x + (horizontalOffset * playerDirection), horizontalLag),
            player.position.y + verticalOffset,
            transform.position.z
        );


        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / smoothSpeed);
    }
}