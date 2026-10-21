/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-13 

 

*/using System;
using System.Collections;
using UnityEngine;

public enum GameState { Playing, Dead, Won }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public Player player;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float respawnDelay = 2f;

    public GameState State { get; private set; } = GameState.Playing;
    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        if (player != null) player.OnDied += HandlePlayerDied;
    }

    void OnDisable()
    {
        // Siempre desuscribirse: evita errores al cambiar de escena o destruir objetos
        if (player != null) player.OnDied -= HandlePlayerDied;
    }

    void Start()
    {
        player.Respawn(spawnPoint); // punto de inicio del jugador
    }

    void HandlePlayerDied(Character who)
    {
        if (State != GameState.Playing) return;

        SetState(GameState.Dead);
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        player.Respawn(spawnPoint);
        SetState(GameState.Playing);
    }

    // La llamará el trigger de la meta
    public void WinLevel()
    {
        if (State != GameState.Playing) return;

        SetState(GameState.Won);
        player.SetControlEnabled(false);
        // Más adelante: cargar el siguiente nivel o mostrar pantalla de fin
    }
    // TEMPORAL: se reemplaza cuando hagamos la UI
    void OnGUI()
    {
        if (State != GameState.Won) return;

        GUI.skin.label.fontSize = 36;
        GUI.Label(new Rect(40f, 40f, 800f, 60f), "¡NIVEL COMPLETADO!");
    }
    void SetState(GameState newState)
    {
        State = newState;
        OnStateChanged?.Invoke(newState); // la UI se va a suscribir acá
    }
}