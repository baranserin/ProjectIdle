using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI tutorialText;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    [TextArea(3, 10)]
    public List<string> tutorialSteps = new List<string>();

    private int currentStep = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    void Start()
    {
        if (tutorialSteps.Count > 0)
        {
            StartStep();
        }
    }

    void StartStep()
    {
        if (currentStep >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }

        typingCoroutine = StartCoroutine(TypeText(tutorialSteps[currentStep]));
    }

    IEnumerator TypeText(string fullText)
    {
        isTyping = true;

        tutorialText.text = fullText;
        tutorialText.maxVisibleCharacters = 0;

        int totalCharacters = tutorialText.textInfo.characterCount;

        while (tutorialText.maxVisibleCharacters < totalCharacters)
        {
            tutorialText.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void OnClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            tutorialText.maxVisibleCharacters = tutorialText.textInfo.characterCount;
            isTyping = false;
        }
        else
        {
            currentStep++;
            StartStep();
        }
    }

    void EndTutorial()
    {
        Debug.Log("Tutorial Finished");
        gameObject.SetActive(false); // tutorial panel kapanýr
    }
}
