using UnityEngine;

namespace HeavenOrHell.Audio
{
  /// <summary>
  /// Erzeugt einmalig sehr leise, kurze SFX-Clips zur Laufzeit.
  /// </summary>
  public static class ProceduralSfxLibrary
  {
    private const int SampleRate = 44100;

    private static AudioClip pickupClip;
    private static AudioClip floorImpactClip;

    public static AudioClip Pickup => pickupClip ??= CreatePickupClip();
    public static AudioClip FloorImpact => floorImpactClip ??= CreateFloorImpactClip();

    private static AudioClip CreatePickupClip()
    {
      const float duration = 0.18f;
      var sampleCount = Mathf.CeilToInt(SampleRate * duration);
      var samples = new float[sampleCount];

      for (var i = 0; i < sampleCount; i++)
      {
        var t = i / (float)SampleRate;
        var envelope = Mathf.Exp(-t * 22f);
        var toneA = Mathf.Sin(2f * Mathf.PI * 784f * t);   // G5
        var toneB = Mathf.Sin(2f * Mathf.PI * 1175f * t);  // D6
        samples[i] = (toneA * 0.55f + toneB * 0.25f) * envelope * 0.12f;
      }

      var clip = AudioClip.Create("SFX_Pickup", sampleCount, 1, SampleRate, false);
      clip.SetData(samples, 0);
      return clip;
    }

    private static AudioClip CreateFloorImpactClip()
    {
      const float duration = 0.1f;
      var sampleCount = Mathf.CeilToInt(SampleRate * duration);
      var samples = new float[sampleCount];

      for (var i = 0; i < sampleCount; i++)
      {
        var t = i / (float)SampleRate;
        var envelope = Mathf.Exp(-t * 35f);
        var thud = Mathf.Sin(2f * Mathf.PI * 95f * t);
        var noise = (Random.value * 2f - 1f) * 0.15f;
        samples[i] = (thud * 0.7f + noise) * envelope * 0.18f;
      }

      var clip = AudioClip.Create("SFX_FloorImpact", sampleCount, 1, SampleRate, false);
      clip.SetData(samples, 0);
      return clip;
    }
  }
}
