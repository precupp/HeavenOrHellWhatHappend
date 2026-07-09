using UnityEngine;

namespace HeavenOrHell.Cauldron
{
    /// <summary>
    /// Sanftes Pulsieren eines Lichts, damit Marked Items in der Welt auffallen.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class MarkedItemPulse : MonoBehaviour
    {
        [SerializeField] private float minIntensity = 1.5f;
        [SerializeField] private float maxIntensity = 3.5f;
        [SerializeField] private float pulseSpeed = 2f;

        private Light pulseLight;
        private float phaseOffset;

        private void Awake()
        {
            pulseLight = GetComponent<Light>();
            // Zufällige Phase, damit nicht alle Items synchron pulsieren.
            phaseOffset = Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed + phaseOffset) + 1f) * 0.5f;
            pulseLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }
    }
}
