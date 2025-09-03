using UnityEngine;
using DG.Tweening; // Add this for DOTween

public class CubeResetPos : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;
    private Tween moveTween;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void ResetCube()
    {
        if (rb == null) return;

        // Stop physics simulation
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Kill any existing tween to avoid overlap
        moveTween?.Kill();

        // Animate to original position & rotation
        moveTween = transform.DOMove(originalPosition, 0.6f)
            .SetEase(Ease.InOutQuad);

        transform.DORotateQuaternion(originalRotation, 0.6f)
            .SetEase(Ease.InOutQuad);

        // Re-enable physics after animation ends
        StartCoroutine(ReenablePhysicsAfterDelay(0.6f));
    }

    private System.Collections.IEnumerator ReenablePhysicsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = false;
    }
}
