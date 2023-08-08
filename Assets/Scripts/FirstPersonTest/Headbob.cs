using UnityEngine;

public class Headbob : MonoBehaviour
{
    [Header("Headbob Settings")]
    public AnimationCurve animCurve;
    public float bobStrength=0.1f;
    public float bobFrequency=3f;
    public bool returnPosition;
    public float bobReturnSpeed=5f;

    private float moveIntensity;
    private Vector3 originalCameraLocalPosition;
    private float timer;

    private FirstPersonController firstPersonController;
    private Transform cam;

    private void Start()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        cam = firstPersonController.cam;
        originalCameraLocalPosition = cam.localPosition;
    }

    private void Update()
    {
        moveIntensity = firstPersonController.moveInput.magnitude/Vector2.one.magnitude;
        if (moveIntensity > 0)
            HandleHeadbob();
        else if(returnPosition)
            ResetHeadbobPosition();
    }

    private void HandleHeadbob()
    {
        timer += Time.deltaTime*bobFrequency * moveIntensity;

        Vector3 newCameraPosition = originalCameraLocalPosition + Vector3.up * bobStrength*moveIntensity * animCurve.Evaluate(timer);

        cam.localPosition = newCameraPosition;
    }

    private void ResetHeadbobPosition()
    {
        timer = 0;
        cam.localPosition = Vector3.Lerp(cam.localPosition, originalCameraLocalPosition, Time.deltaTime * bobReturnSpeed);
    }
}
