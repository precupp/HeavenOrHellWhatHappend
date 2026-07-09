using UnityEngine;

namespace HeavenOrHell.Audio
{
  /// <summary>
  /// Spielt Heaven-, Hell- und Finale-Theme als Loop-Hintergrundmusik.
  /// </summary>
  public class ChapterMusicController : MonoBehaviour
  {
    [SerializeField] private AudioClip heavenTheme;
    [SerializeField] private AudioClip hellTheme;
    [SerializeField] private AudioClip finalChoiceTheme;
    [SerializeField, Range(0f, 1f)] private float volume = 0.2f;

    private AudioSource audioSource;
    private string activeTrackId;

    private void Awake()
    {
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.playOnAwake = false;
      audioSource.loop = true;
      audioSource.spatialBlend = 0f;
      audioSource.volume = volume;
    }

    public void PlayForChapter(string chapterId)
    {
      if (string.IsNullOrEmpty(chapterId) || chapterId == activeTrackId)
        return;

      var clip = chapterId == "hell" ? hellTheme : heavenTheme;
      PlayClip(chapterId, clip);
    }

    public void PlayFinale()
    {
      PlayClip("finale", finalChoiceTheme);
    }

    public void StopMusic()
    {
      activeTrackId = null;
      audioSource.Stop();
    }

    private void PlayClip(string trackId, AudioClip clip)
    {
      if (clip == null)
        return;

      activeTrackId = trackId;
      audioSource.clip = clip;
      audioSource.volume = volume;
      audioSource.Play();
    }
  }
}
