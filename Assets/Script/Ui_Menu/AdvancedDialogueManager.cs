using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;
[System.Serializable]
public class DialoguePage
{
    [TextArea(3, 10)]
    public string text;
    public Transform cameraTarget;
    public GameObject[] objectsToActivate;
}

public class AdvancedDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;

    [Header("Pages")]
    public DialoguePage[] pages;

    [Header("Params")]
    public float typingSpeed = 0.05f;
    public CinemachineVirtualCamera virtualCam;
    public float cameraMoveSpeed = 3f;

    private int currentPageIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isWaitingForNext = false;

    void Start()
    {
        ShowPage();
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
                currentPageIndex++;
                if (currentPageIndex < pages.Length)
                {
                    ShowPage();
                }
                else
                {
                    dialogueText.text = "";
                }
            }
        }
    }

    void ShowPage()
    {
        DialoguePage current = pages[currentPageIndex];

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(current.text));

        // Activer objets associés à cette page
        foreach (GameObject go in current.objectsToActivate)
        {
            go.SetActive(false); // on cache d'abord
        }

        // Bouger la caméra s’il y a une cible
        if (current.cameraTarget != null)
            StartCoroutine(MoveCameraTo(current.cameraTarget));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        isWaitingForNext = false;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Active objets une fois la page terminée
        foreach (GameObject go in pages[currentPageIndex].objectsToActivate)
        {
            go.SetActive(true);
        }

        isTyping = false;
        isWaitingForNext = true;
    }

    void SkipTyping()
    {
        StopCoroutine(typingCoroutine);
        dialogueText.text = pages[currentPageIndex].text;
        foreach (GameObject go in pages[currentPageIndex].objectsToActivate)
        {
            go.SetActive(true);
        }
        isTyping = false;
        isWaitingForNext = true;
    }

    IEnumerator MoveCameraTo(Transform target)
    {
        while (Vector3.Distance(virtualCam.transform.position, target.position) > 0.05f)
        {
            virtualCam.transform.position = Vector3.Lerp(
                virtualCam.transform.position,
                new Vector3(target.position.x, target.position.y, virtualCam.transform.position.z),
                Time.deltaTime * cameraMoveSpeed
            );
            yield return null;
        }
    }
}
