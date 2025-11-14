using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    public float boostAmount = 2f;       
    public float boostDuration = 5f;     

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.StartCoroutine(player.ApplySpeedBoost(boostAmount, boostDuration));
            gameObject.SetActive(false);
        }
    }
}