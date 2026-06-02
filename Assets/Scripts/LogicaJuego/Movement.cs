using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Estado")]
    public bool puedeMoverse = true;
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
        if (!puedeMoverse)
        {
            // --- ¡NUEVO Y CRUCIAL! ---
            // Forzamos la dirección a cero para que FixedUpdate no mueva el Rigidbody
            direction = Vector2.zero; 
            // --------------------------

            // Clavamos los frenos de la física poniéndola a cero
            // Nota: GetComponent<Rigidbody2D>() está bien, pero 'rb' es más rápido ya que lo buscamos en Start
            rb.linearVelocity = Vector2.zero;

            AnimateMovement(Vector2.zero); // Apaga la animación de caminar

            // Nos salimos para no leer las teclas
            return; 
        }
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