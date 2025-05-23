using UnityEngine;
using Cinemachine;

public class AdvancedCamera2D : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public CinemachineVirtualCamera virtualCam;

    [Header("Vitesse et Suivi")]
    public float smoothSpeed = 5f;
    public float horizontalLag = 0.2f;

    [Header("Offsets")]
    public float verticalOffset = 2f;
    public float horizontalOffset = 2f;

    [Header("Limites de caméra")]
    public Vector2 minCameraPos;
    public Vector2 maxCameraPos;

    [SerializeField] private Vector3 velocity = Vector3.zero;
    private float playerDirection = 1f;



    void LateUpdate()
    {
        if (player == null || virtualCam == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null && Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            playerDirection = Mathf.Sign(rb.velocity.x);
        }

        // Position cible avant clamping
        Vector3 targetPosition = new Vector3(
            Mathf.Lerp(transform.position.x, player.position.x + (horizontalOffset * playerDirection), horizontalLag),
            player.position.y + verticalOffset,
            transform.position.z
        );

        // Appliquer SmoothDamp
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / smoothSpeed);

        // Clamp dans les limites définies
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minCameraPos.x, maxCameraPos.x);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minCameraPos.y, maxCameraPos.y);

        transform.position = smoothedPosition;
    }
}
