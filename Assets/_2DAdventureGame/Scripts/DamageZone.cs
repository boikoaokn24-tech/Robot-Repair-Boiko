using UnityEngine;
public class DamageZone : MonoBehaviour
{
    public int damage = 1;
    void OnTriggerStay2D(Collider2D other)
    { var player = other.GetComponent<PlayerController>(); if (player != null) player.ChangeHealth(-damage); }
}
