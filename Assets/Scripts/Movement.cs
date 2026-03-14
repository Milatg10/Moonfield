using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public Animator animator;
    
    // 1. Creamos una variable para el cuerpo físico
    private Rigidbody2D rb;
    private Vector2 direction;

    private void Start()
    {
        // 2. Al empezar, buscamos el Rigidbody2D del personaje
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Obtener input del jugador (esto se queda en Update para que sea rápido)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Crear vector de movimiento
        direction = new Vector2(horizontal, vertical).normalized;

        AnimateMovement(direction);
    }

    // 3. NUEVO: Usamos FixedUpdate para mover el Rigidbody (Físicas seguras)
    private void FixedUpdate()
    {
        // Movemos el cuerpo físico suavemente sin atravesar paredes
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