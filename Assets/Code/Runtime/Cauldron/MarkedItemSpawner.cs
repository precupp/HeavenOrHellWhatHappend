using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HeavenOrHell.Audio;
using HeavenOrHell.Story;
using UnityEngine;

namespace HeavenOrHell.Cauldron
{
  public class MarkedItemSpawner : MonoBehaviour
  {
    [SerializeField] private ChapterItemPool heavenPool;
    [SerializeField] private ChapterItemPool hellPool;
    [SerializeField] private Transform heavenItemRoot;
    [SerializeField] private Transform hellItemRoot;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private float minDistanceFromPlayer = 2f;
    [SerializeField] private float minDistanceBetweenSpawns = 3f;
    [SerializeField] private bool spawnHeavenOnStart = true;

    private readonly List<GameObject> spawnedItems = new();
    private string activeChapterId;

    public string ActiveChapterId => activeChapterId;

    private void Start()
    {
      if (spawnHeavenOnStart && heavenPool != null)
        SpawnForChapter(heavenPool.chapterId);
    }

    [ContextMenu("Spawn Heaven")]
    public void SpawnHeaven() => SpawnForChapter(heavenPool != null ? heavenPool.chapterId : "heaven");

    [ContextMenu("Spawn Hell")]
    public void SpawnHell() => SpawnForChapter(hellPool != null ? hellPool.chapterId : "hell");

    public void SpawnForChapter(string chapterId)
    {
      var pool = GetPool(chapterId);
      if (pool == null)
      {
        Debug.LogWarning($"MarkedItemSpawner: No pool configured for chapter '{chapterId}'.");
        return;
      }

      ClearSpawnedItems();
      activeChapterId = pool.chapterId;

      if (heavenItemRoot != null)
        heavenItemRoot.gameObject.SetActive(pool.chapterId == heavenPool?.chapterId);
      if (hellItemRoot != null)
        hellItemRoot.gameObject.SetActive(pool.chapterId == hellPool?.chapterId);

      var spawnPoints = SelectSpawnPoints(pool.itemsPerRound);
      var prefabs = SelectPrefabs(pool);

      for (var i = 0; i < spawnPoints.Count && i < prefabs.Count; i++)
      {
        var parent = GetItemRoot(pool.chapterId);
        var instance = Instantiate(prefabs[i], spawnPoints[i].SpawnPosition, spawnPoints[i].transform.rotation, parent);
        EnsureItemAudio(instance);
        spawnedItems.Add(instance);
        StartCoroutine(SettleSpawnedItem(instance));
      }
    }

    public void ClearSpawnedItems()
    {
      for (var i = spawnedItems.Count - 1; i >= 0; i--)
      {
        if (spawnedItems[i] != null)
          Destroy(spawnedItems[i]);
      }

      spawnedItems.Clear();
    }

    private ChapterItemPool GetPool(string chapterId)
    {
      if (heavenPool != null && heavenPool.chapterId == chapterId)
        return heavenPool;
      if (hellPool != null && hellPool.chapterId == chapterId)
        return hellPool;
      return null;
    }

    private Transform GetItemRoot(string chapterId)
    {
      if (hellPool != null && chapterId == hellPool.chapterId && hellItemRoot != null)
        return hellItemRoot;
      return heavenItemRoot;
    }

    private List<ItemSpawnPoint> SelectSpawnPoints(int count)
    {
      var allPoints = FindObjectsByType<ItemSpawnPoint>(FindObjectsSortMode.None).ToList();
      var playerPos = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
      var selected = new List<ItemSpawnPoint>();

      allPoints = allPoints
        .Where(p => Vector3.Distance(p.SpawnPosition, playerPos) >= minDistanceFromPlayer)
        .OrderBy(_ => Random.value)
        .ToList();

      foreach (var candidate in allPoints)
      {
        if (selected.Any(s => Vector3.Distance(s.SpawnPosition, candidate.SpawnPosition) < minDistanceBetweenSpawns))
          continue;

        selected.Add(candidate);
        if (selected.Count >= count)
          break;
      }

      if (selected.Count < count)
      {
        foreach (var candidate in allPoints)
        {
          if (selected.Contains(candidate))
            continue;
          selected.Add(candidate);
          if (selected.Count >= count)
            break;
        }
      }

      return selected;
    }

    private static List<GameObject> SelectPrefabs(ChapterItemPool pool)
    {
      var available = pool.itemPrefabs.Where(p => p != null).OrderBy(_ => Random.value).ToList();
      var result = new List<GameObject>();

      while (result.Count < pool.itemsPerRound)
      {
        if (available.Count == 0)
          break;
        result.Add(available[result.Count % available.Count]);
      }

      return result;
    }

    private static void EnsureItemAudio(GameObject instance)
    {
      if (instance.GetComponent<GrabbableItemAudio>() == null)
        instance.AddComponent<GrabbableItemAudio>();
    }

    private static IEnumerator SettleSpawnedItem(GameObject instance)
    {
      var rb = instance.GetComponent<Rigidbody>();
      if (rb == null)
        yield break;

      rb.isKinematic = true;
      rb.linearVelocity = Vector3.zero;
      yield return new WaitForFixedUpdate();
      rb.isKinematic = false;
      rb.angularVelocity = Vector3.zero;
    }
  }
}
