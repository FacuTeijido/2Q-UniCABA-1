/*

 Author: Facu Teijido
 Email: facundoteijido2003@gmail.com

 Creation Date: 2026-10-20 

 

*/using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Character character;   // vacío = busca el Character en el padre
    [SerializeField] RectTransform fill;    // la imagen que se encoge
    [SerializeField] bool faceCamera;       // true para barras flotando sobre enemigos
    [SerializeField] Color fullColor = new Color(0.2f, 0.85f, 0.3f);
    [SerializeField] Color emptyColor = new Color(0.9f, 0.15f, 0.15f);

    Image fillImage;
    Camera cam;

    void Awake()
    {
        if (character == null) character = GetComponentInParent<Character>();
        fillImage = fill.GetComponent<Image>();

        // El relleno se estira de izquierda a derecha según la vida
        fill.anchorMin = Vector2.zero;
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
    }

    void OnEnable()
    {
        if (character == null) return;

        character.OnHealthChanged += Refresh;
        Refresh(character.CurrentHealth, character.MaxHealth);
    }

    void OnDisable()
    {
        // Con el pool, la barra se desactiva junto con su enemigo: hay que desuscribirse
        if (character != null) character.OnHealthChanged -= Refresh;
    }

    void LateUpdate()
    {
        if (!faceCamera) return;

        if (cam == null) cam = Camera.main;
        if (cam != null) transform.rotation = cam.transform.rotation;
    }

    void Refresh(float current, float max)
    {
        float ratio = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        fill.anchorMax = new Vector2(ratio, 1f);
        fillImage.color = Color.Lerp(emptyColor, fullColor, ratio);
    }
}