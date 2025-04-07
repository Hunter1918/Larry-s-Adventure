using DG.Tweening;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 2f;
    public int damage = 10;
    public float duration = 0.5f;

    public SpriteRenderer flashRenderer;
    public Color flashColor = Color.yellow;
    public float flashTime = 0.15f;

    void Start()
    {
        Debug.Log("💥 Explosion stylée déclenchée !");
        if (flashRenderer != null)
        {
            flashRenderer.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
            flashRenderer.DOFade(0.8f, flashTime).SetLoops(2, LoopType.Yoyo);
        }

        Camera.main.transform.DOShakePosition(0.3f, 0.6f, 20, 90);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log("📏 Distance joueur : " + dist);

            if (dist <= radius)
            {
                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(damage);
                    Debug.Log("🔥 Joueur touché !");
                }
            }
        }

        Destroy(gameObject, duration);
    }
}