using System.Collections;
using UnityEngine;

namespace HeavenOrHell.Story
{
  /// <summary>
  /// VR-tauglicher Screen-Fade: ein schwarzes Quad direkt vor der Kamera,
  /// dessen Alpha animiert wird. Screen-Space-Canvas funktioniert im HMD nicht,
  /// deshalb ein Welt-Quad als Kamera-Child.
  /// </summary>
  public class ScreenFader : MonoBehaviour
  {
    [SerializeField] private float fadeDuration = 0.6f;

    private MeshRenderer quadRenderer;
    private Material fadeMaterial;
    private Coroutine activeFade;

    public float FadeDuration => fadeDuration;

    private void Awake()
    {
      CreateFadeQuad();
      SetAlpha(0f);
    }

    public Coroutine FadeOut() => StartFade(1f);
    public Coroutine FadeIn() => StartFade(0f);

    private Coroutine StartFade(float targetAlpha)
    {
      if (activeFade != null)
        StopCoroutine(activeFade);

      activeFade = StartCoroutine(FadeRoutine(targetAlpha));
      return activeFade;
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
      var startAlpha = fadeMaterial.color.a;
      var elapsed = 0f;

      quadRenderer.enabled = true;

      while (elapsed < fadeDuration)
      {
        elapsed += Time.deltaTime;
        SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration));
        yield return null;
      }

      SetAlpha(targetAlpha);
      quadRenderer.enabled = targetAlpha > 0.001f;
      activeFade = null;
    }

    private void SetAlpha(float alpha)
    {
      var c = fadeMaterial.color;
      c.a = alpha;
      fadeMaterial.color = c;
    }

    private void CreateFadeQuad()
    {
      var cam = Camera.main;
      var parent = cam != null ? cam.transform : transform;

      var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
      quad.name = "ScreenFadeQuad";
      Destroy(quad.GetComponent<Collider>());
      quad.transform.SetParent(parent, false);
      quad.transform.localPosition = new Vector3(0f, 0f, 0.35f);
      quad.transform.localRotation = Quaternion.identity;
      // Groß genug, um das gesamte Sichtfeld des HMD abzudecken.
      quad.transform.localScale = new Vector3(2f, 2f, 1f);

      // Sprites/Default unterstützt Alpha und rendert in URP; Overlay-Queue,
      // damit das Quad über allem liegt.
      fadeMaterial = new Material(Shader.Find("Sprites/Default"))
      {
        color = new Color(0f, 0f, 0f, 0f),
        renderQueue = 4000
      };

      quadRenderer = quad.GetComponent<MeshRenderer>();
      quadRenderer.material = fadeMaterial;
      quadRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      quadRenderer.receiveShadows = false;
      quadRenderer.enabled = false;
    }
  }
}
