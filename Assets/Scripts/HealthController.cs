using System.Collections;
using System.Collections.Generic;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    public int maxHealth = 4;
    public int currentHealth;
    [SerializeField] public Image[] hearts;
    [SerializeField] public Sprite fullHeart;
    [SerializeField] public Sprite emptyHeart;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

    }

    private void UpdateUI()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
