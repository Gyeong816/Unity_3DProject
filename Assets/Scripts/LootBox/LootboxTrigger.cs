using UnityEngine;

public class LootboxTrigger : MonoBehaviour
{
    private Lootbox lootbox;

    private void Awake()
    {
        lootbox = GetComponent<Lootbox>();
        if (lootbox == null)
        {
            Debug.LogWarning($"{name}에 Lootbox 컴포넌트가 없습니다!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && lootbox != null)
        {
            UIManager.Instance.ShowLootPrompt(transform, lootbox);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.HideLootPrompt();
        }
    }
}