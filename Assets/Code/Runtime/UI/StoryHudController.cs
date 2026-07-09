using System.Collections;
using HeavenOrHell.Cauldron;
using HeavenOrHell.Story;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HeavenOrHell.UI
{
  /// <summary>
  /// VR-HUD: Todo links unten, Story-Narration in der Mitte.
  /// </summary>
  public class StoryHudController : MonoBehaviour
  {
    [SerializeField] private StoryProgressionManager storyManager;
    [SerializeField] private CauldronController cauldron;
    [SerializeField] private float introDuration = 14f;
    [SerializeField] private float beatObjectiveDuration = 7f;

    private TextMeshProUGUI taskLabel;
    private TextMeshProUGUI taskText;
    private TextMeshProUGUI narrativeText;
    private CanvasGroup narrativeGroup;
    private CanvasGroup taskGroup;
    private Coroutine narrativeRoutine;

    public IEnumerator PlayIntroSequence()
    {
      BuildHudIfNeeded();
      SetTaskVisible(false);
      yield return ShowNarrative(StoryNarrativeContent.Intro, introDuration);
      SetTaskVisible(true);
    }

    private void OnEnable()
    {
      if (storyManager != null)
      {
        storyManager.OnBeatStarted += HandleBeatStarted;
        storyManager.OnStoryComplete += HandleStoryComplete;
      }

      if (cauldron != null)
        cauldron.OnProgressChanged += HandleProgressChanged;
    }

    private void OnDisable()
    {
      if (storyManager != null)
      {
        storyManager.OnBeatStarted -= HandleBeatStarted;
        storyManager.OnStoryComplete -= HandleStoryComplete;
      }

      if (cauldron != null)
        cauldron.OnProgressChanged -= HandleProgressChanged;
    }

    private void HandleBeatStarted(StoryProgressionManager.StoryBeat beat)
    {
      BuildHudIfNeeded();
      UpdateTask(beat.chapterId, cauldron != null ? cauldron.CurrentCount : 0, cauldron != null ? cauldron.RequiredCount : 4);
      ShowNarrativeTemporary(StoryNarrativeContent.GetBeatObjective(beat.beatId, beat.chapterId), beatObjectiveDuration);
    }

    private void HandleStoryComplete()
    {
      UpdateTaskText("All witnesses summoned.\nMake your final choice.");
    }

    public IEnumerator ShowWitnessMoment(string witnessName, string text, float duration)
    {
      BuildHudIfNeeded();
      narrativeText.text = $"<color=#B8D4FF>{witnessName}</color>\n\n{text}";
      yield return ShowNarrative(narrativeText.text, duration);
    }

    public IEnumerator ShowEnding(string endingText)
    {
      BuildHudIfNeeded();
      SetTaskVisible(false);
      narrativeText.text = endingText;
      yield return ShowNarrative(endingText, 0f);
    }

    private void HandleProgressChanged(int current, int required)
    {
      var chapterId = storyManager != null && storyManager.CurrentBeat != null
        ? storyManager.CurrentBeat.chapterId
        : "heaven";
      UpdateTask(chapterId, current, required);
    }

    private void UpdateTask(string chapterId, int current, int required)
    {
      if (taskText == null)
        return;

      var energy = chapterId == "hell" ? "Hell" : "Heaven";
      taskText.text = $"Collect {energy} energy\nThrow into cauldron: {current}/{required}";
    }

    private void UpdateTaskText(string text)
    {
      if (taskText != null)
        taskText.text = text;
    }

    private void ShowNarrativeTemporary(string text, float duration)
    {
      if (narrativeRoutine != null)
        StopCoroutine(narrativeRoutine);

      narrativeRoutine = StartCoroutine(ShowNarrative(text, duration));
    }

    private IEnumerator ShowNarrative(string text, float duration)
    {
      BuildHudIfNeeded();
      narrativeText.text = text;
      yield return FadeGroup(narrativeGroup, 1f, 0.35f);

      if (duration > 0f)
      {
        yield return new WaitForSeconds(duration);
        yield return FadeGroup(narrativeGroup, 0f, 0.5f);
      }

      narrativeRoutine = null;
    }

    private static IEnumerator FadeGroup(CanvasGroup group, float target, float duration)
    {
      if (group == null)
        yield break;

      var start = group.alpha;
      var elapsed = 0f;
      group.gameObject.SetActive(true);

      while (elapsed < duration)
      {
        elapsed += Time.deltaTime;
        group.alpha = Mathf.Lerp(start, target, elapsed / duration);
        yield return null;
      }

      group.alpha = target;
      if (target <= 0.01f)
        group.gameObject.SetActive(false);
    }

    private void SetTaskVisible(bool visible)
    {
      if (taskGroup != null)
      {
        taskGroup.alpha = visible ? 1f : 0f;
        taskGroup.gameObject.SetActive(visible);
      }
    }

    private void BuildHudIfNeeded()
    {
      if (taskText != null)
        return;

      var cameraTransform = Camera.main != null ? Camera.main.transform : transform;

      var canvasGo = new GameObject("StoryHudCanvas");
      canvasGo.transform.SetParent(cameraTransform, false);
      canvasGo.transform.localPosition = VrHudLayout.MainHudPosition(-0.02f);
      canvasGo.transform.localRotation = Quaternion.identity;
      canvasGo.transform.localScale = Vector3.one * VrHudLayout.CanvasScale;

      var canvas = canvasGo.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.WorldSpace;
      canvas.sortingOrder = 50;

      // Explicit canvas size so anchor-based layout has real space to work with.
      var canvasRect = canvasGo.GetComponent<RectTransform>();
      canvasRect.sizeDelta = new Vector2(VrHudLayout.MainPanelWidth + 40f, 640f);

      var scaler = canvasGo.AddComponent<CanvasScaler>();
      scaler.dynamicPixelsPerUnit = 100f;

      canvasGo.AddComponent<GraphicRaycaster>();

      var todoPanel = CreatePanel(canvasGo.transform, "TodoPanel", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(28f, 72f), new Vector2(250f, 96f), new Color(0.04f, 0.06f, 0.1f, VrHudLayout.TaskPanelBackgroundAlpha));
      taskGroup = todoPanel.AddComponent<CanvasGroup>();

      taskLabel = CreateText(todoPanel.transform, "TaskLabel", "CURRENT TASK", 16, FontStyles.Bold, TextAlignmentOptions.TopLeft, new Vector2(12f, -10f), new Vector2(226f, 24f));
      taskLabel.color = new Color(0.75f, 0.82f, 0.95f, 1f);

      taskText = CreateText(todoPanel.transform, "TaskText", "Collect Heaven energy\nThrow into cauldron: 0/4", 18, FontStyles.Normal, TextAlignmentOptions.TopLeft, new Vector2(12f, -34f), new Vector2(226f, 58f));
      taskText.color = Color.white;

      var narrativePanel = CreatePanel(canvasGo.transform, "NarrativePanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, VrHudLayout.MainPanelDownOffset), new Vector2(VrHudLayout.MainPanelWidth, VrHudLayout.NarrativePanelHeight), new Color(0.02f, 0.03f, 0.06f, 0.94f));
      narrativeGroup = narrativePanel.AddComponent<CanvasGroup>();
      narrativeGroup.alpha = 0f;
      narrativePanel.gameObject.SetActive(false);

      narrativeText = CreateCenteredText(narrativePanel.transform, "NarrativeText", "", 30, FontStyles.Normal);
      narrativeText.color = new Color(0.95f, 0.95f, 0.98f, 1f);
      narrativeText.lineSpacing = 4f;
    }

    private static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Color color)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(Image));
      go.transform.SetParent(parent, false);

      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = anchorMin;
      rect.anchorMax = anchorMax;
      // Bottom-left anchored panels use bottom-left pivot so anchoredPosition is a margin.
      rect.pivot = anchorMin == anchorMax && anchorMin == Vector2.zero
        ? Vector2.zero
        : new Vector2(0.5f, 0.5f);
      rect.anchoredPosition = anchoredPos;
      rect.sizeDelta = size;

      var image = go.GetComponent<Image>();
      image.color = color;
      image.raycastTarget = false;
      return go;
    }


    private static TextMeshProUGUI CreateCenteredText(Transform parent, string name, string content, float fontSize, FontStyles style)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
      go.transform.SetParent(parent, false);

      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = new Vector2(0.05f, 0.1f);
      rect.anchorMax = new Vector2(0.95f, 0.9f);
      rect.offsetMin = Vector2.zero;
      rect.offsetMax = Vector2.zero;

      var text = go.GetComponent<TextMeshProUGUI>();
      text.text = content;
      text.fontSize = fontSize;
      // Long narrative/ending strings shrink to fit the panel instead of overflowing the FOV.
      text.enableAutoSizing = true;
      text.fontSizeMin = 14f;
      text.fontSizeMax = fontSize;
      text.fontStyle = style;
      text.alignment = TextAlignmentOptions.Center;
      text.enableWordWrapping = true;
      text.raycastTarget = false;
      return text;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string content, float fontSize, FontStyles style, TextAlignmentOptions alignment, Vector2 anchoredPos, Vector2 size)
    {
      var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
      go.transform.SetParent(parent, false);

      var rect = go.GetComponent<RectTransform>();
      rect.anchorMin = new Vector2(0f, 1f);
      rect.anchorMax = new Vector2(0f, 1f);
      rect.pivot = new Vector2(0f, 1f);
      rect.anchoredPosition = anchoredPos;
      rect.sizeDelta = size;

      var text = go.GetComponent<TextMeshProUGUI>();
      text.text = content;
      text.fontSize = fontSize;
      text.fontStyle = style;
      text.alignment = alignment;
      text.enableWordWrapping = true;
      text.raycastTarget = false;
      return text;
    }
  }
}
