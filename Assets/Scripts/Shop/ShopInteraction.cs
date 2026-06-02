using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class ShopInteraction : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private bool openAutomatically;
    [SerializeField] private bool closeWhenPlayerLeaves = true;

    [Header("Input")]
    [SerializeField] private Key interactKey = Key.E;

    private bool playerInRange;

    private void Reset()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;
    }

    private void Awake()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange || openAutomatically || shopPanel == null)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            shopPanel.SetActive(!shopPanel.activeSelf);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player _))
        {
            return;
        }

        playerInRange = true;
        if (openAutomatically && shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Player _))
        {
            return;
        }

        playerInRange = false;
        if (closeWhenPlayerLeaves && shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }
}
