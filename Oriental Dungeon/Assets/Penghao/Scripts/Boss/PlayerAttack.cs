using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject swordSlashPrefab; // 刀光预制体
    public float attackDuration = 0.3f; // 刀光显示的持续时间
    public float attackCooldown = 0.5f; // 攻击的冷却时间
    public int damage = 10;             // 玩家攻击力

    private bool canAttack = true;      // 是否可以攻击
    private GameObject swordSlash;      // 刀光实例

    void Start()
    {
        // 实例化刀光并设置为玩家的子对象，初始隐藏
        swordSlash = Instantiate(swordSlashPrefab, transform);
        SwordSlash swordSlashScript = swordSlash.GetComponent<SwordSlash>();
        if (swordSlashScript != null)
        {
            swordSlashScript.damage = damage; // 将玩家的攻击力传递给刀光
        }
        swordSlash.SetActive(false); // 隐藏刀光
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) // 检测左键按下
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        // 设置刀光相对位置和方向
        if (transform.localScale.x < 0) // 玩家朝左
        {
            swordSlash.transform.localPosition = new Vector3(0.3f, 0, 0); // 刀光偏左
            swordSlash.transform.localRotation = Quaternion.Euler(0, 180, 0); // 翻转刀光
        }
        else // 玩家朝右
        {
            swordSlash.transform.localPosition = new Vector3(-0.3f, 0, 0); // 刀光偏右
            swordSlash.transform.localRotation = Quaternion.identity; // 保持默认方向
        }

        // 显示刀光
        swordSlash.SetActive(true);

        // 等待攻击持续时间
        yield return new WaitForSeconds(attackDuration);

        // 隐藏刀光
        swordSlash.SetActive(false);

        // 等待冷却时间
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }
}
