using System;
using HeavenOrHell.Story;
using TMPro;
using UnityEngine;

namespace HeavenOrHell.Cauldron
{
  public class CauldronController : MonoBehaviour
  {
    [SerializeField] private MarkedItemSpawner itemSpawner;
    [SerializeField] private ChapterItemPool heavenPool;
    [SerializeField] private ChapterItemPool hellPool;
    [SerializeField] private Transform counterAnchor;
    [SerializeField] private float rejectImpulse = 4f;

    private int currentCount;
    private int requiredCount = 4;
    private TextMeshPro counterText;
    private string activeChapterId;

    public event Action OnChapterComplete;
    public event Action<int, int> OnProgressChanged;

    public int CurrentCount => currentCount;
    public int RequiredCount => requiredCount;

    private void Awake()
    {
      EnsureTriggerCollider();
      EnsureCounterUi();
      ResetCounter();
    }

    private void Start()
    {
      if (itemSpawner != null)
        activeChapterId = itemSpawner.ActiveChapterId;
      UpdateRequiredCount();
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!other.TryGetComponent<CauldronMarkedItem>(out var markedItem))
        return;

      if (string.IsNullOrEmpty(activeChapterId))
        activeChapterId = itemSpawner != null ? itemSpawner.ActiveChapterId : markedItem.ChapterId;

      if (markedItem.ChapterId != activeChapterId)
      {
        RejectItem(other, markedItem);
        return;
      }

      AcceptItem(markedItem);
    }

    public void SetActiveChapter(string chapterId)
    {
      activeChapterId = chapterId;
      ResetCounter();
      UpdateRequiredCount();
    }

    private void AcceptItem(CauldronMarkedItem markedItem)
    {
      currentCount++;
      UpdateCounterUi();
      OnProgressChanged?.Invoke(currentCount, requiredCount);
      Destroy(markedItem.gameObject);

      if (currentCount < requiredCount)
        return;

      OnChapterComplete?.Invoke();
      ResetCounter();
    }

    private void RejectItem(Collider other, CauldronMarkedItem markedItem)
    {
      if (other.attachedRigidbody != null)
        other.attachedRigidbody.AddForce((other.transform.position - transform.position).normalized * rejectImpulse, ForceMode.Impulse);

      markedItem.SetMarkingVisible(true);
    }

    private void ResetCounter()
    {
      currentCount = 0;
      UpdateCounterUi();
      OnProgressChanged?.Invoke(currentCount, requiredCount);
    }

    private void UpdateRequiredCount()
    {
      var pool = GetPool(activeChapterId);
      requiredCount = pool != null ? pool.itemsPerRound : 4;
      UpdateCounterUi();
    }

    private ChapterItemPool GetPool(string chapterId)
    {
      if (heavenPool != null && heavenPool.chapterId == chapterId)
        return heavenPool;
      if (hellPool != null && hellPool.chapterId == chapterId)
        return hellPool;
      return null;
    }

    private void EnsureTriggerCollider()
    {
      var sphere = GetComponent<SphereCollider>();
      if (sphere == null)
        sphere = gameObject.AddComponent<SphereCollider>();

      sphere.isTrigger = true;

      // Blockout meshes are imported with scale 100 — express the desired world-space
      // trigger size in local units so the collider hugs the cauldron opening.
      var maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
      if (maxScale <= 0f)
        maxScale = 1f;

      const float worldRadius = 0.7f;
      sphere.radius = worldRadius / maxScale;

      var renderer = GetComponent<Renderer>();
      if (renderer != null)
      {
        var worldCenter = renderer.bounds.center + Vector3.up * (renderer.bounds.extents.y * 0.5f);
        sphere.center = transform.InverseTransformPoint(worldCenter);
      }
    }

    private void EnsureCounterUi()
    {
      if (counterText != null)
        return;

      var anchor = counterAnchor != null ? counterAnchor : transform;
      var canvasGo = new GameObject("CauldronCounterCanvas");
      // Set world-space transform explicitly: the blockout cauldron has lossy scale 100,
      // so local units on a child would be scaled up by 100.
      canvasGo.transform.SetParent(anchor, true);
      var renderer = GetComponent<Renderer>();
      var topWorld = renderer != null
        ? new Vector3(renderer.bounds.center.x, renderer.bounds.max.y, renderer.bounds.center.z)
        : transform.position;
      canvasGo.transform.position = topWorld + Vector3.up * 0.5f;
      canvasGo.transform.rotation = Quaternion.identity;
      var parentScale = anchor.lossyScale;
      canvasGo.transform.localScale = new Vector3(
        0.01f / Mathf.Max(parentScale.x, 0.0001f),
        0.01f / Mathf.Max(parentScale.y, 0.0001f),
        0.01f / Mathf.Max(parentScale.z, 0.0001f));

      var canvas = canvasGo.AddComponent<Canvas>();
      canvas.renderMode = RenderMode.WorldSpace;

      var textGo = new GameObject("CounterText");
      textGo.transform.SetParent(canvasGo.transform, false);
      counterText = textGo.AddComponent<TextMeshPro>();
      counterText.alignment = TextAlignmentOptions.Center;
      counterText.fontSize = 48;
      counterText.rectTransform.sizeDelta = new Vector2(400f, 120f);
      counterText.color = Color.white;
    }

    private void UpdateCounterUi()
    {
      if (counterText == null)
        return;

      counterText.text = $"{currentCount}/{requiredCount}";
    }

    private void LateUpdate()
    {
      if (counterText == null)
        return;

      var cam = Camera.main;
      if (cam == null)
        return;

      var canvasTransform = counterText.transform.parent;
      canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - cam.transform.position);
    }
  }
}
