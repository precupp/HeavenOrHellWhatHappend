using UnityEngine;

namespace HeavenOrHell.VFX
{
  /// <summary>
  /// Messy colored dust cloud used when a witness is summoned.
  /// Blue for Heaven, red-orange for Hell.
  /// </summary>
  public class SummonDustCloud : MonoBehaviour
  {
    private static Texture2D softCircleTexture;

    private ParticleSystem particleSystem;
    private float lifetime;

    public static SummonDustCloud Spawn(Vector3 worldPosition, string chapterId, float duration = 0f)
    {
      var go = new GameObject($"SummonDust_{chapterId}");
      go.transform.position = worldPosition;
      var cloud = go.AddComponent<SummonDustCloud>();
      cloud.Build(chapterId);
      if (duration > 0f)
        cloud.lifetime = duration;
      return cloud;
    }

    public void Build(string chapterId)
    {
      particleSystem = gameObject.AddComponent<ParticleSystem>();
      var isHell = chapterId == "hell";
      var primary = isHell ? new Color(1f, 0.28f, 0.08f, 0.85f) : new Color(0.45f, 0.72f, 1f, 0.85f);
      var secondary = isHell ? new Color(0.75f, 0.1f, 0.05f, 0.55f) : new Color(0.85f, 0.92f, 1f, 0.55f);

      var main = particleSystem.main;
      main.loop = true;
      main.startLifetime = new ParticleSystem.MinMaxCurve(1.4f, 2.6f);
      main.startSpeed = new ParticleSystem.MinMaxCurve(0.15f, 0.9f);
      main.startSize = new ParticleSystem.MinMaxCurve(0.18f, 0.55f);
      main.startColor = new ParticleSystem.MinMaxGradient(primary, secondary);
      main.maxParticles = 260;
      main.simulationSpace = ParticleSystemSimulationSpace.World;
      main.gravityModifier = isHell ? -0.02f : -0.04f;

      var emission = particleSystem.emission;
      emission.rateOverTime = 42f;

      var shape = particleSystem.shape;
      shape.shapeType = ParticleSystemShapeType.Sphere;
      shape.radius = 0.35f;

      var velocity = particleSystem.velocityOverLifetime;
      velocity.enabled = true;
      velocity.orbitalX = 0.4f;
      velocity.orbitalY = 0.8f;
      velocity.orbitalZ = 0.25f;

      var noise = particleSystem.noise;
      noise.enabled = true;
      noise.strength = 0.55f;
      noise.frequency = 0.65f;
      noise.scrollSpeed = 0.35f;

      var colorOverLifetime = particleSystem.colorOverLifetime;
      colorOverLifetime.enabled = true;
      var gradient = new Gradient();
      gradient.SetKeys(
        new[]
        {
          new GradientColorKey(primary, 0f),
          new GradientColorKey(secondary, 0.55f),
          new GradientColorKey(Color.clear, 1f)
        },
        new[]
        {
          new GradientAlphaKey(0.9f, 0f),
          new GradientAlphaKey(0.45f, 0.7f),
          new GradientAlphaKey(0f, 1f)
        });
      colorOverLifetime.color = gradient;

      var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
      renderer.renderMode = ParticleSystemRenderMode.Billboard;
      renderer.material = CreateCloudMaterial();
      renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
      renderer.receiveShadows = false;
    }

    /// <summary>
    /// URP-safe transparent particle material. The built-in "Particles/Standard Unlit"
    /// shader renders as opaque squares under URP, so we use Sprites/Default with a
    /// procedurally generated soft circle texture instead.
    /// </summary>
    private static Material CreateCloudMaterial()
    {
      var material = new Material(Shader.Find("Sprites/Default"))
      {
        mainTexture = GetSoftCircleTexture(),
        renderQueue = 3000
      };
      return material;
    }

    private static Texture2D GetSoftCircleTexture()
    {
      if (softCircleTexture != null)
        return softCircleTexture;

      const int size = 64;
      softCircleTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
      var center = (size - 1) * 0.5f;

      for (var y = 0; y < size; y++)
      {
        for (var x = 0; x < size; x++)
        {
          var dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) / center;
          var alpha = Mathf.Clamp01(1f - dist);
          alpha = alpha * alpha; // softer falloff towards the edge
          softCircleTexture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
        }
      }

      softCircleTexture.Apply();
      return softCircleTexture;
    }

    private void Update()
    {
      if (lifetime <= 0f)
        return;

      lifetime -= Time.deltaTime;
      if (lifetime <= 0f)
        Destroy(gameObject);
    }

    public void FadeOutAndDestroy(float delay = 0.4f)
    {
      if (particleSystem == null)
      {
        Destroy(gameObject);
        return;
      }

      var emission = particleSystem.emission;
      emission.rateOverTime = 0f;
      Destroy(gameObject, delay + 2.5f);
    }
  }
}
