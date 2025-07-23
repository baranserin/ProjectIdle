using UnityEngine;

public class deneme : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        public ProductConfig ItemtoCheck;

        if(ItemtoCheck.productName == "Çay") { }

        if (teaLevel >= 5 && !orangeTeaUpgradeButton.activeSelf)
        {
            orangeTeaUpgradeButton.SetActive(true);
        }
    }
}
