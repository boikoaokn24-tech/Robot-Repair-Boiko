using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction TalkAction;
    public float speed = 3f;
    public int maxHealth = 5;
    public float timeInvincible = 2f;
    public float timeBetweenShots = .5f;
    public GameObject projectilePrefab;
    public AudioClip hitSound, healSound, launchSound;
    public int health { get; private set; }
    Rigidbody2D body;
    Animator animator;
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    Color baseColor;
    Vector2 move, look = Vector2.right;
    float immunity, shotTimer;
    NonPlayerCharacter lastNPC;
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        health = maxHealth;
    }
    void OnEnable() { MoveAction.Enable(); LaunchAction.Enable(); TalkAction.Enable(); }
    void OnDisable()
    {
        MoveAction.Disable(); LaunchAction.Disable(); TalkAction.Disable();
        if (lastNPC != null) lastNPC.ShowBubble(false);
    }
    void Start() { UIHandler.instance?.SetHealthValue(1f); }
    void Update()
    {
        immunity = Mathf.Max(0, immunity - Time.deltaTime);
        shotTimer = Mathf.Max(0, shotTimer - Time.deltaTime);
        if (GameManager.instance != null && GameManager.instance.ended) { move = Vector2.zero; return; }
        move = Vector2.ClampMagnitude(MoveAction.ReadValue<Vector2>(), 1f);
        if (move.sqrMagnitude > .001f) look = move.normalized;
        if (animator != null)
        {
            animator.SetFloat("Look X", look.x); animator.SetFloat("Look Y", look.y);
            animator.SetFloat("Speed", move.magnitude);
        }
        if (LaunchAction.WasPressedThisFrame() && shotTimer <= 0 && projectilePrefab != null)
        {
            var projectile = Instantiate(projectilePrefab, body.position + Vector2.up * .5f, Quaternion.identity).GetComponent<Projectile>();
            projectile.Launch(look, 300f);
            shotTimer = timeBetweenShots;
            animator?.SetTrigger("Launch"); Play(launchSound);
        }
        var hit = Physics2D.Raycast(body.position + Vector2.up * .2f, look, 1.5f, LayerMask.GetMask("NPC"));
        var npc = hit.collider != null ? hit.collider.GetComponent<NonPlayerCharacter>() : null;
        if (lastNPC != npc && lastNPC != null) lastNPC.ShowBubble(false);
        lastNPC = npc;
        if (npc != null)
        {
            npc.ShowBubble(true);
            if (TalkAction.WasPressedThisFrame()) UIHandler.instance?.DisplayDialogue();
        }
    }
    void FixedUpdate() { body.MovePosition(body.position + move * speed * Time.fixedDeltaTime); }
    public void ChangeHealth(int amount)
    {
        if (health <= 0 || (GameManager.instance != null && GameManager.instance.ended)) return;
        if (amount < 0)
        {
            if (immunity > 0) return;
            immunity = timeInvincible;
            animator?.SetTrigger("Hit"); Play(hitSound);
        }
        int previous = health;
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        if (health == previous) return;
        if (amount > 0) Play(healSound);
        StopAllCoroutines(); StartCoroutine(Flash(amount < 0 ? new Color(1f,.45f,.35f) : new Color(.45f,1f,.65f)));
        UIHandler.instance?.SetHealthValue(health / (float)maxHealth);
    }
    IEnumerator Flash(Color color) { spriteRenderer.color = color; yield return new WaitForSeconds(.16f); spriteRenderer.color = baseColor; }
    void Play(AudioClip clip) { if (audioSource != null && clip != null) audioSource.PlayOneShot(clip); }
}
