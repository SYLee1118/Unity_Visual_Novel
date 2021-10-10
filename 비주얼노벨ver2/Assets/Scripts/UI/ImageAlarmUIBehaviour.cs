using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAlarmUIBehaviour : MonoBehaviour
{
    private GameObject currentImage;

    public void ActiveImageAlarm(string[] parameters)
    {
        gameObject.SetActive(true);
        NodeManager.Instance.dialogueBehaviour.isClickable = false;
        currentImage = transform.Find(parameters[0]).gameObject;
        currentImage.SetActive(true);
    }

    public void DeactiveImageAlarm()
    {
        NodeManager.Instance.dialogueBehaviour.isClickable = true;
        currentImage.SetActive(false);
        gameObject.SetActive(false);
    }
}
