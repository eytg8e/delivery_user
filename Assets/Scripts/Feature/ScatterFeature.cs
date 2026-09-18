using System.Collections;
using UnityEngine;

public class ScatterFeature : MonoBehaviour
{
    [SerializeField] private ItemInstance item;
    [SerializeField] private PlayerCarry playerCarry;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Quaternion startRotation;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private bool startPacked;
    [SerializeField] private bool startActivated;

    [SerializeField] private ParticleSystem particle;

    [SerializeField] private float scatteringTime = 0f;
    [SerializeField] private float duration = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;

        startPacked = item.ItemState.IsPacked;
        startActivated = item.ItemState.IsActive;
    }

    void Update()
    {
        if (item == playerCarry.CurrentItem && !item.ItemState.IsPacked)
        {
            if (item.gameObject.layer != LayerMask.NameToLayer("Scattering"))
            {
                particle.Play();
                gameManager.PlacementController.Place();
                StartCoroutine("Disappear");
            }
        }
    }

    IEnumerator Disappear()
    {
        item.gameObject.layer = LayerMask.NameToLayer("Scattering");
        item.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        while (scatteringTime <= duration)
        {
            scatteringTime += Time.deltaTime;
            transform.localScale = startScale * Mathf.Lerp(1f, 0f, scatteringTime / duration);
            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = startScale;

        item.ItemState.IsPacked = startPacked;
        item.ItemState.IsActive = startActivated;

        item.gameObject.layer = LayerMask.NameToLayer("Stackable");
        item.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        if (playerCarry.CurrentItem != null) playerCarry.Cancel();
        scatteringTime = 0f;
    }
}
