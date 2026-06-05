using UnityEngine;

public class EditorReset : MonoBehaviour
{
    private static bool hasReset = false;

    void Awake()
    {
#if UNITY_EDITOR
        if (!hasReset)
        {
            PlayerPrefs.DeleteAll();
            hasReset = true;
        }
#endif
    }
}