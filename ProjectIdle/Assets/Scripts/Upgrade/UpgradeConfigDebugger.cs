using UnityEngine;
using System.Collections.Generic;

public class UpgradeConfigDebugger : MonoBehaviour
{
    public List<UpgradeConfig> upgradeConfigs;

    void Start()
    {
        Debug.Log("🔍 UpgradeConfig Debugger Başladı");

        if (upgradeConfigs == null || upgradeConfigs.Count == 0)
        {
            Debug.LogWarning("⚠️ upgradeConfigs listesi boş!");
            return;
        }

        for (int i = 0; i < upgradeConfigs.Count; i++)
        {
            var config = upgradeConfigs[i];

            if (config == null)
            {
                Debug.LogError($"❌ [{i}] Null UpgradeConfig bulundu!");
                continue;
            }

            if (string.IsNullOrWhiteSpace(config.upgradeName))
                Debug.LogWarning($"⚠️ [{i}] upgradeName boş! (Asset: {config.name})");

            if (config.icon == null)
                Debug.LogWarning($"⚠️ [{i}] icon eksik (Asset: {config.name})");

            Debug.Log($"✅ [{i}] OK: {config.name} (Name: {config.upgradeName})");
        }

        Debug.Log("✅ Debugger tamamlandı.");
    }
}
