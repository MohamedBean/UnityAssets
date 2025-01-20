using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HasHealth : MonoBehaviour
{
    [HideInInspector] public float currentHealth;
    public float startingHealth;
    public GameObject healthBar;
    [HideInInspector] GameObject healthBarInstance;
    public bool displayHealthBar;

    void Start()
    {
        currentHealth = startingHealth;
        if (displayHealthBar)
        {
            DisplayHealthBar();
        }
    }

    private void Update()
    {
        CheckForBarToggle();
        CheckForDeath();
        UpdateHealthBar();
    }

    public void CheckForDeath()
    {
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Took Damage!");
    }

    public void DisplayHealthBar()
    {
        healthBarInstance = Instantiate(healthBar, transform);
        healthBarInstance.transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, 0f);
        healthBarInstance.transform.localScale = new Vector3(1, 0.1f, 1);
    }
    public void DestroyHealthBar()
    {
        Destroy(healthBar);
    }

    public void UpdateHealthBar()
    {
        if (displayHealthBar)
        {
            float x = 1 * (currentHealth / startingHealth);
            healthBarInstance.transform.localScale = new Vector3(x, 0.1f, 1);
        }
    }

    public void CheckForBarToggle()
    {
        if (displayHealthBar && healthBarInstance == null)
        {
            Debug.Log("Displayed bar!");
            DisplayHealthBar();
        }
        if (!displayHealthBar && healthBarInstance != null)
        {
            Debug.Log("Destroyed bar!");
            Destroy(healthBarInstance);
        }
    }



}
