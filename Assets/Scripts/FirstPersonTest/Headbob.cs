using UnityEngine;

public class Headbob : MonoBehaviour
{
    [Header("Headbob Settings")]
    public AnimationCurve animCurve;
    public float bobStrength=0.1f;
    public float bobFrequency=3f;
    public float bobReturnSpeed=5f;

    private Vector3 originalCameraLocalPosition;
    private float timer;

    private FirstPersonController firstPersonController;
    private Transform cam;

    private void Start()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        cam = firstPersonController.playerCameraTransform;
        originalCameraLocalPosition = cam.localPosition;
    }

    private void Update()
    {
        if ( firstPersonController.GetMovementInput().magnitude > 0)
            HandleHeadbob();
        else
            ResetHeadbobPosition();
    }

    private void HandleHeadbob()
    {
        timer += Time.deltaTime*bobFrequency;

        Vector3 newCameraPosition = originalCameraLocalPosition + Vector3.up * bobStrength * animCurve.Evaluate(timer);

        cam.localPosition = newCameraPosition;
    }

    private void ResetHeadbobPosition()
    {
        timer = 0;
        cam.localPosition = Vector3.Lerp(cam.localPosition, originalCameraLocalPosition, Time.deltaTime * bobReturnSpeed);
    }
}
