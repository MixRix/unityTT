using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;

public class levelManager : MonoBehaviour
{
     ThirdPersonShooterController player;
     Enemy enemy;
    StarterAssetsInputs starterAssetsInputs;

    public GameObject loseScreen;
    public GameObject WinScreen;

    public int wins;
    public int loses;

    public TextMeshProUGUI winLoseDisplay;

    private void Awake()
    {
        player = FindObjectOfType<ThirdPersonShooterController>();
        enemy = FindObjectOfType<Enemy>();
        starterAssetsInputs = FindObjectOfType<StarterAssetsInputs>();
    }

    private void Start()
    {
        LoadPlayerProgress();
    }
    private void Update()
    {
        if (player != null && enemy != null)
        {
            if (!player.isAlivePlayer)
            {
                starterAssetsInputs.cursorLocked = false;
                loseScreen.SetActive(true);
                loses++;
                player.isAlivePlayer = true;
            } 
            if (!enemy.isAliveEnemy)
            {
                starterAssetsInputs.cursorLocked = false;
                WinScreen.SetActive(true);
                wins++;
                enemy.isAliveEnemy = true;
            }
            
        }

        SavePlayerProgress();
        winLoseDisplay.SetText("wins:" + wins + " / " + "loses: " + loses);
        
    }
    public void ResetTheGame()
    {
        
        
        
        loseScreen.SetActive(false);
        WinScreen.SetActive(false);
        starterAssetsInputs.cursorLocked = true;
        Time.timeScale = 1f;
        print("Scene restarted");
        SavePlayerProgress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


    }
    
    
        public void SavePlayerProgress()
        {
            PlayerPrefs.SetInt("Wins", wins);
            PlayerPrefs.SetInt("Loses", loses);
            PlayerPrefs.Save();
        }

        public void LoadPlayerProgress()
        {
            wins = PlayerPrefs.GetInt("Wins");
            loses = PlayerPrefs.GetInt("Loses");
        }
    public void DeleteData()
    {
        PlayerPrefs.DeleteKey("Wins");
        PlayerPrefs.DeleteKey("Loses");
    }
    
}
