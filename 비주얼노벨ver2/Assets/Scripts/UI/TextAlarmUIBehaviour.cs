using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAlarmUIBehaviour : MonoBehaviour
{
    public Text alramText;
    
    public void ActiveTextAlarm(string[] parameters)
    {
        gameObject.SetActive(true);
        NodeManager.Instance.dialogueBehaviour.isClickable = false;
        alramText.text = parameters[0];
    }

    public void DeactiveTextAlarm()
    {
        NodeManager.Instance.dialogueBehaviour.isClickable = true;
        gameObject.SetActive(false);
    }
}
