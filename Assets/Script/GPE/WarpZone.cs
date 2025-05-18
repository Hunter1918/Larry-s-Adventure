using UnityEngine;
using System.Collections;
using Cinemachine;

public class WarpZone : MonoBehaviour
{
    [Header("Téléportation & Niveaux")]
    public Transform teleportDestination;
    public GameObject Level;
    public GameObject Level_Next;
    public GameObject player;

    [Header("Caméras")]
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera transitionCam;

    [Header("Animation & Fade")]
    public Animator pageAnimator;
    public float fadeDuration = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HandleTeleportSequence());
        }
    }

    private IEnumerator HandleTeleportSequence()
    {
        // Désactive le joueur
        player.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        // Switch sur la caméra de transition
        mainCam.Priority = 0;
        transitionCam.Priority = 10;

        // Fade out du niveau actuel
        yield return StartCoroutine(FadeOut(Level));
        yield return new WaitForSeconds(2f);

        // Animation de page
        pageAnimator.gameObject.SetActive(true);
        pageAnimator.SetTrigger("TurnPage");
        yield return new WaitForSeconds(GetAnimationLength(pageAnimator, "TurnPage"));
        yield return new WaitForSeconds(2.5f);

        // Téléportation et niveau suivant
        player.transform.position = teleportDestination.position;
        Level_Next.SetActive(true);

        // Fade in du nouveau niveau
        yield return StartCoroutine(FadeIn(Level_Next));

        // Reactiver le joueur
        player.SetActive(true);

        // Revenir à la caméra principale
        transitionCam.Priority = 0;
        mainCam.Priority = 10;
        this.gameObject.SetActive(false);
    }

    private IEnumerator FadeOut(GameObject obj)
    {
        // On fade out le canvas s'il y a un CanvasGroup
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            cg.alpha = 0f;
        }

        // On désactive tout sauf le WarpZone et on cache visuellement le reste
        foreach (Transform child in obj.transform)
        {
            if (child.GetComponent<WarpZone>() != null)
            {
                // Juste désactiver le MeshRenderer du portail si on veut le cacher
                MeshRenderer mr = child.GetComponent<MeshRenderer>();
                if (mr != null) mr.enabled = false;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }


    private IEnumerator FadeIn(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            obj.SetActive(true);
            cg.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            cg.alpha = 1f;
        }
    }

    private float GetAnimationLength(Animator animator, string animationName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationName)
                return clip.length;
        }
        return 1f; // Fallback
    }
}
