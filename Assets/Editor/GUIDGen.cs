using UnityEditor;

public class GUIDGen : EditorWindow
{
    [MenuItem("My Tools/Generate GUID")]
    static void Init()
    {
        // Genera un GUID y lo copia al portapapeles
        System.Guid guid = System.Guid.NewGuid();
        EditorGUIUtility.systemCopyBuffer = guid.ToString();
    }

}
