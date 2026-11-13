using System.Collections;
using UnityEngine;
public class TimedSwitch : MonoBehaviour
{
    public GameObject door;
    public float openTime = 4f;
    public SpriteRenderer indicator;
    public TimedSwitch controller;
    public SpriteRenderer[] linkedIndicators;
    Color initialColor;
    void Awake() { if (indicator != null) initialColor = indicator.color; }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;
        if (controller != null) { controller.Activate(); return; }
        Activate();
    }
    public void Activate()
    {
        if (door == null) return;
        StopAllCoroutines(); StartCoroutine(OpenDoor());
    }
    IEnumerator OpenDoor()
    {
        door.SetActive(false);
        SetIndicators(new Color(.3f, 1f, .6f));
        UIHandler.instance?.ShowHint("Двері відкрито на " + openTime.ToString("0") + " с. Проходь!");
        yield return new WaitForSeconds(openTime);
        // Never close on the player: wait until the doorway is free.
        var renderer = door.GetComponent<SpriteRenderer>();
        var bounds = renderer.bounds;
        while (PlayerInside(bounds)) yield return null;
        door.SetActive(true);
        SetIndicators(initialColor);
    }
    void SetIndicators(Color color)
    {
        if (indicator != null) indicator.color = color;
        if (linkedIndicators != null) foreach (var linked in linkedIndicators) if (linked != null) linked.color = color;
    }
    bool PlayerInside(Bounds bounds)
    {
        foreach (var hit in Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0))
            if (hit.GetComponent<PlayerController>() != null) return true;
        return false;
    }
    void OnDisable() { StopAllCoroutines(); if (controller == null && door != null) door.SetActive(false); }
}
