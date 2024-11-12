using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // This imports SceneManager

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;
    public Transform boss;
    private BossController controller;
    public GameObject bossHealthBar;
    private Slider slider;
    public float displayDistance = 10f;

    public string endSceneName = "EndScene"; // Add the name of your end scene
    private bool hasTriggeredEndScene = false; // Flag to prevent multiple triggers

    void Start()
    {
        bossHealthBar.SetActive(false);
        slider = bossHealthBar.GetComponent<Slider>();
        controller = boss.GetComponent<BossController>();
        slider.maxValue = controller.maxHealth;
    }

    void Update()
    {
        if (boss)
        {
            float distance = Vector3.Distance(player.position, boss.position);
            slider.value = controller.currentHealth;

            // Check for boss death
            if (controller.currentHealth <= 0 && !hasTriggeredEndScene)
            {
                hasTriggeredEndScene = true;
                StartCoroutine(LoadEndScene());
            }

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
        }
    }

    IEnumerator LoadEndScene()
    {
        // Optional: Add a small delay before loading the end scene
        yield return new WaitForSeconds(2f);

        // Load the end scene - Fixed this line
        SceneManager.LoadScene("EndScene");
    }
}