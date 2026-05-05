using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // შეამოწმე Player-ია თუ სხვა object
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false); // Coin ქრება
        }
    }
}