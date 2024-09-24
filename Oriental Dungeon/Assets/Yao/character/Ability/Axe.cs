using UnityEngine;

public class FlyingAxe : MonoBehaviour
{
    public GameObject axePrefab;
    public float throwForce = 10f;
    public float aimLineLength = 2f;
    public LayerMask woodLayer;
    public string playerTag = "Player";
    public float axeSpawnRadius = 1.5f;
    public Camera mainCamera;
    public CharacterController2D playerController;
    public Vector2 platformSize = new Vector2(1f, 0.1f); // 新增：平台尺寸设置

    private GameObject currentAxe;
    private LineRenderer aimLine;
    private bool isAiming = false;
    private Vector2 throwDirection;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        if (aimLine == null)
        {
            aimLine = gameObject.AddComponent<LineRenderer>();
            aimLine.positionCount = 2;
            aimLine.startWidth = 0.1f;
            aimLine.endWidth = 0.1f;
            aimLine.material = new Material(Shader.Find("Sprites/Default"));
            aimLine.enabled = false;
        }

        if (playerController == null)
        {
            Debug.LogError("CharacterController2D is not assigned. Please assign it in the inspector.");
        }

        if (axePrefab == null)
        {
            Debug.LogError("Axe Prefab is not assigned. Please assign it in the inspector.");
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera not found. Please assign it in the inspector or tag your camera as MainCamera.");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartAiming();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ThrowAxe();
        }

        if (isAiming)
        {
            UpdateAimDirection();
            UpdateAimLine();
        }
    }

    void StartAiming()
    {
        if (aimLine == null)
        {
            Debug.LogError("AimLine is not initialized. Reinitializing components.");
            InitializeComponents();
            return;
        }

        isAiming = true;
        aimLine.enabled = true;
        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
        }
        else
        {
            Debug.LogWarning("CharacterController2D is null. Movement disable failed.");
        }
    }

    void UpdateAimDirection()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        throwDirection = (mousePosition - (Vector2)transform.position).normalized;
    }

    void UpdateAimLine()
    {
        if (aimLine == null) return;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)throwDirection * aimLineLength;
        aimLine.SetPosition(0, startPos);
        aimLine.SetPosition(1, endPos);
    }

    void ThrowAxe()
    {
        if (axePrefab == null)
        {
            Debug.LogError("Axe Prefab is not assigned. Cannot throw axe.");
            return;
        }

        isAiming = false;
        if (aimLine != null) aimLine.enabled = false;

        if (playerController != null)
        {
            playerController.SetMovementEnabled(true);
        }
        else
        {
            Debug.LogWarning("CharacterController2D is null. Movement enable failed.");
        }

        if (currentAxe != null)
        {
            Destroy(currentAxe);
        }

        Vector2 spawnPosition = (Vector2)transform.position + throwDirection * axeSpawnRadius;
        currentAxe = Instantiate(axePrefab, spawnPosition, Quaternion.identity);
        
        AxeBehavior axeBehavior = currentAxe.AddComponent<AxeBehavior>();
        axeBehavior.Initialize(this, woodLayer, playerTag, throwDirection, platformSize);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, axeSpawnRadius);
    }
}

public class AxeBehavior : MonoBehaviour
{
    private FlyingAxe parentScript;
    private LayerMask woodLayer;
    private string playerTag;
    private Vector2 throwDirection;
    private Vector2 platformSize;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private BoxCollider2D axeCollider;
    private bool isStuck = false;
    private float rotationSpeed = 720f;
    private GameObject platform;
    private const string GROUND_LAYER_NAME = "Ground";

    public void Initialize(FlyingAxe parent, LayerMask wood, string playerTagName, Vector2 direction, Vector2 platSize)
    {
        parentScript = parent;
        woodLayer = wood;
        playerTag = playerTagName;
        throwDirection = direction;
        platformSize = platSize;
        SetupComponents();
    }

    void SetupComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        axeCollider = GetComponent<BoxCollider2D>();
        if (axeCollider == null)
        {
            axeCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        axeCollider.size = new Vector2(0.5f, 0.5f);
        axeCollider.isTrigger = true;

        // 设置初始旋转
        float angle = Mathf.Atan2(throwDirection.y, throwDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 添加初始力
        rb.AddForce(throwDirection * parentScript.throwForce, ForceMode2D.Impulse);
    }

    void Update()
    {
        if (!isStuck)
        {
            // 飞行时旋转
            float rotationDirection = throwDirection.x < 0 ? -1 : 1;
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * rotationDirection);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isStuck) return;

        if (((1 << collision.gameObject.layer) & woodLayer) != 0)
        {
            StickToWood(collision);
        }
        else if (collision.gameObject.tag != playerTag)
        {
            Destroy(gameObject);
        }
    }

    void StickToWood(Collider2D collision)
    {
        isStuck = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // 计算碰撞点
        Vector2 collisionPoint = collision.ClosestPoint(transform.position);

        // 生成向上的平台
        platform = new GameObject("AxePlatform");
        platform.transform.position = collisionPoint + Vector2.up * (platformSize.y / 2);
        platform.transform.SetParent(collision.transform);

        // 设置平台的图层为 Ground
        platform.layer = LayerMask.NameToLayer(GROUND_LAYER_NAME);

        // 添加平台组件
        BoxCollider2D platformCollider = platform.AddComponent<BoxCollider2D>();
        platformCollider.size = platformSize;
        platformCollider.offset = Vector2.zero;

        PlatformEffector2D platformEffector = platform.AddComponent<PlatformEffector2D>();
        platformEffector.useOneWay = true;
        platformEffector.useOneWayGrouping = true;
        platformEffector.surfaceArc = 170f;

        // 调整斧头位置和旋转以匹配平台
        transform.position = platform.transform.position + Vector3.up * (platformSize.y / 2);
        transform.rotation = Quaternion.identity; // 重置旋转
        transform.SetParent(platform.transform);

        // 禁用斧头的碰撞器
        axeCollider.enabled = false;

        // 确保精灵渲染器朝向正确
        spriteRenderer.flipY = false;
    }

    void OnDestroy()
    {
        // 当斧头被销毁时，同时销毁平台
        if (platform != null)
        {
            Destroy(platform);
        }
    }
}