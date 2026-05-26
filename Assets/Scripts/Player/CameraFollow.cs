using UnityEngine;

/// <summary>
/// Caravan Kitchen — CameraFollow.cs
/// Cámara 2D suave que sigue al jugador con suavizado y límites de zona.
/// Incluye zoom adaptativo según velocidad y efecto de sacudida (shake).
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }

    // ─── INSPECTOR ────────────────────────────────────────────────────────
    [Header("Target")]
    public Transform target;
    [Range(1f, 20f)] public float followSmoothing = 6f;

    [Header("Offset y Z")]
    public Vector2 offset = Vector2.zero;
    public float cameraZ   = -10f;

    [Header("Límites de zona")]
    public bool  useBounds = false;
    public float minX, maxX, minY, maxY;

    [Header("Zoom adaptativo")]
    public float normalSize   = 5f;
    public float runningSize  = 6.5f;
    [Range(1f, 10f)] public float zoomSmoothing = 4f;
    private Camera cam;

    [Header("Shake")]
    [Range(0f, 1f)] public float shakeIntensity = 0.15f;
    private float shakeTimer = 0f;
    private Vector3 shakeOffset;

    // ─── INICIALIZACIÓN ───────────────────────────────────────────────────
    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    // ─── LATE UPDATE (después de física) ──────────────────────────────────────
    void LateUpdate()
    {
        if (target == null) return;

        // Shake
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            shakeOffset = Random.insideUnitSphere * shakeIntensity;
        }
        else shakeOffset = Vector3.zero;

        // Posición objetivo
        Vector3 desired = new Vector3(
            target.position.x + offset.x + shakeOffset.x,
            target.position.y + offset.y + shakeOffset.y,
            cameraZ);

        // Límites de zona
        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minX, maxX);
            desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        // Suavizado
        transform.position = Vector3.Lerp(transform.position, desired, followSmoothing * Time.deltaTime);

        // Zoom adaptativo según velocidad del jugador
        if (cam != null)
        {
            var player = target.GetComponent<PlayerController>();
            float targetSize = (player != null && player.IsRunning) ? runningSize : normalSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSmoothing * Time.deltaTime);
        }
    }

    // ─── SHAKE PÚBLICO ───────────────────────────────────────────────────
    public void Shake(float duration = 0.2f) => shakeTimer = duration;

    // ─── CONFIGURAR LÍMITES DESDE ZONA ──────────────────────────────────────
    public void SetBounds(float xMin, float xMax, float yMin, float yMax)
    {
        useBounds = true;
        minX = xMin; maxX = xMax;
        minY = yMin; maxY = yMax;
    }

    public void ClearBounds() => useBounds = false;
}
