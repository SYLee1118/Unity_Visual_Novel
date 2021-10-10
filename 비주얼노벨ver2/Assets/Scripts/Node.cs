using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData
{
    public CharacterBehaviour character { get; private set; }
    public int state { get; private set; }

    public CharacterData(CharacterBehaviour character, int state)
    {
        this.character = character;
        this.state = state;
    }
}

[System.Serializable]
public class Node
{
    public int nodeID = 0;
    public int destNodeID = 0;
    public string name = null;
    public string dialogue = null;
    public CharacterData[] characterData = new CharacterData[3];
    public int characterCount = 0;
    public string bgi = null;
    public string bgm = null;
    public string sfx = null;

    // 노드의 실행과 동시에 실행되는 커맨드
    public List<Command> preCommands = new List<Command>();
    // 노드에서 빠져나갈 때 실행되는 커맨드.
    public List<Command> postCommands = new List<Command>();
}
