/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public string displayName = "Enemy";
    public float maxHealth = 50f;

    [Header("Movimiento")]
    public float moveSpeed = 3f;
    public float turnSpeed = 360f; // grados por segundo

    [Header("Combate")]
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("Visión")]
    public float detectionRange = 12f;                 // distancia máxima de visión
    [Range(10f, 360f)] public float viewAngle = 120f;  // ancho del cono de visión
    public float memoryTime = 4f;                      // segundos que busca tras perderlo

    [Header("Audición")]
    [Range(0f, 2f)] public float hearingSensitivity = 1f; // multiplica el radio de cada ruido

    [Header("Patrulla")]
    public float patrolRadius = 15f;                   // radio alrededor de su punto de origen
    [Range(0.1f, 1f)] public float patrolSpeedMultiplier = 0.5f;
}