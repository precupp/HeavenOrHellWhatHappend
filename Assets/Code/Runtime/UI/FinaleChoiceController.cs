using System;
using System.Collections;
using HeavenOrHell.Story;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace HeavenOrHell.UI
{
  public enum FinaleVerdict
  {
    Heaven,
    Hell,
    Condemn
  }

  /// <summary>
  /// Presents the three finale verdict choices and shows the ending text.
  /// </summary>
  public class FinaleChoiceController : MonoBehaviour
  {
    [SerializeField] private StoryProgressionManager storyManager;
    [SerializeField] private StoryHudController hudController;

    public event Action<FinaleVerdict> OnVerdictChosen;

    private CanvasGroup rootGroup;
    private TextMeshProUGUI promptText;
    private readonly Button[] choiceButtons = new Button[3];
    private bool choiceMade;
    private int selectedIndex = -1;

    private void OnEnable()
    {
      if (storyManager != null)
        storyManager.OnStoryComplete += HandleStoryComplete;
    }

    private void OnDisable()
    {
      if (storyManager != null)
        storyManager.OnStoryComplete -= HandleStoryComplete;
    }

    private void HandleStoryComplete()
    {
      StartCoroutine(PresentFinaleChoices());
    }

    private IEnumerator PresentFinaleChoices()
    {
      BuildUiIfNeeded();
      yield return Fade(rootGroup, 1f, 0.35f);

      promptText.text = StoryNarrativeContent.FinalePrompt;
      SetChoice(0, "Let them remain in Heaven", true);
      SetChoice(1, "Send them back to Hell", true);
      SetChoice(2, "Condemn the debtor", true);

      choiceMade = false;
      selectedIndex = -1;
      while (!choiceMade)
        yield return null;

      yield return Fade(rootGroup, 0f, 0.3f);
      rootGroup.gameObject.SetActive(false);

      var verdict = (FinaleVerdict)selectedIndex;
      var endingText = verdict switch
      {
        FinaleVerdict.Heaven => StoryNarrativeContent.EndingHeaven,
        FinaleVerdict.Hell => StoryNarrativeContent.EndingHell,
        _ => StoryNarrativeContent.EndingCondemn
      };

      if (hudController != null)
        yield return hudController.ShowEnding(endingText);

      OnVerdictChosen?.Invoke(verdict);
      Debug.Log($"Finale: Verdict chosen — {verdict}");
    }

    private void SetChoice(int index, string label, bool visible)
    {
      choiceButtons[index].gameObject.SetActive(visible);
      if (visible)
        choiceButtons[index].GetComponentInChildren<TextMeshProUGUI>().text = label;
    }

    private void OnChoice(int index)
    {
      selectedIndex = index;
      choiceMade = true;
    }

    private void Update()
    {
      if (rootGroup == null || !rootGroup.gameObject.activeInHierarchy)
        return;

      if (Input.GetKeyDown(KeyCode.Alpha1)) OnChoice(0);
      if (Input.GetKeyDown(KeyCode.Alpha2)) OnChoice(1);
      if (Input.GetKeyDown(KeyCode.Alpha3)) OnChoice(2);
    }

    private void BuildUiIfNeeded()
    {
      if (rootGroup != null)
        return;

      EnsureEventSystem();

      var cameraTransform = Camera.main != null ? Camera.main.transform : transform;
      var canvasGo = new GameObject("FinaleChoiceCanvas");
      canvasGo.transform.SetParent(cameraTransform, false);
      canvasGo.transform.localPosition = VrHudLayout.MainHudPosition();
      canvasGo.transform.localScale = Vector3.one * VrHudLayout.CanvasScale;

      var canvas = canvasGo.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.WorldSpace;
      canvas.sortingOrder = 70;
      canvasGo.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 100f;
      canvasGo.AddComponent<GraphicRaycaster>();
      canvasGo.AddComponent<TrackedDeviceGraphicRaycaster>();

      var panel = CreatePanel(canvasGo.transform, new Vector2(VrHudLayout.MainPanelWidth, VrHudLayout.FinalePanelHeight), new Color(0.03f, 0.04f, 0.08f, 0.94f));
      rootGroup = panel.AddComponent<CanvasGroup>();

      promptText = CreateAnchoredText(panel.transform, "Prompt", StoryNarrativeContent.FinalePrompt, 32, FontStyles.Normal, TextAlignmentOptions.Center, new Vector2(0.05f, 0.58f), new Vector2(0.95f, 0.92f));
      promptText.enableAutoSizing = true;
      promptText.fontSizeMin = 16f;
      promptText.fontSizeMax = 32f;

      var labels = new[]
      {
        "Let them remain in Heaven",
        "Send them back to Hell",
        "Condemn the debtor"
      };

      for (var i = 0; i < choiceButtons.Length; i++)
      {
        var index = i;
        var y = -190f - i * 62f;
        choiceButtons[i] = CreateButton(panel.transform, $"FinaleChoice{i}", labels[i], new Vector2(24f, y), new Vector2(1102f, 58f), () => OnChoice(index));
      }

      rootGroup.alpha = 0f;
      panel.SetActive(false);
    }

    private static IEnumerator Fade(CanvasGroup group, float target, float duration)
    {
      group.gameObject.SetActive(true);
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

    private static void EnsureEventSystem()
    {
      if (UnityEngine.Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
        return;

      var es = new GameObject("EventSystem");
      es.AddComponent<UnityEngine.EventSystems.EventSystem>();
      es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    private static GameObject CreatePanel(Transform parent, Vector2 size, Color color)
    {
      var go = new GameObject("FinalePanel", typeof(RectTransform), typeof(Image));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
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
      Vector2 anchorMax)
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
      text.color = Color.white;
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
      text.fontSize = 26;
      text.alignment = TextAlignmentOptions.Center;
      text.color = Color.white;
      text.enableWordWrapping = true;
      text.raycastTarget = false;
      return button;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string content, float size, FontStyles style, TextAlignmentOptions align, Vector2 pos, Vector2 rectSize)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
      rect.pivot = new Vector2(0f, 1f);
      rect.anchoredPosition = pos;
      rect.sizeDelta = rectSize;
      var text = go.GetComponent<TextMeshProUGUI>();
      text.text = content;
      text.fontSize = size;
      text.fontStyle = style;
      text.alignment = align;
      text.color = Color.white;
      text.enableWordWrapping = true;
      return text;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction onClick)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
      go.transform.SetParent(parent, false);
      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
      rect.pivot = new Vector2(0f, 1f);
      rect.anchoredPosition = pos;
      rect.sizeDelta = size;
      var image = go.GetComponent<Image>();
      image.color = new Color(0.12f, 0.16f, 0.24f, 0.95f);
      var button = go.GetComponent<Button>();
      button.targetGraphic = image;
      button.onClick.AddListener(onClick);
      var labelText = CreateText(go.transform, "Label", label, 26, FontStyles.Normal, TextAlignmentOptions.MidlineLeft, new Vector2(14f, 0f), new Vector2(size.x - 24f, size.y));
      labelText.enableAutoSizing = true;
      labelText.fontSizeMin = 14f;
      labelText.fontSizeMax = 26f;
      return button;
    }
  }
}
