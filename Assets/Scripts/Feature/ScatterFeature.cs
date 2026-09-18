using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ScatterFeature : MonoBehaviour
{
    [SerializeField] private ItemInstance item;
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Transform startTransform;
    [SerializeField] private ParticleSystem particle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Start()
    {
        startTransform = gameObject.transform;
    }

    void Update()
    {
        if (item == playerCarry.CurrentItem && !item.ItemState.IsPacked)
        {
            particle.Play();
            gameManager.PlacementController.Place();
        }
    }

    // IEnumerator Regenerate()
    // {

    // }
}
