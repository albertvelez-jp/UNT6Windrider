using UnityEngine;
using TMPro;

public class WalletDisplay : MonoBehaviour
{
    public TextMeshProUGUI walletText;

    void Start()
    {
        UpdateWalletUI();
    }

    void UpdateWalletUI()
    {
        int total = PlayerPrefs.GetInt("WalletCoins", 0);
        walletText.text = total.ToString();
    }
}