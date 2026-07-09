using UnityEngine;

namespace HeavenOrHell.Cauldron
{
    /// <summary>
    /// Marker for where marked cauldron items can spawn in the shared blockout map.
    /// </summary>
    public class ItemSpawnPoint : MonoBehaviour
    {
        [SerializeField] private string zoneId = "office";
        [SerializeField] private float placementHeightOffset = 0.02f;

        public string ZoneId => zoneId;
        public Vector3 SpawnPosition => transform.position + Vector3.up * placementHeightOffset;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.8f);
            Gizmos.DrawWireSphere(SpawnPosition, 0.12f);
            Gizmos.DrawLine(transform.position, SpawnPosition);
        }
#endif
    }
}
