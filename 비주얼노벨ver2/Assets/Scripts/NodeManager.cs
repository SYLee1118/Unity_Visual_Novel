using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("중복된 노드매니저 생성됨!");
            Destroy(this);
        }
    }

    private void Start()
    {
        GamePoint = 0;
        LoadData(chapterName);
    }

    public DialogueBehaviour dialogueBehaviour;

    [SerializeField]
    private string chapterName;
    [SerializeField]
    private GameObject bgParentObject;
    [SerializeField]
    private GameObject sgParentObject;
    [SerializeField]
    private HistoryScrollviewBehaviour historyScrollviewBehaviour;
    [SerializeField]
    private Text pointText;
    private Node currentNode;
    // 이번 챕터에서 쓰이는 오브젝트 사전
    public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
    // 이번 챕터의 노드 사전
    private Dictionary<int, Node> nodes = new Dictionary<int, Node>();

    private int gamePoint;
    public int GamePoint
    {
        get
        {
            return gamePoint;
        }
        set
        {
            pointText.text = "점수 : " + value.ToString() + "점";
            gamePoint = value;
        }
    }

    public void LoadData(string fileName)
    {
        TextAsset txtAsset_dialogue = (TextAsset)Resources.Load(fileName);
        XmlDocument xmlDoc_dialogue = new XmlDocument();
        xmlDoc_dialogue.LoadXml(txtAsset_dialogue.text);

        XmlNodeList nodeList_dialogue = xmlDoc_dialogue.SelectNodes("Root/text");

        foreach (XmlNode xmlNode in nodeList_dialogue)
        {
            // 대사 data 기본 정보들 로드.
            Node node = new Node();
            node.nodeID = int.Parse(xmlNode.SelectSingleNode("nodeID")?.InnerText);
            node.destNodeID = int.Parse(xmlNode.SelectSingleNode("destNodeID")?.InnerText);
            node.name = xmlNode.SelectSingleNode("name")?.InnerText;
            node.dialogue = xmlNode.SelectSingleNode("dialogue")?.InnerText;

            node.characterCount = 0;

            // 캐릭터 정보 로드.
            string ch1 = xmlNode.SelectSingleNode("캐릭터1")?.InnerText;
            if (ch1 != null)
            {
                // 최초로 로드되는 캐릭터의 경우, object 사전에 등록.
                if (objects.ContainsKey(ch1) == false)
                    AddDictionary(ch1, sgParentObject);
                // 캐릭터 표정 정보를 노드의 캐릭터 데이터에 등록.
                node.characterData[0] = new CharacterData(objects[ch1].GetComponent<CharacterBehaviour>(), int.Parse(xmlNode.SelectSingleNode("표정1")?.InnerText));
                node.characterCount++;
            }

            string ch2 = xmlNode.SelectSingleNode("캐릭터2")?.InnerText;
            if (ch2 != null)
            {
                if (objects.ContainsKey(ch2) == false)
                    AddDictionary(ch2, sgParentObject);
                node.characterData[1] = new CharacterData(objects[ch2].GetComponent<CharacterBehaviour>(), int.Parse(xmlNode.SelectSingleNode("표정2")?.InnerText));
                node.characterCount++;
            }

            string ch3 = xmlNode.SelectSingleNode("캐릭터3")?.InnerText;
            if (ch3 != null)
            {
                if (objects.ContainsKey(ch3) == false)
                    AddDictionary(ch3, sgParentObject);
                node.characterData[2] = new CharacterData(objects[ch3].GetComponent<CharacterBehaviour>(), int.Parse(xmlNode.SelectSingleNode("표정3")?.InnerText));
                node.characterCount++;
            }

            // bgi 정보 로드
            node.bgi = xmlNode.SelectSingleNode("bgi")?.InnerText;
            if (node.bgi != null)
            {
                if (!objects.ContainsKey(node.bgi))
                    AddDictionary(node.bgi, bgParentObject);
            }

            // bgm 정보 로드
            node.bgm = xmlNode.SelectSingleNode("bgm")?.InnerText;
            if (node.bgm != null)
            {
                if (!objects.ContainsKey(node.bgm))
                    AddDictionary(node.bgm, bgParentObject);
            }

            // sfx 정보 로드.
            node.sfx = xmlNode.SelectSingleNode("sfx")?.InnerText;
            if (node.sfx != null)
            {
                if (!objects.ContainsKey(node.sfx))
                    AddDictionary(node.sfx, bgParentObject);
            }

            // 최종적으로 만들어진 node를 사전에 등록.
            nodes.Add(node.nodeID, node);
        }

        TextAsset txtAsset_command = (TextAsset)Resources.Load(fileName + "_Command");
        XmlDocument xmlDoc_command = new XmlDocument();
        xmlDoc_dialogue.LoadXml(txtAsset_command.text);

        XmlNodeList nodeList_command = xmlDoc_dialogue.SelectNodes("Root/text");

        // Command 파일에서, 각 node의 preCommands, proCommands, 거기에 파라미터들까지 해당 node에 등록.
        foreach(XmlNode xmlNode in nodeList_command)
        {
            Command command = new Command();
            command.action = xmlNode.SelectSingleNode("action")?.InnerText;
            command.parameters = xmlNode.SelectSingleNode("parameters")?.InnerText.Split(',');
            if (xmlNode.SelectSingleNode("preORpost")?.InnerText == "pre")
                nodes[int.Parse(xmlNode.SelectSingleNode("nodeID")?.InnerText)].preCommands.Add(command);
            if (xmlNode.SelectSingleNode("preORpost")?.InnerText == "post")
                nodes[int.Parse(xmlNode.SelectSingleNode("nodeID")?.InnerText)].postCommands.Add(command);
        }

        // currentNode에 기본 데이터를 가진 node를 할당. null 오류 방지.
        currentNode = new Node();
        // 항상 1번 노드가 첫 노드.
        ExecuteNode(1);
    }

    private void AddDictionary(string prefabName, GameObject parentObject = null)
    {
        GameObject go = GameObject.Find(prefabName);
        // 씬에 존재하는지 체크해보고, 존재하지 않으면 warning과 동시에 일단 리소스 폴더에서 로드.
        // 타임라인 연출에 포함되는 오브젝트들은 미리 등록되어있는게 베스트.
        if (go == null)
        {
            Debug.LogWarning(prefabName + "이 씬에 존재하지 않음!");
            go = Instantiate(Resources.Load(prefabName)) as GameObject;
        }
        go.name = prefabName;
        if (parentObject != null)
        {
            go.transform.SetParent(parentObject.transform);
            go.transform.position = parentObject.transform.position;
        }
        go.SetActive(true);
        objects.Add(prefabName, go);
    }

    public void ExecuteNode(int destNodeID = 0)
    {
        // destNodeID의 값이 수동으로 주어지지 않을 경우, 현재 노드가 갖고있는 다음 node의 ID의 값을 할당.
        // 선택지, 히스토리로 이동 등 수동으로 이동해야할 때는 destNodeID를 수동으로 지정.
        if (destNodeID == 0)
            destNodeID = currentNode.destNodeID;

        // currentNode의 destNodeID도 0인 경우.
        if (destNodeID == 0)
        {
            Debug.LogWarning("다음 노드 연걸점이 없음.");
            // 다음 노드 연결점이 postCommands로 인해 결정되는 경우. (선택지나, 조건부 분기 등)
            // postCommands 발동 후 바로 return.
            if(currentNode.postCommands.Count > 0)
            {
                foreach(Command c in currentNode.postCommands)
                    CommandExecuter.Instance.Execute(c);
            }
            return;
        }

        // destNodeID가 존재하는 경우. (다음 노드가 존재하는 경우.)
        // 현재 노드의 postCommnads만 실행하고 정상적으로 다음 노드로의 프로세스를 진행.
        if (currentNode.postCommands.Count > 0 && currentNode.destNodeID != 0)
        {
            foreach (Command c in currentNode.postCommands)
                CommandExecuter.Instance.Execute(c);
        }

        Node nextNode = nodes[destNodeID];

        // 대사가 같지 않고, 비어있지 않으면(비어있다는건 CG준비씬 등의 경우이기 때문) 대사 타이핑
        if (currentNode.dialogue != nextNode.dialogue)
        {
            dialogueBehaviour.StartTyping(nextNode.name, nextNode.dialogue);
        }

        // 캐릭터 수에 따라서 캐릭터들 위치 설정.
        switch(nextNode.characterCount)
        {
            case 0:
                break;

            case 1:
                nextNode.characterData[0].character.transform.localPosition = new Vector3(0, 0, 0);
                break;

            case 2:
                nextNode.characterData[0].character.transform.localPosition = new Vector3(-400, 0, 0);
                nextNode.characterData[1].character.transform.localPosition = new Vector3(400, 0, 0);
                break;

            case 3:
                nextNode.characterData[0].character.transform.localPosition = new Vector3(-650, 0, 0);
                nextNode.characterData[1].character.transform.localPosition = new Vector3(0, 0, 0);
                nextNode.characterData[2].character.transform.localPosition = new Vector3(650, 0, 0);
                break;

            default:
                Debug.LogError("잘못된 캐릭터 수!");
                break;
        }

        // 현재 노드의 캐릭터들과 다음 노드의 캐릭터들 중 같은 캐릭터가 있는지 체크.
        // 다음 노드에 없어지는 캐릭터들은 fadeout처리.
        for(int i = 0; i < currentNode.characterCount; i++)
        {
            // 다음 노드에 애초에 캐릭터가 하나도 없으면, 따질 필요없이 전부 페이드 아웃.
            if (nextNode.characterCount == 0)
                currentNode.characterData[i].character.FadeOut();
            else
            {
                for (int k = 0; k < nextNode.characterCount; k++)
                {
                    // 같은 캐릭터가 있으면 break
                    if (currentNode.characterData[i].character == nextNode.characterData[k].character)
                        break;
                    // 끝까지 체크했는데도 같은 캐릭터가 없으면 fadeout처리.
                    else if (k == nextNode.characterCount - 1)
                        currentNode.characterData[i].character.FadeOut();
                }
            }
        }
        // 캐릭터 표정(상태) 설정
        for(int i = 0; i < nextNode.characterCount; i++)
            nextNode.characterData[i].character.SetState(nextNode.characterData[i].state);

        // 현재 node의 bgi와 다음 노드의 bgi가 일치하지 않으면 fadeout / in 효과로 전환.
        if (nextNode.bgi != currentNode.bgi)
        {
            if (currentNode.bgi != null)
                objects[currentNode.bgi].GetComponent<BGIBehaviour>().FadeOut();
            if (nextNode.bgi != null)
                objects[nextNode.bgi].GetComponent<BGIBehaviour>().FadeIn();
        }

        // bgi와 동일하게 체크.
        if(nextNode.bgm != currentNode.bgm)
        {
            if(currentNode.bgm != null)
                objects[currentNode.bgm].GetComponent<AudioSource>().Stop();
            if (nextNode.bgm != null && objects[nextNode.bgm].GetComponent<AudioSource>().isPlaying == false)
                objects[nextNode.bgm].GetComponent<AudioSource>().Play();
        }

        // sfx가 재생되는 중 스킵해버릴수도 있으므로, Stop.
        if (currentNode.sfx != null)
            objects[currentNode.sfx].GetComponent<AudioSource>().Stop();

        // 다음 sfx 재생.
        if (nextNode.sfx != null)
            objects[nextNode.sfx].GetComponent<AudioSource>().Play();

        currentNode = nextNode;

        // 다음 노드에의 진입시 실행되는 commands를 실행.
        if (currentNode.preCommands.Count > 0)
        {
            foreach (Command c in currentNode.preCommands)
                CommandExecuter.Instance.Execute(c);
        }
    }
}
