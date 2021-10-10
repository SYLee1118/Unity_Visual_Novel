using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class BGIBehaviour : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    IEnumerator fadeIn()
    {
        if (image.enabled == true)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            yield break;
        }

        image.enabled = true;
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        for (int i = 0; i < 25; i++)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.04f * (float)i);
            yield return new WaitForSeconds(0.02f);
        }
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    IEnumerator fadeOut()
    {
        if (image.enabled == false)
            yield break;

        image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        for (int i = 25; i > 0; i--)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, 0.04f * (float)i);
            yield return new WaitForSeconds(0.02f);
        }
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        image.enabled = false;
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(fadeIn());
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(fadeOut());
    }

}
