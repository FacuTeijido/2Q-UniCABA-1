/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-20 

 

*/using System;
using UnityEngine;

public static class NoiseSystem
{
    public static event Action<Vector3, float> OnNoise; // (posición, radio)

    public static void Emit(Vector3 position, float radius)
    {
        OnNoise?.Invoke(position, radius);

#if UNITY_EDITOR
        DrawDebugCircle(position, radius); // solo para ver el radio en la Scene view
#endif
    }

#if UNITY_EDITOR
    static void DrawDebugCircle(Vector3 center, float radius)
    {
        const int segments = 32;
        Vector3 prev = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 next = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Debug.DrawLine(prev, next, Color.magenta, 0.5f);
            prev = next;
        }
    }
#endif
}