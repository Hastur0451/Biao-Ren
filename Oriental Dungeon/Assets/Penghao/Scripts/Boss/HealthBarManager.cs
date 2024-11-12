using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player; // 玩家对象
    public Transform boss;   // Boss对象
    private BossController controller;
    public GameObject bossHealthBar; // Boss的血条UI
    private Slider slider;
    public float displayDistance = 10f; // 显示血条的距离

    // Start is called before the first frame update
    void Start()
    {
        // 初始状态隐藏血条
        bossHealthBar.SetActive(false);
        slider = bossHealthBar.GetComponent<Slider>();
        controller = boss.GetComponent<BossController>();
        slider.maxValue = controller.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (boss) { 
            // 计算玩家和Boss之间的距离
            float distance = Vector3.Distance(player.position, boss.position);
            slider.value = controller.currentHealth;
            // 如果距离小于显示距离，则显示血条；否则隐藏
            if (distance <= displayDistance)
            {
                bossHealthBar.SetActive(true);
            }
            else
            {
                bossHealthBar.SetActive(false);
            }
        }
        else
        {
            bossHealthBar.SetActive(false);
            //如果要加通关显示之类的可以在这里加
        }
    }
}
