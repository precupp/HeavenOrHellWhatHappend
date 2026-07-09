using UnityEngine;

namespace HeavenOrHell.Story
{
  /// <summary>
  /// Wendet ein ChapterTheme auf die gemeinsame Blockout-Map an:
  /// Skybox, Ambient, Fog und das Haupt-Directional-Light.
  /// Der Wechsel passiert instant und wird vom ScreenFader kaschiert.
  /// </summary>
  public class ChapterThemeApplier : MonoBehaviour
  {
    [SerializeField] private Light mainLight;

    public void Apply(ChapterTheme theme)
    {
      if (theme == null)
        return;

      if (theme.skyboxMaterial != null)
        RenderSettings.skybox = theme.skyboxMaterial;

      RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
      RenderSettings.ambientLight = theme.ambientColor;

      RenderSettings.fog = theme.useFog;
      if (theme.useFog)
      {
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = theme.fogColor;
        RenderSettings.fogDensity = theme.fogDensity;
      }

      if (mainLight != null)
      {
        mainLight.color = theme.mainLightColor;
        mainLight.intensity = theme.mainLightIntensity;
      }

      DynamicGI.UpdateEnvironment();
    }
  }
}
