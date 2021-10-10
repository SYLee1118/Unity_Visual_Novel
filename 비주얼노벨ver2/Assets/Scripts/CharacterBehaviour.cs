using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject[] states;

    private int currentState = 0;

    public void SetState(int state)
    {
        if (currentState == state)
        {
            if (states[currentState].activeSelf == false)
                StartCoroutine(FadeIn(state));
            else
                return;
        }
        else
        {
            StopAllCoroutines();
            states[currentState].SetActive(false);
            StartCoroutine(FadeOut(currentState));
            StartCoroutine(FadeIn(state));
            currentState = state;
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOut(currentState));
    }

    IEnumerator FadeIn(int state)
    {
        states[state].SetActive(true);
        Image image = states[state].GetComponent<Image>();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        for (int i = 0; i < 10; i++)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.10f * (float)i);
            yield return new WaitForSeconds(0.02f);
        }
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    IEnumerator FadeOut(int state)
    {
        Image image = states[state].GetComponent<Image>();
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        for (int i = 10; i > 0; i--)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.10f * (float)i);
            yield return new WaitForSeconds(0.02f);
        }
        states[state].SetActive(false);
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }
}
