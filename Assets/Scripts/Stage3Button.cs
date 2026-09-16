using UnityEngine;

public class Stage3Button : MonoBehaviour
{
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject == player) bridge.SetActive(true);
    }
}
