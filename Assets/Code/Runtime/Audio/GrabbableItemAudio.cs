using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HeavenOrHell.Audio
{
  /// <summary>
  /// Sanfte Pickup- und Boden-Kollision-Sounds für grabbare Items.
  /// </summary>
  [RequireComponent(typeof(Rigidbody))]
  public class GrabbableItemAudio : MonoBehaviour
  {
    [SerializeField] private float pickupVolume = 0.22f;
    [SerializeField] private float impactVolume = 0.18f;
    [SerializeField] private float minImpactSpeed = 0.8f;
    [SerializeField] private float impactCooldown = 0.25f;

    private AudioSource audioSource;
    private XRGrabInteractable grabInteractable;
    private float lastImpactTime = -1f;

    private void Awake()
    {
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.playOnAwake = false;
      audioSource.spatialBlend = 1f;
      audioSource.minDistance = 0.4f;
      audioSource.maxDistance = 8f;
      audioSource.rolloffMode = AudioRolloffMode.Linear;

      grabInteractable = GetComponent<XRGrabInteractable>();
      if (grabInteractable != null)
        grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDestroy()
    {
      if (grabInteractable != null)
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs _)
    {
      audioSource.PlayOneShot(ProceduralSfxLibrary.Pickup, pickupVolume);
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (Time.time - lastImpactTime < impactCooldown)
        return;

      if (grabInteractable != null && grabInteractable.isSelected)
        return;

      var speed = collision.relativeVelocity.magnitude;
      if (speed < minImpactSpeed)
        return;

      lastImpactTime = Time.time;
      var scaledVolume = Mathf.Clamp(speed / 4f, 0.35f, 1f) * impactVolume;
      audioSource.PlayOneShot(ProceduralSfxLibrary.FloorImpact, scaledVolume);
    }
  }
}
