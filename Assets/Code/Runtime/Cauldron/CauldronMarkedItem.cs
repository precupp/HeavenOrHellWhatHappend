using UnityEngine;

namespace HeavenOrHell.Cauldron
{
    /// <summary>
    /// Markiert ein Objekt als "gehört in den Kessel" für ein bestimmtes Story-Kapitel.
    /// Das visuelle Marking (Licht/Glow) liegt als Child-GameObject in <see cref="vfxRoot"/>
    /// und bleibt auch aktiv, während das Item getragen wird.
    /// </summary>
    public class CauldronMarkedItem : MonoBehaviour
    {
        [SerializeField] private string itemId;
        [SerializeField] private string chapterId;
        [SerializeField] private GameObject vfxRoot;

        public string ItemId => itemId;
        public string ChapterId => chapterId;

        public void SetMarkingVisible(bool visible)
        {
            if (vfxRoot != null)
                vfxRoot.SetActive(visible);
        }

        public void Initialize(string newItemId, string newChapterId)
        {
            itemId = newItemId;
            chapterId = newChapterId;
        }
    }
}
