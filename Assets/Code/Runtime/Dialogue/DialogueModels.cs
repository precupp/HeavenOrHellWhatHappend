using System;

namespace HeavenOrHell.Dialogue
{
  [Serializable]
  public struct DialogueOption
  {
    public string choiceText;
    public string nextNodeId;

    public DialogueOption(string choiceText, string nextNodeId)
    {
      this.choiceText = choiceText;
      this.nextNodeId = nextNodeId;
    }
  }

  [Serializable]
  public class DialogueNode
  {
    public string id;
    public string speaker;
    public string line;
    public DialogueOption[] options;
    public string nextId;
    public bool endsConversation;

    public bool HasChoices => options != null && options.Length > 0;
  }

  public class DialogueGraph
  {
    public string witnessId;
    public string startNodeId;
    public DialogueNode[] nodes;

    public DialogueNode GetNode(string id)
    {
      foreach (var node in nodes)
      {
        if (node.id == id)
          return node;
      }

      return null;
    }
  }
}
