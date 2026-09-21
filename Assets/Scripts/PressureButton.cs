using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UIElements;

public class PressureButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<ItemInstance> pressingItems;

    [SerializeField] private GameObject connectedGameObject;
    [SerializeField] private float sum;
    [SerializeField] private float threshold = 10f;
    [SerializeField] private bool isPressed = false;

    [SerializeField] private GameObject pressedMesh;
    [SerializeField] private GameObject normalMesh;
    [SerializeField] private TextMeshPro massCheckerText;

    public bool IsPressed => isPressed;

    void Awake()
    {
        pressingItems = new List<ItemInstance>();
        Deactivate();
    }

    private void FixedUpdate()
    {
        int removedCount = pressingItems.RemoveAll(item => item == null || item.IsCarried || !item.gameObject.activeInHierarchy);

        if (removedCount > 0) CheckPressed();
    }

    private void OnTriggerEnter(Collider collider)
    {
        ItemInstance triggeredItem = collider.GetComponentInParent<ItemInstance>();


        Debug.Log(
            $"Enter: {collider.name}, Collider ID: {collider.GetInstanceID()}, " +
            $"Item: {triggeredItem}, Count: {pressingItems.Count}"
        );

        if (triggeredItem == null) return;
        if (pressingItems.Contains(triggeredItem)) return;
        pressingItems.Add(triggeredItem);
        CheckPressed();
    }

    private void OnTriggerExit(Collider collider)
    {
        ItemInstance triggeredItem = collider.gameObject.GetComponentInParent<ItemInstance>();

        Debug.Log(
            $"Exit: {collider.name}, Collider ID: {collider.GetInstanceID()}, " +
            $"Item: {triggeredItem}, Count: {pressingItems.Count}"
        );

        if (triggeredItem == null) return;

        pressingItems.Remove(triggeredItem);
        CheckPressed();
    }

    private void CheckPressed()
    {
        Debug.Log(pressingItems);

        sum = 0f;

        foreach (ItemInstance item in pressingItems)
        {
            sum += item.ItemData.Weight;
        }

        if (sum >= threshold)
        {
            isPressed = true;
            Actiavte();
        }
        else
        {
            isPressed = false;
            Deactivate();
        }

        massCheckerText.text = sum + " / " + threshold + " kg";
    }

    private void Actiavte()
    {
        pressedMesh.SetActive(true);
        normalMesh.SetActive(false);
        connectedGameObject.SetActive(true);
    }

    private void Deactivate()
    {
        pressedMesh.SetActive(false);
        normalMesh.SetActive(true);
        connectedGameObject.SetActive(false);
    }
}
