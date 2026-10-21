/*
 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com
 Creation Date: 2026-10-13 
*/
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TankMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float turnSpeed = 120f;

    Rigidbody rb;
    float moveInput;
    float turnInput;

    [Header("Ruido")]
    [SerializeField] float moveNoiseRadius = 10f;
    [SerializeField] float moveNoiseInterval = 0.5f;
    float noiseTimer;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        moveInput = (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0);
        turnInput = (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0);

        EmitMoveNoise();
    }

    void EmitMoveNoise()
    {
        if (moveInput == 0f)
        {
            noiseTimer = 0f; // quieto = en silencio
            return;
        }

        noiseTimer -= Time.deltaTime;
        if (noiseTimer <= 0f)
        {
            noiseTimer = moveNoiseInterval;
            NoiseSystem.Emit(transform.position, moveNoiseRadius);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Vector3 step = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + step);

        Quaternion turn = Quaternion.Euler(0f, turnInput * turnSpeed * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
    
    void OnDisable()
    {
        moveInput = 0f;
        turnInput = 0f;
    }
}