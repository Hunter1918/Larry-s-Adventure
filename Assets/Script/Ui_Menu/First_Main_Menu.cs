using System.Collections;
using UnityEngine;

public class First_Main_Menu : MonoBehaviour
{
    public GameObject CanvaIntro;   // Le Canvas d’intro ("Appuyez sur une touche")
    public GameObject CanvaMenu;    // Le Canvas du menu principal final

    public Animator cameraAnimator; // Animator de la caméra
    public Animator bookAnimator;   // Animator du livre

    private bool hasStarted = false;

    void Start()
    {
        Time.timeScale = 1f;
        CanvaIntro.SetActive(true);
        CanvaMenu.SetActive(false);

        // Reset sécurité (au cas où les bool restent bloqués d’un ancien play)
        cameraAnimator.SetBool("StartCam", false);
        bookAnimator.SetBool("StartAnim", false);
    }

    void Update()
    {
        if (!hasStarted && Input.anyKeyDown)
        {
            hasStarted = true;
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        Debug.Log("➡️ Fermeture du canvas d'intro");
        CanvaIntro.SetActive(false);

        Debug.Log("🎥 Lancement anim caméra");
        cameraAnimator.SetBool("StartCam", true);
        yield return new WaitForSeconds(4f); // adapte la durée si nécessaire

        Debug.Log("📖 Lancement anim livre");
        yield return new WaitForSeconds(0.1f); // avant de faire SetBool
        bookAnimator.SetBool("StartAnim", true);

        yield return new WaitForSeconds(4f); // adapte aussi

        Debug.Log("✅ Affichage du menu");
        CanvaMenu.SetActive(true);
    }
}