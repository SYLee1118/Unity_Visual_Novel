using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HistoryScrollviewBehaviour : MonoBehaviour
{
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private GameObject logPrefab;

    private List<int> alreadyCreatedID = new List<int>();

    public void AddLog(int nodeID, string name, string dialogue)
    {
        // 히스토리로 다시 돌아갔을 때 로그가 중복 등록되는것을 방지하기 위해서
        // 한번 추가한적이 있는지 기록.
        if (alreadyCreatedID.Contains(nodeID) == true)
            return;

        contentTransform.sizeDelta = new Vector2(contentTransform.rect.width, contentTransform.rect.height + 100.0f);
        GameObject newLog = Instantiate(logPrefab, contentTransform.transform);
        newLog.GetComponent<RectTransform>().localPosition = new Vector3(650, -50.0f + -1.0f * contentTransform.rect.height + 100.0f, 0);
        newLog.transform.Find("name").GetComponent<Text>().text = name;
        newLog.transform.Find("Dialogue").GetComponent<Text>().text = dialogue;
        alreadyCreatedID.Add(nodeID);
    }
}
