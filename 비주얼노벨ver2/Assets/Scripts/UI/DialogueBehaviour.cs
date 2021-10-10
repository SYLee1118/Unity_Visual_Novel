using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBehaviour : MonoBehaviour
{
    [SerializeField]
    Text nameText;
    [SerializeField]
    Text dialogueText;

    private string dialogue;
    private bool isTyping = false;
    public bool isClickable { get; set; } = true;

    private IEnumerator TypingDialogue()
    {
        isTyping = true;
        dialogueText.text = "";
        foreach(char c in dialogue)
        {
            dialogueText.text += c;
            // 띄어쓰기까지 딜레이를 주면 어색해보임. 띄어쓰기일 경우 딜레이를 주지 않도록.
            if (c == '\u00A0')
                continue;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
    }

    public void StartTyping(string name, string dialogue)
    {
        if (name == null)
            nameText.text = "";
        else
            nameText.text = name;

        if (dialogue == null)
            dialogue = "";

        // 줄바꿈 문제 해결용
        this.dialogue = dialogue.Replace(' ', '\u00A0');

        StartCoroutine(TypingDialogue());
    }

    public void SkipTyping()
    {
        StopAllCoroutines();
        isTyping = false;
        dialogueText.text = dialogue;
    }

    #region UI
    public void ClickTextboxEvent()
    {
        if (isClickable == false)
            return;

        if (isTyping == true)
            SkipTyping();
        else
            NodeManager.Instance.ExecuteNode();
    }
    #endregion
}
