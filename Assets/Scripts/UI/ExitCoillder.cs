using UnityEngine;

public class ExitCoillder : MonoBehaviour
{
    private bool playerInside = false;
    private float timer = 0f;
    public float timerDuration = 10f; // 10초

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            timer = timerDuration;
            UIManager.Instance.ShowExitTimerUI(); 
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            timer -= Time.deltaTime;
            UIManager.Instance.UpdateExitTimerUI(timer);

            if (timer <= 0)
            {
                UIManager.Instance.ShowExitCanvas();
                playerInside = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            timer = 0;
            UIManager.Instance.HideExitTimerUI();
        }
    }
}