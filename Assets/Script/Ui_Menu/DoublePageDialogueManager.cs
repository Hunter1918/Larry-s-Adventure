using System.Collections;
using UnityEngine;
using TMPro;
using Cinemachine;

[System.Serializable]
public class DialogueDoublePage
{
    [TextArea(3, 10)] public string leftText;
    [TextArea(3, 10)] public string rightText;
    public Transform cameraTarget;
    public GameObject[] objectsToActivate;
}

public class DoublePageDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI leftTextUI;
    public TextMeshProUGUI rightTextUI;

    [Header("Pages")]
    public DialogueDoublePage[] pages;

    [Header("Params")]
    public float typingSpeed = 0.02f;
    public CinemachineVirtualCamera virtualCam;
    public float cameraMoveSpeed = 3f;

    private int currentPageIndex = 0;
    private bool isTyping = false;
    private bool isWaitingForNext = false;
    private Coroutine typingCoroutine;

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
                    ShowPage();
                else
                    ClearAll();
            }
        }
    }

    void ShowPage()
    {
        DialogueDoublePage current = pages[currentPageIndex];

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(current.leftText, current.rightText));

        foreach (GameObject go in current.objectsToActivate)
            go.SetActive(false);

        if (current.cameraTarget != null)
            StartCoroutine(MoveCameraTo(current.cameraTarget));
    }

    IEnumerator TypeText(string left, string right)
    {
        isTyping = true;
        isWaitingForNext = false;
        leftTextUI.text = "";
        rightTextUI.text = "";

        foreach (char c in left.ToCharArray())
        {
            leftTextUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        foreach (char c in right.ToCharArray())
        {
            rightTextUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        foreach (GameObject go in pages[currentPageIndex].objectsToActivate)
            go.SetActive(true);

        isTyping = false;
        isWaitingForNext = true;
    }

    void SkipTyping()
    {
        StopCoroutine(typingCoroutine);
        var current = pages[currentPageIndex];
        leftTextUI.text = current.leftText;
        rightTextUI.text = current.rightText;
        foreach (GameObject go in current.objectsToActivate)
            go.SetActive(true);

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

    void ClearAll()
    {
        leftTextUI.text = "";
        rightTextUI.text = "";
    }
}
