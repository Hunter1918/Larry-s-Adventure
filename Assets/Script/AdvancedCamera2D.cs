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

    [Header("Zoom Dynamique")]
    public float zoomOutFactor = 1.5f; 
    public float zoomSpeed = 2f; 

    [Header("Effet Shake")]
    public float shakeIntensity = 2f;
    public float shakeDuration = 0.2f;

    // ===== VARIABLES PRIVÉES =====
    [SerializeField] private Vector3 velocity = Vector3.zero;
    private float defaultZoom;
    private CinemachineBasicMultiChannelPerlin noise;
    private bool isShaking = false;
    private float playerDirection = 1f;

    void Start()
    {
        if (virtualCam != null)
        {
            noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise != null) noise.m_AmplitudeGain = 0f; 

            defaultZoom = virtualCam.m_Lens.OrthographicSize;
        }
    }

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

        // ===== GESTION DU ZOOM DYNAMIQUE =====
        if (rb != null)
        {
            if (rb.velocity.y < -1f) 
            {
                virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(
                    virtualCam.m_Lens.OrthographicSize,
                    defaultZoom * zoomOutFactor,
                    Time.deltaTime * zoomSpeed
                );
            }
            else
            {
                virtualCam.m_Lens.OrthographicSize = Mathf.Lerp(
                    virtualCam.m_Lens.OrthographicSize,
                    defaultZoom,
                    Time.deltaTime * zoomSpeed
                );
            }
        }

        // ===== EFFET DE SHAKE À L'ATTERRISSAGE =====
        if (rb != null && rb.velocity.y == 0 && !isShaking)
        {
            float fallThreshold = -1f; 
            if (rb.velocity.y <= fallThreshold)
            {
                StartCoroutine(ShakeCamera());
            }
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / smoothSpeed);
    }

    private System.Collections.IEnumerator ShakeCamera()
    {
        isShaking = true;
        if (noise != null)
        {
            noise.m_AmplitudeGain = shakeIntensity;
            yield return new WaitForSeconds(shakeDuration);
            noise.m_AmplitudeGain = 0f; 
        }
        isShaking = false;
    }
}