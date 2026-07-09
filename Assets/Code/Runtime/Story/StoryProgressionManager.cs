using System;
using System.Collections;
using HeavenOrHell.Audio;
using HeavenOrHell.Cauldron;
using HeavenOrHell.UI;
using UnityEngine;

namespace HeavenOrHell.Story
{
  /// <summary>
  /// Steuert den Story-Ablauf im Limbo-Office:
  /// Pro Beat sammelt der Spieler 4 Items (Heaven- oder Hell-Energie) im Kessel,
  /// damit wird der zugehörige Charakter (Engel/Dämon) beschworen.
  /// Reihenfolge laut Story: Angel 1 -> Demon 1 -> Angel 2 -> Demon 2 -> Finale.
  /// Beim Beat-Wechsel wird die gleiche Map nur umgethemed (Sky, Licht, Fog) + Fade.
  /// </summary>
  public class StoryProgressionManager : MonoBehaviour
  {
    [Serializable]
    public class StoryBeat
    {
      public string beatId = "angel1";
      public string characterName = "Angel 1";
      [Tooltip("Bestimmt Item-Pool und Kessel-Validierung: 'heaven' oder 'hell'.")]
      public string chapterId = "heaven";
      public ChapterTheme theme;
    }

    [SerializeField] private StoryBeat[] beats;
    [SerializeField] private CauldronController cauldron;
    [SerializeField] private MarkedItemSpawner itemSpawner;
    [SerializeField] private ChapterThemeApplier themeApplier;
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private ChapterMusicController musicController;
    [SerializeField] private StoryHudController hudController;
    [SerializeField] private WitnessSummonController witnessPresenter;

    private int currentBeatIndex = -1;
    private bool transitionRunning;

    /// <summary>Neuer Beat beginnt — HUD zeigt Zieltext, Items spawnen.</summary>
    public event Action<StoryBeat> OnBeatStarted;

    /// <summary>Wird gefeuert, wenn genug Energie gesammelt wurde: Charakter erscheint. Hook für das Dialogsystem.</summary>
    public event Action<StoryBeat> OnCharacterSummoned;

    /// <summary>Alle Beats abgeschlossen: finale Entscheidung (Heaven/Hell/Timeout).</summary>
    public event Action OnStoryComplete;

    public StoryBeat CurrentBeat =>
      currentBeatIndex >= 0 && currentBeatIndex < beats.Length ? beats[currentBeatIndex] : null;

    private void OnEnable()
    {
      if (cauldron != null)
        cauldron.OnChapterComplete += HandleCauldronComplete;
    }

    private void OnDisable()
    {
      if (cauldron != null)
        cauldron.OnChapterComplete -= HandleCauldronComplete;
    }

    private void Start()
    {
      StartCoroutine(BeginFirstBeat());
    }

    private IEnumerator BeginFirstBeat()
    {
      yield return null;

      if (hudController != null)
        yield return hudController.PlayIntroSequence();

      EnterBeat(0);
    }

    private void HandleCauldronComplete()
    {
      if (transitionRunning)
        return;

      StartCoroutine(CauldronCompleteSequence());
    }

    private IEnumerator CauldronCompleteSequence()
    {
      transitionRunning = true;
      var beat = CurrentBeat;

      if (beat != null)
      {
        if (witnessPresenter != null)
          yield return witnessPresenter.PresentWitness(beat);

        Debug.Log($"StoryProgression: Genug Energie gesammelt — {beat.characterName} wurde befragt.");
        OnCharacterSummoned?.Invoke(beat);
      }

      yield return TransitionToBeat(currentBeatIndex + 1);
      transitionRunning = false;
    }

    private IEnumerator TransitionToBeat(int nextIndex)
    {
      if (screenFader != null)
      {
        yield return screenFader.FadeOut();
        yield return new WaitForSeconds(0.2f);
      }

      if (nextIndex >= beats.Length)
      {
        Debug.Log("StoryProgression: Alle Beats abgeschlossen — Finale.");
        itemSpawner?.ClearSpawnedItems();
        musicController?.PlayFinale();
        OnStoryComplete?.Invoke();
      }
      else
      {
        EnterBeat(nextIndex);
      }

      if (screenFader != null)
        yield return screenFader.FadeIn();
    }

    private void EnterBeat(int index)
    {
      currentBeatIndex = index;
      var beat = beats[index];

      Debug.Log($"StoryProgression: Beat '{beat.beatId}' beginnt (Kapitel {beat.chapterId}).");

      if (themeApplier != null)
        themeApplier.Apply(beat.theme);

      musicController?.PlayForChapter(beat.chapterId);

      if (itemSpawner != null)
        itemSpawner.SpawnForChapter(beat.chapterId);

      if (cauldron != null)
        cauldron.SetActiveChapter(beat.chapterId);

      OnBeatStarted?.Invoke(beat);
    }
  }
}
