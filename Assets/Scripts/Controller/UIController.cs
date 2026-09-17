using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameManager GameManager;
    [SerializeField] private GameObject ItemStatusUI;
    [SerializeField] private GameObject HUD;
    [SerializeField] private TextMeshProUGUI stickerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateWeightHUD(ItemInstance item)
    {

    }

    public void ShowPackedItemStateUI()
    {
        // stickerText.text = ;
    }

    public void ShowUnPackedItemStateUI()
    {

    }
}
