using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public GameObject collectEffect;
    public SoundManager SoundManagerScript;

    [Header("Persistent ID")]
    public string coinID;

    private void Start()
    {
        SoundManagerScript = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

        if (PlayerPrefs.GetInt(coinID, 0) == 1)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SoundManagerScript.SelectAudio(3, 8);
        GameManager.instance.AddCoins(coinValue, coinID); // le pasamos el ID al GameManager

        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}