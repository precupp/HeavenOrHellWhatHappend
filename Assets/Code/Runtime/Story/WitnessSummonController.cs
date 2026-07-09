using System.Collections;
using HeavenOrHell.UI;
using HeavenOrHell.VFX;
using UnityEngine;

namespace HeavenOrHell.Story
{
  /// <summary>
  /// Spawns colored dust cloud and shows short witness text — no dialogue.
  /// </summary>
  public class WitnessSummonController : MonoBehaviour
  {
    [SerializeField] private StoryHudController hudController;
    [SerializeField] private Transform summonAnchor;
    [SerializeField] private Vector3 summonOffset = new(1.8f, 1.35f, 0.4f);
    [SerializeField] private float witnessDisplayDuration = 8f;

    private SummonDustCloud activeCloud;

    public IEnumerator PresentWitness(StoryProgressionManager.StoryBeat beat)
    {
      var position = summonAnchor != null ? summonAnchor.position + summonOffset : transform.position;
      activeCloud = SummonDustCloud.Spawn(position, beat.chapterId);

      var cloudLine = beat.chapterId == "hell"
        ? "A messy crimson dust cloud swirls into being..."
        : "A soft blue-white dust cloud swirls into being...";
      var text = $"{StoryNarrativeContent.GetSummonText(beat.beatId)}\n\n{cloudLine}";

      if (hudController != null)
        yield return hudController.ShowWitnessMoment(beat.characterName, text, witnessDisplayDuration);

      if (activeCloud != null)
      {
        activeCloud.FadeOutAndDestroy();
        activeCloud = null;
      }
    }
  }
}
