using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class Wheel : MonoBehaviour
{
    public GameObject obj;
    public XRKnob xRKnob;

    private void Update()
    {
        float yRot = xRKnob.value * 45f;

        obj.transform.rotation = Quaternion.Euler(0, yRot,0);
    }
}
