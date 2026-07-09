using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace HeavenOrHell.Interaction
{
  /// <summary>
  /// VR door that toggles open/closed when selected with a ray or direct interactor.
  /// Rotates around a vertical world-space hinge (Y axis), not the imported mesh tilt.
  /// </summary>
  public class OpenableDoor : MonoBehaviour
  {
    [SerializeField] private float openAngle = -90f;
    [SerializeField] private float openDuration = 0.75f;
    [SerializeField] private Transform handle;

    private Transform hingePivot;
    private Quaternion closedPivotRotation;
    private Quaternion openPivotRotation;
    private bool isOpen;
    private Coroutine animationRoutine;
    private XRSimpleInteractable interactable;

    public void Configure(float angle, Transform doorHandle = null)
    {
      openAngle = angle;
      if (doorHandle != null)
        handle = doorHandle;
    }

    private void Awake()
    {
      if (handle == null)
        handle = FindHandleTransform();

      CreateHingePivot();
      EnsureInteractable();
    }

    private Transform FindHandleTransform()
    {
      foreach (Transform child in transform)
      {
        if (child.name.Contains("Door_Handle"))
          return child;
      }

      return null;
    }

    private void CreateHingePivot()
    {
      var renderers = GetComponentsInChildren<Renderer>();
      if (renderers.Length == 0)
        return;

      var bounds = renderers[0].bounds;
      for (var i = 1; i < renderers.Length; i++)
        bounds.Encapsulate(renderers[i].bounds);

      var hingePosition = bounds.center;
      if (handle != null)
      {
        var toHandle = handle.position - bounds.center;
        toHandle.y = 0f;
        if (toHandle.sqrMagnitude > 0.0001f)
        {
          var hingeDir = -toHandle.normalized;
          var reach = new Vector3(
            Mathf.Abs(hingeDir.x) * bounds.extents.x,
            0f,
            Mathf.Abs(hingeDir.z) * bounds.extents.z);
          hingePosition = bounds.center + hingeDir * reach.magnitude;
        }
      }

      var pivotGo = new GameObject($"{name}_Hinge");
      hingePivot = pivotGo.transform;

      // Keep a vertical hinge even when the imported door mesh is tilted (Blender X=270).
      var verticalHingeRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
      hingePivot.SetPositionAndRotation(hingePosition, verticalHingeRotation);

      var previousParent = transform.parent;
      hingePivot.SetParent(previousParent, true);
      transform.SetParent(hingePivot, true);

      closedPivotRotation = hingePivot.rotation;
      openPivotRotation = closedPivotRotation * Quaternion.AngleAxis(openAngle, Vector3.up);
    }

    private void EnsureInteractable()
    {
      var target = handle != null ? handle.gameObject : gameObject;

      interactable = target.GetComponent<XRSimpleInteractable>();
      if (interactable == null)
        interactable = target.AddComponent<XRSimpleInteractable>();

      EnsureInteractionCollider(target);

      interactable.selectEntered.AddListener(OnSelected);
    }

    private static void EnsureInteractionCollider(GameObject target)
    {
      if (target.GetComponent<Collider>() != null)
        return;

      var renderer = target.GetComponentInChildren<Renderer>();
      if (renderer == null)
      {
        var box = target.AddComponent<BoxCollider>();
        box.size = new Vector3(0.12f, 0.2f, 0.12f);
        return;
      }

      var bounds = renderer.bounds;
      var boxCollider = target.AddComponent<BoxCollider>();
      boxCollider.center = target.transform.InverseTransformPoint(bounds.center);
      var localSize = target.transform.InverseTransformVector(bounds.size);
      boxCollider.size = new Vector3(
        Mathf.Abs(localSize.x),
        Mathf.Abs(localSize.y),
        Mathf.Abs(localSize.z));
    }

    private void OnSelected(SelectEnterEventArgs _)
    {
      Toggle();
    }

    public void Toggle()
    {
      if (hingePivot == null)
        return;

      isOpen = !isOpen;
      if (animationRoutine != null)
        StopCoroutine(animationRoutine);

      animationRoutine = StartCoroutine(AnimateTo(isOpen ? openPivotRotation : closedPivotRotation));
    }

    private IEnumerator AnimateTo(Quaternion targetRotation)
    {
      var startRotation = hingePivot.rotation;
      var elapsed = 0f;

      while (elapsed < openDuration)
      {
        elapsed += Time.deltaTime;
        hingePivot.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / openDuration);
        yield return null;
      }

      hingePivot.rotation = targetRotation;
      animationRoutine = null;
    }

    private void OnDestroy()
    {
      if (interactable != null)
        interactable.selectEntered.RemoveListener(OnSelected);
    }
  }
}