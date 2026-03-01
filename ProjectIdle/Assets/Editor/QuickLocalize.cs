using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

public class QuickLocalize : EditorWindow
{
    [MenuItem("Tools/Akilli Bagla")]
    public static void AkilliBagla()
    {
        var texts = GameObject.FindObjectsOfType<TextMeshProUGUI>();
        // "Game" tablonuzun adını buraya tam yazın
        string tableName = "Game";

        foreach (var t in texts)
        {
            var localizeEvent = t.gameObject.GetComponent<LocalizeStringEvent>();
            if (localizeEvent == null)
                localizeEvent = t.gameObject.AddComponent<LocalizeStringEvent>();

            // Objenin adını Key olarak kullanmayı dener
            localizeEvent.StringReference.SetReference(tableName, t.gameObject.name);
            Debug.Log(t.name + " objesi '" + t.gameObject.name + "' anahtarıyla bağlandı.");
        }
    }
}