using UnityEngine;

namespace HeavenOrHell.Story
{
    [CreateAssetMenu(fileName = "ChapterItemPool", menuName = "HeavenOrHell/Chapter Item Pool")]
    public class ChapterItemPool : ScriptableObject
    {
        public string chapterId;
        public GameObject[] itemPrefabs;
        public int itemsPerRound = 4;
    }
}
