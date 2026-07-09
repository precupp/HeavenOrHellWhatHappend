using UnityEngine;

namespace HeavenOrHell.Interaction
{
  /// <summary>
  /// Wires up blockout toilet doors for VR interaction at runtime.
  /// </summary>
  public class OpenableDoorSetup : MonoBehaviour
  {
    [SerializeField] private Transform blockoutRoot;

    private static readonly (string doorName, float openAngle)[] DoorConfigs =
    {
      ("Toilet_Door_I1", -90f),
      ("Toilet_Door_I2", -90f),
      ("Toilet_Door_I3", 90f),
      ("Toilet_Heaven_Door", -90f),
      ("Toilet_Heaven_Door.001", 90f),
      ("Toilet_Heaven_Door.002", -90f),
    };

    private void Awake()
    {
      if (blockoutRoot == null)
      {
        var blockout = GameObject.Find("Blockout_combined_without_ceiling");
        if (blockout != null)
          blockoutRoot = blockout.transform;
      }

      if (blockoutRoot == null)
      {
        Debug.LogWarning("OpenableDoorSetup: Blockout root not found.");
        return;
      }

      foreach (var (doorName, openAngle) in DoorConfigs)
        SetupDoor(FindDeepChild(blockoutRoot, doorName), openAngle);
    }

    private static void SetupDoor(Transform doorTransform, float openAngle)
    {
      if (doorTransform == null)
        return;

      if (doorTransform.GetComponent<OpenableDoor>() != null)
        return;

      Transform handle = null;
      foreach (Transform child in doorTransform)
      {
        if (child.name.Contains("Door_Handle"))
        {
          handle = child;
          break;
        }
      }

      var door = doorTransform.gameObject.AddComponent<OpenableDoor>();
      door.Configure(openAngle, handle);
    }

    private static Transform FindDeepChild(Transform root, string childName)
    {
      if (root.name == childName)
        return root;

      for (var i = 0; i < root.childCount; i++)
      {
        var found = FindDeepChild(root.GetChild(i), childName);
        if (found != null)
          return found;
      }

      return null;
    }
  }
}
