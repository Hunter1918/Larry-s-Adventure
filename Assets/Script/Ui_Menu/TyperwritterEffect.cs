using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;
    void Start()
    {
        StartTyping("Cette histoire commence dans une taverne, au coeur d’un village oublie par le temps… \r\nAh non, pardon, je me suis trompe d’histoire. Hrm hrm ! \r\nReprenons.\r\nCette histoire commence dans un conte de fees… mais pas n’importe lequel. On parle ici d’un livre oublie par le temps… que dis-je ! Un livre oublie par les personnages qui y vivent eux-memes.\r\nEh oui, vous etes bel et bien en train de lire Once Upon a Bone.\r\nCette histoire raconte l'aventure tragique de Larry, un squelette qui fut autrefois un humain. \r\nComme probablement vous, qui lisez ce recit.\r\nMais reprenons depuis le debut, au temps ou Larry etait encore fait de chair et de sang.");    }

    public void StartTyping(string message)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
