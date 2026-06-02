using System.Collections;
using UnityEngine;

public class MagicLaserProjectile : BaseProjectile
{
    [SerializeField] private ProjectileSO projectileSO;
    [SerializeField] private float laserGrowTime = 0.2f;

    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider2D;
    private float startSpriteSizeX;
    private float startColliderSizeX;
    private float startColliderOffsetX;
    private Vector3 startPoint;
    private bool isGrowing = true;

    public void Init(ProjectileSO projectileSettings)
    {
        projectileSO = projectileSettings;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        if (spriteRenderer != null)
        {
            startSpriteSizeX = spriteRenderer.size.x;
        }

        if (capsuleCollider2D != null)
        {
            startColliderSizeX = capsuleCollider2D.size.x;
            startColliderOffsetX = capsuleCollider2D.offset.x;
        }
    }

    private void Start()
    {
        LaserFaceMouse();
        startPoint = transform.position;
        StartCoroutine(GrowAndFadeRoutine());
    }

    private void Update()
    {
        if (!isGrowing && projectileSO != null)
        {
            DetectFireDistance(projectileSO, startPoint);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (projectileSO != null && collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(projectileSO.projectileDamageAmout);
        }

        Indestructable indestructable = collision.gameObject.GetComponent<Indestructable>();
        if (indestructable != null && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void LaserFaceMouse()
    {
        if (Camera.main == null || GameInput.Instance == null)
        {
            return;
        }

        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector2 direction = Camera.main.WorldToScreenPoint(transform.position) - mousePos;
        transform.right = -direction;
    }

    private IEnumerator GrowAndFadeRoutine()
    {
        if (spriteRenderer == null || capsuleCollider2D == null)
        {
            Debug.LogError("[MagicLaserProjectile] SpriteRenderer or CapsuleCollider2D is missing.");
            Destroy(gameObject);
            yield break;
        }

        float range = GetLaserRange();
        float timePassed = 0f;

        while (timePassed < laserGrowTime)
        {
            timePassed += Time.deltaTime;
            float t = Mathf.Clamp01(timePassed / laserGrowTime);
            float newSizeX = Mathf.Lerp(startSpriteSizeX, range, t);

            spriteRenderer.size = new Vector2(newSizeX, spriteRenderer.size.y);
            capsuleCollider2D.size = new Vector2(Mathf.Lerp(startColliderSizeX, range, t), capsuleCollider2D.size.y);
            capsuleCollider2D.offset = new Vector2(Mathf.Lerp(startColliderOffsetX, range * 0.5f, t), capsuleCollider2D.offset.y);

            yield return null;
        }

        isGrowing = false;

        SpriteFade spriteFade = GetComponent<SpriteFade>();
        if (spriteFade != null)
        {
            yield return StartCoroutine(spriteFade.FadeRoutine());
        }

        Destroy(gameObject);
    }

    private float GetLaserRange()
    {
        if (projectileSO != null && projectileSO.weaponSO != null && projectileSO.weaponSO.weaponProjectileRange > 0f)
        {
            return projectileSO.weaponSO.weaponProjectileRange;
        }

        if (projectileSO != null && projectileSO.projectileRange > 0f)
        {
            return projectileSO.projectileRange;
        }

        Debug.LogWarning("[MagicLaserProjectile] Laser range is not set. Using fallback range 5.");
        return 5f;
    }
}
