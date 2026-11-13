using UnityEngine;
public class HealthCollectible : MonoBehaviour
{
    public int amount = 1;
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && player.health > 0 && player.health < player.maxHealth)
        { player.ChangeHealth(amount); Destroy(gameObject); }
    }
}
