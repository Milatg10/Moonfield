using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;

    public Animator animator;

    private void Update()
    {
        // Obtener input del jugador
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Crear vector de movimiento
        Vector3 direction = new Vector3(horizontal, vertical).normalized;

        AnimateMovement(direction);

        // Aplicar movimiento
        transform.position += direction * speed * Time.deltaTime;
    }

    void AnimateMovement(Vector3 direction)
    {
        if (animator != null)
        {
                if (direction.magnitude > 0)
            {
                animator.SetBool("IsMoving", true);

                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.y);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }
}
