using System.Collections;
using UnityEngine;
using TMPro;

public class PagedDialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    [TextArea(5, 10)]
    public string fullText; // Ton gros texte complet

    [Header("Params")]
    public float typingSpeed = 0.05f;
    public int maxCharsPerPage = 400;

    private string[] pages;
    private int currentPageIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool isWaitingForNext = false;

    void Start()
    {
        pages = SplitIntoPages(fullText);
        ShowPage();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = pages[currentPageIndex];
                isTyping = false;
                isWaitingForNext = true;
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
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(pages[currentPageIndex]));
    }

    IEnumerator TypeText(string page)
    {
        isTyping = true;
        isWaitingForNext = false;
        dialogueText.text = "";

        foreach (char c in page)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        isWaitingForNext = true;
    }

    string[] SplitIntoPages(string text)
    {
        int totalLength = text.Length;
        int pageCount = Mathf.CeilToInt((float)totalLength / maxCharsPerPage);
        string[] result = new string[pageCount];

        for (int i = 0; i < pageCount; i++)
        {
            int start = i * maxCharsPerPage;
            int length = Mathf.Min(maxCharsPerPage, totalLength - start);
            result[i] = text.Substring(start, length);
        }

        return result;
    }
}
