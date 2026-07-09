using UnityEngine;

namespace HeavenOrHell.UI
{
  /// <summary>
  /// Shared camera-local layout for VR HUD canvases.
  /// </summary>
  public static class VrHudLayout
  {
    public const float CanvasScale = 0.00168f;
    public const float DistanceZ = 1.45f;

    public const float MainVerticalOffset = -0.1f;
    public const float MainPanelDownOffset = -60f;

    public const float MainPanelWidth = 1320f;
    public const float NarrativePanelHeight = 290f;
    public const float DialoguePanelHeight = 390f;
    public const float FinalePanelHeight = 360f;

    public const float TaskPanelBackgroundAlpha = 0.2f;

    public static Vector3 MainHudPosition(float extraYOffset = 0f) =>
      new Vector3(0f, MainVerticalOffset + extraYOffset, DistanceZ);

    public static Vector3 CenteredPosition(float yOffset = 0f) =>
      MainHudPosition(yOffset);
  }
}