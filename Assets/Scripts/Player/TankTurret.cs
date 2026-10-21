/*
 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com
 Creation Date: 2026-10-13 
*/using UnityEngine;
using UnityEngine.InputSystem;

public class TankTurret : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 180f;   
    [SerializeField, Range(0f, 180f)] float maxAngle = 90f; // Ángulo máximo de rotación del cañón respecto al eje del tanque
    [SerializeField] float angleOffset = 0f;    

    Camera cam;
    float currentAngle; 
    void Awake()
    {
        cam = Camera.main;
        if (cam == null)
            Debug.LogError("TankTurret: no hay ninguna cámara con el tag MainCamera.");
    }

    void Update()
    {
        if (cam == null || Mouse.current == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane plane = new Plane(Vector3.up, transform.position);
        if (!plane.Raycast(ray, out float dist)) return;

        Vector3 worldDir = ray.GetPoint(dist) - transform.position;
        worldDir.y = 0f;
        if (worldDir.sqrMagnitude < 0.01f) return;

        Transform body = transform.parent;
        Vector3 localDir = body != null ? body.InverseTransformDirection(worldDir) : worldDir;

        float targetAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
        targetAngle = Mathf.Clamp(targetAngle, -maxAngle, maxAngle);

        float step = rotationSpeed * Time.deltaTime;
        currentAngle = maxAngle >= 180f
            ? Mathf.MoveTowardsAngle(currentAngle, targetAngle, step) 
            : Mathf.MoveTowards(currentAngle, targetAngle, step);     

        transform.localRotation = Quaternion.Euler(0f, currentAngle + angleOffset, 0f);
    }

    void OnDrawGizmosSelected()
    {
        Transform body = transform.parent != null ? transform.parent : transform;
        Gizmos.color = Color.yellow;

        Vector3 right = body.TransformDirection(Quaternion.Euler(0f, maxAngle, 0f) * Vector3.forward);
        Vector3 left = body.TransformDirection(Quaternion.Euler(0f, -maxAngle, 0f) * Vector3.forward);

        Gizmos.DrawRay(transform.position, right * 5f);
        Gizmos.DrawRay(transform.position, left * 5f);
    }
}