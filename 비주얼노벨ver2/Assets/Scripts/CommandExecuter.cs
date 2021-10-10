using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Command
{
    public string action;
    public string[] parameters;
}

public class CommandExecuter : MonoBehaviour
{
    public static CommandExecuter Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("중복된 커맨드실행자 생성됨!");
            Destroy(this);
        }
    }

    [SerializeField] private SelectionUIBehaviour selectionUIBehaviour;
    [SerializeField] private DialogueBehaviour dialogueBehaviour;
    [SerializeField] private TextAlarmUIBehaviour alarmUIBehaviour;
    [SerializeField] private ImageAlarmUIBehaviour imageAlarmUIBehaviour;
    [SerializeField] private PlayableDirector cutScene_1;
    [SerializeField] private InventoryScrollViewBehaviour inventory;

    private Dictionary<string, Action<string[]>> actionDictionary = new Dictionary<string, Action<string[]>>(); 

    public void Execute(Command command)
    {
        actionDictionary[command.action](command.parameters);
    }

    private void Start()
    {
        void cutScene_1_EndEvent()
        {
            NodeManager.Instance.ExecuteNode();
            cutScene_1.Stop();
        }
        cutScene_1.stopped += delegate { cutScene_1_EndEvent(); };

        SetActionDictionary();
    }

    #region Commands
    private void SetActionDictionary()
    {
        actionDictionary.Add("ActiveSelectionUI", ActiveSelectionUI);
        actionDictionary.Add("ActiveSelectionUI_TimeLimit", ActiveSelectionUI_TimeLimit);
        actionDictionary.Add("ActiveTextAlarm", ActiveTextAlarm);
        actionDictionary.Add("GetPoint", GetPoint);
        actionDictionary.Add("SelectNextNodeAccordingToPoint", SelectNextNodeAccordingToPoint);
        actionDictionary.Add("CutScene_1", CutScene_1);
        actionDictionary.Add("ActiveImageAlarm", ActiveImageAlarm);
        actionDictionary.Add("GetItem", GetItem);
        actionDictionary.Add("UseItem", UseItem);
    }

    private void ActiveSelectionUI(string[] parameters)
    {
        selectionUIBehaviour.ActiveSelectionButtons(parameters);
    }

    private void ActiveSelectionUI_TimeLimit(string[] parameters)
    {
        selectionUIBehaviour.ActiveSelectionButtons_TimeLimit(parameters);
    }

    private void ActiveTextAlarm(string[] parameters)
    {
        alarmUIBehaviour.ActiveTextAlarm(parameters);
    }

    private void GetPoint(string[] parameters)
    {
        NodeManager.Instance.GamePoint += int.Parse(parameters[0]);
    }

    private void SelectNextNodeAccordingToPoint(string[] parameters)
    {
        int pointCondition = int.Parse(parameters[0]);
        int lowPointNodeID = int.Parse(parameters[1]);
        int highPointNodeID = int.Parse(parameters[2]);

        if (NodeManager.Instance.GamePoint < pointCondition)
            NodeManager.Instance.ExecuteNode(lowPointNodeID);
        else
            NodeManager.Instance.ExecuteNode(highPointNodeID);
    }

    public void CutScene_1(string[] parameters)
    {
        // 타임라인 재생시 대사창을 깔끔하게 비워두도록 공백칸이 됨.
        // 컷씬1이 끝나자말자 빈 대사칸에서 다음으로 넘어가도록 이벤트가 추가되어있음. (Start함수에)
        NodeManager.Instance.objects["BGM02"].GetComponent<AudioSource>().PlayDelayed(0.4f);
        cutScene_1.Play();
    }

    public void ActiveImageAlarm(string[] parameters)
    {
        imageAlarmUIBehaviour.ActiveImageAlarm(parameters);
    }

    public void GetItem(string[] parameters)
    {
        NodeManager.Instance.dialogueBehaviour.isClickable = false;
        inventory.Add(parameters[0]);
    }

    public void UseItem(string[] parameters)
    {
        // 해당 아이템을 보유하고 있을 경우
        if (inventory.Remove(parameters[0]) == true)
            NodeManager.Instance.ExecuteNode(int.Parse(parameters[1]));
        // 해당 아이템이 없을 경우
        else
            NodeManager.Instance.ExecuteNode(int.Parse(parameters[2]));
    }
    #endregion
}
