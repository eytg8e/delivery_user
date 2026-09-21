using System;
using TMPro;
using UnityEngine;
using UnityEngine.AdaptivePerformance.Provider;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject ItemStatusUI;
    [SerializeField] private GameObject HUD;

    [Header("배송 상황 전달")]
    [SerializeField] private TextMeshProUGUI currentDeliveryText;
    [SerializeField] private GameObject deliveryIconBar;
    [SerializeField] private GameObject incompletedIcon;
    [SerializeField] private GameObject completedIcon;

    [Header("조작키 표시 버튼")]
    [SerializeField] private GameObject controlStatusBar;
    [SerializeField] private GameObject pickupOn;
    [SerializeField] private GameObject pickupOff;
    [SerializeField] private GameObject packOn;
    [SerializeField] private GameObject packOff;
    [SerializeField] private GameObject packedOnlyWarning;
    [SerializeField] private GameObject activateOn;
    [SerializeField] private GameObject activateOff;
    [SerializeField] private GameObject rotateOn;
    [SerializeField] private GameObject rotateOff;

    [Header("우측 하단 택배 패널 텍스트")]
    [SerializeField] private TextMeshProUGUI receiverText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI destinationText;
    [SerializeField] private GameObject pictogramList;
    [SerializeField] private GameObject blowPictogram;
    [SerializeField] private GameObject glidePictogram;
    [SerializeField] private GameObject scatterPictogram;
    [SerializeField] private GameObject fragilePictogram;
    [SerializeField] private GameObject bouncePictogram;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateDeliveryStateUI(int currentNum, int goalNum, bool[] completedList)
    {
        foreach (Transform child in deliveryIconBar.transform)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        currentDeliveryText.text = $"Complete: {currentNum} / {goalNum}";

        for (int i = 0; i < completedList.Length; i++)
        {
            if (completedList[i]) Instantiate(completedIcon, deliveryIconBar.transform);
            else Instantiate(incompletedIcon, deliveryIconBar.transform);
        }
    }

    public void UpdateControlPanelUI()
    {
        ItemInstance currentUITarget = gameManager.CurrentUITarget;
        if (currentUITarget == null)
        {
            DeactivateAllControlPanels();
            return;
        }
        else
        {
            ActivateAllControlPanels();
        }
        if (currentUITarget.ItemData.CarryCondition == CarryCondition.PackedOnly)
        {
            if (currentUITarget.IsCarried)
            {
                packOn.SetActive(false);
                packOff.SetActive(false);
                packedOnlyWarning.SetActive(true);
            }

            else if (!currentUITarget.ItemState.IsPacked)
            {
                pickupOn.SetActive(false);
                pickupOff.SetActive(true);
            }
        }

        if (currentUITarget.ItemState.IsPacked)
        {
            activateOn.SetActive(false);
            activateOff.SetActive(true);
        }

        if (!currentUITarget.ItemData.CanActivate)
        {
            activateOn.SetActive(false);
            activateOff.SetActive(true);
        }
    }

    public void DeactivateAllControlPanels()
    {
        pickupOn.SetActive(false);
        pickupOff.SetActive(true);
        packOn.SetActive(false);
        packOff.SetActive(true);
        packedOnlyWarning.SetActive(false);
        activateOn.SetActive(false);
        activateOff.SetActive(true);
        rotateOn.SetActive(false);
        rotateOff.SetActive(true);
    }

    public void ActivateAllControlPanels()
    {
        pickupOn.SetActive(true);
        pickupOff.SetActive(false);
        packOn.SetActive(true);
        packOff.SetActive(false);
        packedOnlyWarning.SetActive(false);
        activateOn.SetActive(true);
        activateOff.SetActive(false);
        rotateOn.SetActive(true);
        rotateOff.SetActive(false);
    }

    public void UpdatePackedItemStateUI()
    {
        ItemStatusUI.SetActive(false);
    }

    public void UpdatePackedItemStateUI(String receiver, String category, String destination)
    {
        ResetPictograms();

        receiverText.text = receiver;
        categoryText.text = $"{category}, {gameManager.CurrentUITarget.ItemData.Weight}kg";
        destinationText.text = destination;


        ItemFeature[] features = gameManager.CurrentUITarget.ItemData.Features;


        foreach (ItemFeature feature in features)
        {
            Debug.Log(feature);
            switch (feature)
            {
                case ItemFeature.Blow:
                    blowPictogram.SetActive(true);
                    break;
                case ItemFeature.Bounce:
                    bouncePictogram.SetActive(true);
                    break;
                case ItemFeature.Fragile:
                    fragilePictogram.SetActive(true);
                    break;
                case ItemFeature.Glide:
                    glidePictogram.SetActive(true);
                    break;
                case ItemFeature.Scatter:
                    scatterPictogram.SetActive(true);
                    break;
            }
        }

        ItemStatusUI.SetActive(true);
    }

    void ResetPictograms()
    {
        blowPictogram.SetActive(false);
        bouncePictogram.SetActive(false);
        fragilePictogram.SetActive(false);
        glidePictogram.SetActive(false);
        scatterPictogram.SetActive(false);
    }
}
