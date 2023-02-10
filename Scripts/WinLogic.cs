using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLogic : MonoBehaviour
{
    public Spawner TheSpawner1, TheSpawner2, TheSpawner3, TheSpawner4, TheSpawner5;
    public Player ThePlayer;
    public bool HasWon = false;
    public GameObject YouWinCanvas;
    private void LateUpdate()
    {
        if(HasWon == false)
        {
            if (TheSpawner1.AmountSpawned >= TheSpawner1.AmountToSpawn && TheSpawner2.AmountSpawned >= TheSpawner2.AmountToSpawn
            && TheSpawner3.AmountSpawned >= TheSpawner3.AmountToSpawn && TheSpawner4.AmountSpawned >= TheSpawner4.AmountToSpawn
            && TheSpawner5.AmountSpawned >= TheSpawner5.AmountToSpawn)
            {
                Debug.Log("Ammmmm");
                if (ThePlayer.bestTarget == null || !ThePlayer.bestTarget.CompareTag("Enemy"))
                {
                    Debug.Log("Ammmmm2");
                    HasWon = true;
                    StartCoroutine(TimeOfWin());
                }
            }
        }
        
    }

    private IEnumerator TimeOfWin()
    {
        yield return new WaitForSeconds(5);
        if (ThePlayer.bestTarget == null || !ThePlayer.bestTarget.CompareTag("Enemy"))
        {
            Win();
        }
        else { HasWon = false; }
       
    }
    private void Win()
    {
        Debug.Log("HasWonnnn");
        StartCoroutine(TimeToEnd());
    }
    private IEnumerator TimeToEnd()
    {
        YouWinCanvas.SetActive(true);
        yield return new WaitForSeconds(5);
        Retry();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
