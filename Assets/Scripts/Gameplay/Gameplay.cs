using UnityEngine;      
using UnityEngine.AI;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class Gameplay : MonoBehaviour
{
    private bool gameActive = false; 
    private bool gamePaused = false;
    
    [Header("Game Settings")]
    public float battleRange = 3f;

    [Header("UI References")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Button[] replayButtons;

    void Start()
    {
        if(winPanel) winPanel.SetActive(false);
        if(losePanel) losePanel.SetActive(false);
        
        foreach (Button btn in replayButtons)
        {
            if(btn != null) btn.onClick.AddListener(RestartGame);
        }
    }
    
    void Update()
    {
        if (gamePaused) return;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            if (player != null && enemy != null)
            {
                float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                
                if (distance < battleRange && !gameActive)
                {
                    // FIXED: Now looking for NPCCounter instead of FollowerCounter
                    NPCCounter playerCounter = player.GetComponent<NPCCounter>();
                    NPCCounter enemyCounter = enemy.GetComponent<NPCCounter>();
                    
                    if (playerCounter != null && enemyCounter != null)
                    {
                        // FIXED: Calling GetNPCCount() to match your new script
                        DetermineWinner(playerCounter.GetNPCCount(), enemyCounter.GetNPCCount());
                    }
                }
            }
        }
    }

    void DetermineWinner(int playerCount, int enemyCount)
    {
        gameActive = true;
        gamePaused = true;
        Time.timeScale = 0f;

        if (playerCount > enemyCount)
        {
            if(winPanel) winPanel.SetActive(true);
        }
        else
        {
            if(losePanel) losePanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}