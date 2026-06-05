using UnityEngine;

public class ShipSelector : MonoBehaviour
{
    [Header("Modelos de naves")]
    public GameObject shipDefault;
    public GameObject shipCloud;
    public GameObject shipUFO;

    [Header("Malla del jugador")]
    public GameObject playerMesh;

    void Start()
    {
        string equipped = PlayerPrefs.GetString("Equipped_Ship", "Default");

        shipDefault.SetActive(equipped == "Default");
        shipCloud.SetActive(equipped == "Cloud");
        shipUFO.SetActive(equipped == "UFO");

        playerMesh.SetActive(equipped != "UFO");
    }
}