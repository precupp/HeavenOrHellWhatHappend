using System.Collections;
using HeavenOrHell.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace HeavenOrHell.UI
{
  /// <summary>
  /// Full-screen VR dialogue panel with speaker, text, continue and choice buttons.
  /// </summary>
  public class DialogueUiController : MonoBehaviour
  {
    private CanvasGroup rootGroup;
    private TextMeshProUGUI speakerText;
    private TextMeshProUGUI bodyText;
    private Button continueButton;
    private readonly Button[] choiceButtons = new Button[6];
    private readonly TextMeshProUGUI[] choiceLabels = new TextMeshProUGUI[6];

    private bool continuePressed;
    private int selectedChoiceIndex = -1;
    private DialogueOption pickedOption;

    public IEnumerator RunDialogue(DialogueGraph graph)
    {
      BuildUiIfNeeded();
      yield return ShowRoot();

      var nodeId = graph.startNodeId;
      while (!string.IsNullOrEmpty(nodeId))
      {
        var node = graph.GetNode(nodeId);
        if (node == null)
          break;

        if (node.HasChoices)
        {
          if (!string.IsNullOrEmpty(node.line))
            yield return ShowLine(node.speaker, node.line, false);

          yield return WaitForChoice(node.options);
          nodeId = pickedOption.nextNodeId;
          continue;
        }

        yield return ShowLine(node.speaker, node.line, !node.endsConversation);
        if (node.endsConversation)
          break;

        nodeId = node.nextId;
      }

      yield return HideRoot();
    }

    public IEnumerator ShowLine(string speaker, string line, bool waitForContinue)
    {
      BuildUiIfNeeded();
      SetChoicesVisible(false);
      continueButton.gameObject.SetActive(waitForContinue);

      speakerText.text = speaker ?? string.Empty;
      speakerText.color = GetSpeakerColor(speaker);
      bodyText.text = line ?? string.Empty;

      if (!waitForContinue)
        yield break;

      continuePressed = false;
      while (!continuePressed)
        yield return null;
    }

    public IEnumerator ShowSummonIntro(string witnessName, string chapterId, string introLine)
    {
      BuildUiIfNeeded();
      yield return ShowRoot();
      speakerText.text = witnessName;
      speakerText.color = chapterId == "hell" ? new Color(1f, 0.45f, 0.25f) : new Color(0.55f, 0.78f, 1f);
      bodyText.text = introLine;
      SetChoicesVisible(false);
      continueButton.gameObject.SetActive(true);
      continuePressed = false;
      while (!continuePressed)
        yield return null;
    }

    private IEnumerator WaitForChoice(DialogueOption[] options)
    {
      continueButton.gameObject.SetActive(false);
      speakerText.text = "Detective";
      speakerText.color = GetSpeakerColor("Detective");
      bodyText.text = "Choose your question:";

      var count = Mathf.Min(options.Length, choiceButtons.Length);
      for (var i = 0; i < choiceButtons.Length; i++)
      {
        var visible = i < count;
        choiceButtons[i].gameObject.SetActive(visible);
        if (!visible)
          continue;
        choiceLabels[i].text = options[i].choiceText;
      }

      selectedChoiceIndex = -1;
      while (selectedChoiceIndex < 0)
        yield return null;

      pickedOption = options[selectedChoiceIndex];
      SetChoicesVisible(false);
    }

    private void OnContinueClicked() => continuePressed = true;

    private void OnChoiceClicked(int index) => selectedChoiceIndex = index;

    private void Update()
    {
      // UI is built lazily on first dialogue; nothing to poll before that.
      if (rootGroup == null)
        return;

      if (continueButton != null && continueButton.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
        OnContinueClicked();

      if (selectedChoiceIndex >= 0)
        return;

      for (var i = 0; i < choiceButtons.Length; i++)
      {
        if (!choiceButtons[i].gameObject.activeInHierarchy)
          continue;
        if (i < 6 && Input.GetKeyDown(KeyCode.Alpha1 + i))
          OnChoiceClicked(i);
      }
    }

    private void SetChoicesVisible(bool visible)
    {
      foreach (var button in choiceButtons)
      {
        if (button != null)
          button.gameObject.SetActive(visible);
      }
    }

    private IEnumerator ShowRoot()
    {
      rootGroup.gameObject.SetActive(true);
      yield return Fade(rootGroup, 1f, 0.3f);
    }

    private IEnumerator HideRoot()
    {
      yield return Fade(rootGroup, 0f, 0.35f);
      rootGroup.gameObject.SetActive(false);
    }

    private static IEnumerator Fade(CanvasGroup group, float target, float duration)
    {
      var start = group.alpha;
      var elapsed = 0f;
      while (elapsed < duration)
      {
        elapsed += Time.deltaTime;
        group.alpha = Mathf.Lerp(start, target, elapsed / duration);
        yield return null;
      }

      group.alpha = target;
    }

    private static Color GetSpeakerColor(string speaker)
    {
      if (string.IsNullOrEmpty(speaker))
        return Color.white;
      if (speaker.StartsWith("Angel"))
        return new Color(0.62f, 0.82f, 1f);
      if (speaker.StartsWith("Demon"))
        return new Color(1f, 0.42f, 0.28f);
      return new Color(0.92f, 0.92f, 0.95f);
    }

    private void BuildUiIfNeeded()
    {
      if (rootGroup != null)
        return;

      EnsureEventSystem();

      var cameraTransform = Camera.main != null ? Camera.main.transform : transform;
      var canvasGo = new GameObject("DialogueHudCanvas");
      canvasGo.transform.SetParent(cameraTransform, false);
      canvasGo.transform.localPosition = VrHudLayout.MainHudPosition();
      canvasGo.transform.localRotation = Quaternion.identity;
      canvasGo.transform.localScale = Vector3.one * VrHudLayout.CanvasScale;

      var canvas = canvasGo.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.WorldSpace;
      canvas.sortingOrder = 60;
      canvasGo.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 100f;
      canvasGo.AddComponent<GraphicRaycaster>();
      canvasGo.AddComponent<TrackedDeviceGraphicRaycaster>();

      var panel = CreatePanel(canvasGo.transform, "DialoguePanel", new Vector2(0.5f, 0.5f), new Vector2(VrHudLayout.MainPanelWidth, VrHudLayout.DialoguePanelHeight), new Color(0.03f, 0.04f, 0.08f, 0.92f));
      rootGroup = panel.AddComponent<CanvasGroup>();

      speakerText = CreateAnchoredText(panel.transform, "Speaker", "Witness", 28, FontStyles.Bold, TextAlignmentOptions.Center, new Vector2(0.05f, 0.8f), new Vector2(0.95f, 0.95f), new Color(0.7f, 0.85f, 1f));
      bodyText = CreateAnchoredText(panel.transform, "Body", "", 31, FontStyles.Normal, TextAlignmentOptions.Center, new Vector2(0.05f, 0.22f), new Vector2(0.95f, 0.78f), Color.white);
      // Long witness lines shrink to fit the panel instead of spilling out of the FOV.
      bodyText.enableAutoSizing = true;
      bodyText.fontSizeMin = 16f;
      bodyText.fontSizeMax = 31f;

      continueButton = CreateAnchoredButton(panel.transform, "ContinueButton", "Continue", new Vector2(0.5f, 0.08f), new Vector2(220f, 50f), OnContinueClicked);

      for (var i = 0; i < choiceButtons.Length; i++)
      {
        var y = -58f - i * 52f;
        var index = i;
        choiceButtons[i] = CreateButton(panel.transform, $"Choice{i}", "", new Vector2(24f, y), new Vector2(1102f, 50f), () => OnChoiceClicked(index));
        choiceLabels[i] = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        choiceLabels[i].enableAutoSizing = true;
        choiceLabels[i].fontSizeMin = 14f;
        choiceLabels[i].fontSizeMax = 22f;
        choiceButtons[i].gameObject.SetActive(false);
      }

      rootGroup.alpha = 0f;
      panel.SetActive(false);
    }

    private static void EnsureEventSystem()
    {
      if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
        return;

      var es = new GameObject("EventSystem");
      es.AddComponent<UnityEngine.EventSystems.EventSystem>();
      es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    private static GameObject CreatePanel(Transform parent, string name, Vector2 anchor, Vector2 size, Color color)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(Image));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = anchor;
      rect.anchorMax = anchor;
      rect.pivot = new Vector2(0.5f, 0.5f);
      rect.anchoredPosition = new Vector2(0f, VrHudLayout.MainPanelDownOffset);
      rect.sizeDelta = size;
      go.GetComponent<Image>().color = color;
      return go;
    }

    private static TextMeshProUGUI CreateAnchoredText(
      Transform parent,
      string name,
      string content,
      float size,
      FontStyles style,
      TextAlignmentOptions align,
      Vector2 anchorMin,
      Vector2 anchorMax,
      Color color)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = anchorMin;
      rect.anchorMax = anchorMax;
      rect.offsetMin = Vector2.zero;
      rect.offsetMax = Vector2.zero;

      var text = go.GetComponent<TextMeshProUGUI>();
      text.text = content;
      text.fontSize = size;
      text.fontStyle = style;
      text.alignment = align;
      text.color = color;
      text.enableWordWrapping = true;
      text.raycastTarget = false;
      return text;
    }

    private static Button CreateAnchoredButton(
      Transform parent,
      string name,
      string label,
      Vector2 anchor,
      Vector2 size,
      UnityEngine.Events.UnityAction onClick,
      Vector2? stretchMin = null,
      Vector2? stretchMax = null)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();

      if (stretchMin.HasValue && stretchMax.HasValue)
      {
        rect.anchorMin = stretchMin.Value;
        rect.anchorMax = stretchMax.Value;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
      }
      else
      {
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
      }

      var image = go.GetComponent<Image>();
      image.color = new Color(0.12f, 0.16f, 0.24f, 0.95f);

      var button = go.GetComponent<Button>();
      button.targetGraphic = image;
      button.onClick.AddListener(onClick);

      var textGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
      textGo.transform.SetParent(go.transform, false);
      var textRect = textGo.GetComponent<RectTransform>();
      textRect.anchorMin = Vector2.zero;
      textRect.anchorMax = Vector2.one;
      textRect.offsetMin = new Vector2(12f, 0f);
      textRect.offsetMax = new Vector2(-12f, 0f);

      var text = textGo.GetComponent<TextMeshProUGUI>();
      text.text = label;
      text.fontSize = 22;
      text.alignment = TextAlignmentOptions.Center;
      text.color = Color.white;
      text.enableWordWrapping = true;
      text.raycastTarget = false;
      return button;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string content, float size, FontStyles style, TextAlignmentOptions align, Vector2 pos, Vector2 rectSize, Color color)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = new Vector2(0f, 1f);
      rect.anchorMax = new Vector2(0f, 1f);
      rect.pivot = new Vector2(0f, 1f);
      rect.anchoredPosition = pos;
      rect.sizeDelta = rectSize;
      var text = go.GetComponent<TextMeshProUGUI>();
      text.text = content;
      text.fontSize = size;
      text.fontStyle = style;
      text.alignment = align;
      text.color = color;
      text.enableWordWrapping = true;
      return text;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction onClick)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = new Vector2(0f, 1f);
      rect.anchorMax = new Vector2(0f, 1f);
      rect.pivot = new Vector2(0f, 1f);
      rect.anchoredPosition = pos;
      rect.sizeDelta = size;

      var image = go.GetComponent<Image>();
      image.color = new Color(0.12f, 0.16f, 0.24f, 0.95f);

      var button = go.GetComponent<Button>();
      button.targetGraphic = image;
      button.onClick.AddListener(onClick);

      var text = CreateText(go.transform, "Label", label, 22, FontStyles.Normal, TextAlignmentOptions.MidlineLeft, new Vector2(12f, 0f), new Vector2(size.x - 20f, size.y), Color.white);
      text.alignment = TextAlignmentOptions.MidlineLeft;
      text.raycastTarget = false;
      return button;
    }
  }
}
