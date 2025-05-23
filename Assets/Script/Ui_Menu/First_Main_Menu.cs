using System.Collections;
using UnityEngine;

public class First_Main_Menu : MonoBehaviour
{
    public GameObject CanvaLogo416; // 💡 Nouveau : Logo studio 416
    public GameObject CanvaIntro;
    public GameObject CanvaMenu;

    public Animator cameraAnimator;
    public Animator bookAnimator;

    private bool hasStarted = false;

    void Start()
    {
        Time.timeScale = 1f;

        CanvaLogo416.SetActive(true);
        CanvaIntro.SetActive(false);
        CanvaMenu.SetActive(false);

        cameraAnimator.SetBool("StartCam", false);
        bookAnimator.SetBool("StartAnim", false);

        StartCoroutine(LogoThenIntro());
    }

    IEnumerator LogoThenIntro()
    {
        yield return StartCoroutine(FadeInCanvas(CanvaLogo416, 1f));
        yield return new WaitForSeconds(2f); // Durée d’affichage du logo

        yield return StartCoroutine(FadeOutCanvas(CanvaLogo416, 1f));
        CanvaLogo416.SetActive(false);

        CanvaIntro.SetActive(true);
    }

    void Update()
    {
        if (!hasStarted && Input.anyKeyDown && CanvaIntro.activeSelf)
        {
            hasStarted = true;
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        CanvaIntro.SetActive(false);
        cameraAnimator.SetBool("StartCam", true);
        yield return new WaitForSeconds(4f);

        yield return new WaitForSeconds(0.1f);
        bookAnimator.SetBool("StartAnim", true);
        yield return new WaitForSeconds(4f);

        yield return StartCoroutine(FadeInCanvas(CanvaMenu, 1f));
        CanvaMenu.SetActive(true);
    }

    IEnumerator FadeInCanvas(GameObject target, float duration)
    {
        target.SetActive(true);

        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.AddComponent<CanvasGroup>();

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    IEnumerator FadeOutCanvas(GameObject target, float duration)
    {
        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = target.AddComponent<CanvasGroup>();

        cg.alpha = 1f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        cg.alpha = 0f;
        target.SetActive(false);
    }
}
