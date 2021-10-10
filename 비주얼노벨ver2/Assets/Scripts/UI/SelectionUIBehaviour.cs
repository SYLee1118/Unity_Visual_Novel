using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUIBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject[] selectionButtons;
    [SerializeField] private Text[] texts;
    [SerializeField] Image timerImage;

    private int[] destNodeID;

    WaitForSeconds delay02 = new WaitForSeconds(0.2f);

    private void Awake()
    {
        destNodeID = new int[3] { 0, 0, 0 };
    }

    // 선택지 활성화
    public void ActiveSelectionButtons(string[] parameters)
    {
        for(int i = 0; i < parameters.Length / 2; i++)
        {
            destNodeID[i] = int.Parse(parameters[2 * i]);
            texts[i].text = parameters[2 * i + 1];
            selectionButtons[i].SetActive(true);
        }
    }

    // 시간제한이 있는 선택지 활성화.
    public void ActiveSelectionButtons_TimeLimit(string[] parameters)
    {
        // 시간제한
        float timeLimitSceonds = float.Parse(parameters[0]);
        // 시간제한이 끝날경우 이동하는 node의 번호
        int timeLimitNodeID = int.Parse(parameters[1]);

        StartCoroutine(TimeLimit(timeLimitSceonds, timeLimitNodeID));

        // 시간제한, 시간제한시의 node 번호 두개의 parameter를 건너뛰고 읽음.
        for (int i = 0; i < (parameters.Length - 2) / 2; i++)
        {
            destNodeID[i] = int.Parse(parameters[2 + 2 * i]);
            texts[i].text = parameters[2 + 2 * i + 1];
            selectionButtons[i].SetActive(true);
        }
    }

    IEnumerator TimeLimit(float seconds, int nodeID)
    {
        float timer = 0.0f;
        timerImage.gameObject.SetActive(true);
        timerImage.fillAmount = 1.0f;
        while (true)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            timerImage.fillAmount = 1.0f - timer / seconds;

            if (timer >= seconds)
                break;
        }

        // 시간제한이 끝나버릴경우
        foreach (GameObject go in selectionButtons)
            go.SetActive(false);

        NodeManager.Instance.ExecuteNode(nodeID);
    }

    #region UI
    public void SelectionButtonEvent(int buttonIndex)
    {
        timerImage.gameObject.SetActive(false);
        foreach (GameObject go in selectionButtons)
            go.SetActive(false);
        StopAllCoroutines();
        NodeManager.Instance.ExecuteNode(destNodeID[buttonIndex]);
    }
    #endregion
}
