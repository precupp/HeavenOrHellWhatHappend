using UnityEngine;

namespace HeavenOrHell.Story
{
    /// <summary>
    /// Runtime data for chapter atmosphere. Applied by ChapterThemeApplier in Step 5.
    /// </summary>
    [CreateAssetMenu(fileName = "ChapterTheme", menuName = "HeavenOrHell/Chapter Theme")]
    public class ChapterTheme : ScriptableObject
    {
        public string chapterId;
        public Material skyboxMaterial;
        public Color ambientColor = new(0.85f, 0.88f, 0.95f);
        public bool useFog;
        public Color fogColor = Color.white;
        public float fogDensity = 0.01f;
        public Color mainLightColor = Color.white;
        public float mainLightIntensity = 1.2f;
    }
}
