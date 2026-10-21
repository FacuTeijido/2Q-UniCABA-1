/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-15 

 

*/using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LevelExit : MonoBehaviour
{
    [SerializeField] LevelObjective objective;
    [SerializeField] GameObject barrier;   // pared que bloquea mientras está cerrada (opcional)
    [SerializeField] Renderer marker;      // marca en el piso que cambia de color (opcional)
    [SerializeField] Color closedColor = Color.red;
    [SerializeField] Color openColor = Color.green;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnEnable()
    {
        if (objective != null) objective.OnCompleted += Open;
    }

    void OnDisable()
    {
        if (objective != null) objective.OnCompleted -= Open;
    }

    void Start()
    {
        SetOpen(false);
    }

    void Open()
    {
        SetOpen(true);
    }

    void SetOpen(bool open)
    {
        IsOpen = open;
        if (barrier != null) barrier.SetActive(!open);
        if (marker != null) marker.material.color = open ? openColor : closedColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsOpen || other.GetComponentInParent<Player>() == null) return;

        Debug.Log($"Salida cerrada: faltan {objective.AliveCount} enemigos.");
    }

    void OnTriggerStay(Collider other)
    {
        if (!IsOpen) return;
        if (other.GetComponentInParent<Player>() == null) return;

        GameManager.Instance.WinLevel();
    }
}