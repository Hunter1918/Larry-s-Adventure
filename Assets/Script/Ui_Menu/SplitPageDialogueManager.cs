using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;

[System.Serializable]
public class DialogueSplitPage
{
    [TextArea(3, 10)] public string leftText;
    [TextArea(3, 10)] public string rightText;
    public Transform leftCamTarget;
    public Transform rightCamTarget;
    public GameObject[] objectsToActivate;
}

public class SplitPageDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI leftTextUI;
    public TextMeshProUGUI rightTextUI;

    [Header("Pages")]
    public DialogueSplitPage[] pages;

    [Header("Params")]
    public float typingSpeed = 0.02f;
    public CinemachineVirtualCamera virtualCam;
    public float cameraMoveSpeed = 3f;

    [Header("Transition Fin")]
    public Camera mainCamera;                // Caméra principale à activer
    public GameObject playerToActivate;      // Le joueur à activer
    public GameObject uiToDisable;           // Le canvas UI dialogue à cacher
    public GameObject levelToActivate;       // Le niveau 1 à activer

    private int currentPageIndex = 0;
    private bool onLeft = true;
    private bool isTyping = false;
    private bool isWaitingForNext = false;
    private Coroutine typingCoroutine;

    [Header("Choix du pacte")]
    public GameObject choixUI; // UI avec les boutons Oui / Non
    public string phraseDeclencheur = "Il lui fit une offre : en echange de son ame";


    void Start()
    {
        // 🔁 Caméra
        virtualCam.gameObject.SetActive(true);
        virtualCam.Priority = 20;

        // 🧹 Reset propre
        currentPageIndex = 0;
        onLeft = true;
        isTyping = false;
        isWaitingForNext = false;

        // 🛑 Force nettoyage texte
        leftTextUI.text = "";
        rightTextUI.text = "";

        // 📍 Positionne la caméra à gauche
        MoveCameraTo(pages[0].leftCamTarget);

        // 📝 Lance le texte gauche
        ShowLeftText();
        if (choixUI != null)
            choixUI.SetActive(false);

    }





    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else if (isWaitingForNext)
            {
                if (onLeft)
                {
                    ShowRightText();
                }
                else
                {
                    currentPageIndex++;
                    if (currentPageIndex < pages.Length)
                    {
                        ShowLeftText();
                    }
                    else
                    {
                        EndDialogueAndStartLevel();
                    }
                }
            }
        }
    }

    void ShowLeftText()
    {
        onLeft = true;
        leftTextUI.text = "";
        rightTextUI.text = "";
        isWaitingForNext = false;
        typingCoroutine = StartCoroutine(TypeText(pages[currentPageIndex].leftText, leftTextUI));
        MoveCameraTo(pages[currentPageIndex].leftCamTarget);
    }

    void ShowRightText()
    {
        onLeft = false;
        isWaitingForNext = false;
        typingCoroutine = StartCoroutine(TypeText(pages[currentPageIndex].rightText, rightTextUI));
        MoveCameraTo(pages[currentPageIndex].rightCamTarget);

        foreach (GameObject go in pages[currentPageIndex].objectsToActivate)
        {
            go.SetActive(true);
        }
    }

    IEnumerator TypeText(string text, TextMeshProUGUI target)
    {
        isTyping = true;
        target.text = "";
        foreach (char c in text.ToCharArray())
        {
            target.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        string texteActuel = (onLeft ? pages[currentPageIndex].leftText : pages[currentPageIndex].rightText);

        if (texteActuel.Contains(phraseDeclencheur))
        {
            choixUI.SetActive(true);
            isWaitingForNext = false; // on bloque la suite ici
        }
        else
        {
            isWaitingForNext = true;
        }

    }

    void SkipTyping()
    {
        StopCoroutine(typingCoroutine);

        if (onLeft)
            leftTextUI.text = pages[currentPageIndex].leftText;
        else
            rightTextUI.text = pages[currentPageIndex].rightText;

        foreach (GameObject go in pages[currentPageIndex].objectsToActivate)
        {
            go.SetActive(true);
        }

        isTyping = false;

        // 🛡️ Nouvelle logique de sécurité pour bloquer suite si phrase spéciale
        string texteActuel = (onLeft ? pages[currentPageIndex].leftText : pages[currentPageIndex].rightText);
        if (texteActuel.Contains(phraseDeclencheur))
        {
            choixUI.SetActive(true);
            isWaitingForNext = false;
        }
        else
        {
            isWaitingForNext = true;
        }
    }


    void MoveCameraTo(Transform target)
    {
        StartCoroutine(MoveCamSmooth(target));
    }

    IEnumerator MoveCamSmooth(Transform target)
    {
        Vector3 camTarget = new Vector3(target.position.x, target.position.y, virtualCam.transform.position.z);
        while (Vector3.Distance(virtualCam.transform.position, camTarget) > 0.05f)
        {
            virtualCam.transform.position = Vector3.Lerp(virtualCam.transform.position, camTarget, Time.deltaTime * cameraMoveSpeed);
            yield return null;
        }
    }

    void EndDialogueAndStartLevel()
    {
        leftTextUI.text = "";
        rightTextUI.text = "";

        if (uiToDisable != null)
            uiToDisable.SetActive(false);

        if (playerToActivate != null)
            playerToActivate.SetActive(true);

        if (levelToActivate != null)
            levelToActivate.SetActive(true);

        if (virtualCam != null)
            virtualCam.gameObject.SetActive(false);

        if (mainCamera != null)
            mainCamera.enabled = true;
    }
    public void ChoixAccepter()
    {
        choixUI.SetActive(false);
        isWaitingForNext = true; // permet de continuer
    }

    public void ChoixRefuser()
    {
        choixUI.SetActive(false);
        // TODO : charger une scène ou afficher fin
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu"); // à adapter selon ton projet
    }

}
