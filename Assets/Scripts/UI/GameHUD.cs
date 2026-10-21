/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-20 

 

*/using System.Collections;
using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] LevelObjective objective;

    [Header("Textos y paneles")]
    [SerializeField] TMP_Text enemyCounter;
    [SerializeField] TMP_Text hintText;
    [SerializeField] GameObject deathMessage;
    [SerializeField] GameObject winPanel;

    [Header("Escenas")]
    [SerializeField] string menuSceneName = "MainMenu"; // poné el nombre real de tu escena de menú

    void Start()
    {
        deathMessage.SetActive(false);
        winPanel.SetActive(false);
        hintText.gameObject.SetActive(false);

        GameManager.Instance.OnStateChanged += HandleStateChanged;
        objective.OnProgressChanged += UpdateCounter;
        objective.OnCompleted += HandleObjectiveCompleted;

        // Sincroniza por si los enemigos ya habían aparecido antes de suscribirnos
        UpdateCounter(objective.AliveCount, objective.TotalCount);
        ShowHint("Eliminá a todos los enemigos para abrir la salida");
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= HandleStateChanged;

        if (objective != null)
        {
            objective.OnProgressChanged -= UpdateCounter;
            objective.OnCompleted -= HandleObjectiveCompleted;
        }
    }

    void HandleStateChanged(GameState state)
    {
        deathMessage.SetActive(state == GameState.Dead);
        winPanel.SetActive(state == GameState.Won);
    }

    void UpdateCounter(int alive, int total)
    {
        enemyCounter.text = $"Enemigos restantes: {alive}";
    }

    void HandleObjectiveCompleted()
    {
        ShowHint("¡Salida abierta! Llegá a la zona verde");
    }

    void ShowHint(string message)
    {
        StopAllCoroutines();
        StartCoroutine(HintRoutine(message));
    }

    IEnumerator HintRoutine(string message)
    {
        hintText.text = message;
        hintText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        hintText.gameObject.SetActive(false);
    }

    // ---- Botones del panel de victoria (se conectan en el OnClick del Inspector) ----

    public void Retry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }
}