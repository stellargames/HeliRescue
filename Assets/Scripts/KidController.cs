using UnityEngine;

public class KidController : MonoBehaviour
{
    private static readonly int AnimatorRun = Animator.StringToHash("Run");
    [SerializeField] private Animator animator;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var helicopter = other.attachedRigidbody.gameObject
            .GetComponent<HelicopterController>();
        var helicopterIsLanded = helicopter.IsLanded;
        animator.SetBool(AnimatorRun, helicopterIsLanded);

        if (helicopterIsLanded)
        {
            var distance = helicopter.transform.position.x - transform.position.x;
            if (Mathf.Abs(distance) < 0.5f)
                BeRescued();
            else
                MoveTowardsHelicopter(distance);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        animator.SetBool(AnimatorRun, false);
    }

    private void MoveTowardsHelicopter(float distance)
    {
        var position = transform.position;
        var goingLeft = distance < 0f;
        spriteRenderer.flipX = goingLeft;
        var speed = goingLeft ? -runSpeed : runSpeed;
        var positionX = position.x + speed * Time.fixedDeltaTime;
        transform.position = new Vector2(positionX, position.y);
    }

    private void BeRescued()
    {
        animator.SetBool(AnimatorRun, false);
        Destroy(gameObject, 1f);
    }
}
