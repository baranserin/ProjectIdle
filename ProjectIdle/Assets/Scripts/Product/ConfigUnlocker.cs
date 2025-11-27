using System.Collections.Generic;
using UnityEngine;

public class ConfigUnlocker : MonoBehaviour
{
    [Header("Açmak istediðin ürün configleri")]
    public List<ProductConfig> configsToUnlock;

    // Bunu buton OnClick'ten çaðýr
    public void UnlockChosenConfigs()
    {
        if (IncomeManager.Instance == null)
        {
            Debug.LogError("IncomeManager.Instance is null!");
            return;
        }

        IncomeManager.Instance.UnlockProductsByConfigs(configsToUnlock);
    }
}
