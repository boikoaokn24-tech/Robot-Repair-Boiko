using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Поля налаштування патруля
    public float speed = 1.0f;
    public bool vertical;
    public float changeTime = 3.0f;
    public int initialDirection = 1;
    [Min(0)] public float initialPatrolOffset = 0;

    // Внутрішні компоненти та змінні
    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool broken = true;
    public bool isBroken => broken;
    public AudioClip repairSound;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = Mathf.Clamp(changeTime - initialPatrolOffset, 0.05f, changeTime);
        direction = initialDirection < 0 ? -1 : 1;
    }

    void Update()
    {
        // Таймер патрулювання
        if (!broken || (GameManager.instance != null && GameManager.instance.ended)) return;
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction; // Зміна напрямку (1 на -1, і навпаки)
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        // Якщо ворог полагоджений — зупиняємо рух
        if (!broken || (GameManager.instance != null && GameManager.instance.ended))
        {
            return;
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.fixedDeltaTime;
            if (animator != null)
            {
                animator.SetFloat("Move X", 0);
                animator.SetFloat("Move Y", direction);
            }
        }
        else
        {
            position.x = position.x + speed * direction * Time.fixedDeltaTime;
            if (animator != null)
            {
                animator.SetFloat("Move X", direction);
                animator.SetFloat("Move Y", 0);
            }
        }

        rigidbody2d.MovePosition(position);
    }

    // Нанесення шкоди при дотику
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    // Метод ремонту ворога
    public void Fix()
    {
        if (!broken) return;
        broken = false;
        var source = GetComponent<AudioSource>();
        if (source != null && repairSound != null) source.PlayOneShot(repairSound);
        rigidbody2d.simulated = false; // Вимикаємо фізику та тригери
        if (animator != null)
        {
            animator.SetTrigger("Fixed");
        }
    }
}

