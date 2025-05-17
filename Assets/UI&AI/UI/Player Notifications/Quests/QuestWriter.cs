using UnityEngine;
using TMPro;
using System.Collections;

public class QuestWriter : MonoBehaviour
{
    private TMP_Text textComponent;
    [SerializeField] private float typingSpeed = 0.05f;
    public static QuestWriter instance;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        instance = this;
        textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        // StartTyping(fullText);
    }

    public void StartTyping(string textToType)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(textToType));
    }

    private IEnumerator TypeText(string textToType)
    {
        textComponent.text = "";
        foreach (char c in textToType)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
