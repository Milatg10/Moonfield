using UnityEngine;

// Controla el movimiento del jugador mediante un Rigidbody2D.
// La lectura de input ocurre en Update y el desplazamiento físico en FixedUpdate
// para evitar que el personaje atraviese colisionadores a velocidades altas.
public class Movement : MonoBehaviour
{
    [Header("Estado")]
    public bool puedeMoverse = true;
    public float speed = 5f;
    public Animator animator;

    private Rigidbody2D rb;
    private Vector2 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!puedeMoverse)
        {
            // Se resetean dirección y velocidad para que FixedUpdate no aplique
            // el último vector leído mientras el movimiento estaba permitido
            direction = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            AnimateMovement(Vector2.zero);
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // normalized garantiza velocidad constante en diagonal (sin el √2 de velocidad extra)
        direction = new Vector2(horizontal, vertical).normalized;

        AnimateMovement(direction);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    void AnimateMovement(Vector2 dir)
    {
        if (animator != null)
        {
            if (dir.magnitude > 0)
            {
                animator.SetBool("IsMoving", true);
                animator.SetFloat("horizontal", dir.x);
                animator.SetFloat("vertical", dir.y);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }
}
