using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private InteractWithItem interactWithItem;
    private UseItem useItem;
    private InventoryManager inventoryManager;

    private bool isPrimaryHeld = false;
    private float primaryHoldTime = 0f;
    private const float maxMultiplier = 5f; // Cap for damage multiplier

    [SerializeField] private Animator armAnimator; // Animator for the arm

    void Start()
    {
        interactWithItem = GetComponent<InteractWithItem>();
        useItem = GetComponent<UseItem>();
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryManager.ToggleInventory();
        }

        if (!inventoryManager.inventoryOn)
        {
            if (Input.GetKeyDown(KeyCode.F) && interactWithItem != null)
            {
                interactWithItem.TryInteractWithItem();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                inventoryManager.DropItem();
            }

            HandlePrimaryAttack();
            HandleOffhandItem();
        }
    }

    void HandlePrimaryAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPrimaryHeld = true;
            primaryHoldTime = 0f;
        }

        if (Input.GetMouseButton(0) && isPrimaryHeld)
        {
            primaryHoldTime += Time.deltaTime;
            float multiplier = Mathf.Min(1f + primaryHoldTime, maxMultiplier);
            Debug.Log($"Multiplier: {armAnimator.GetFloat("Multiplier")}");
            UpdateArmAnimation(multiplier);
        }

        if (Input.GetMouseButtonUp(0) && isPrimaryHeld)
        {
            float damageMultiplier = Mathf.Min(1f + primaryHoldTime * 4, maxMultiplier);
            useItem.TryUseItem(damageMultiplier);

            // Let the release animation play before resetting
            StartCoroutine(ResetArmAnimationWithDelay(0.2f)); // Adjust delay as needed
            isPrimaryHeld = false;
        }
    }

    void HandleOffhandItem()
    {
        if (Input.GetMouseButtonDown(1))
        {
            useItem.TryUseOffHandItem();
        }
    }

    void UpdateArmAnimation(float multiplier)
    {
        if (armAnimator != null)
        {
            float mult = multiplier / maxMultiplier;
            // to stop animation for resetting(resets at 1)
            if (mult > 0.99f) {
                mult = 0.99f;
            }
            armAnimator.SetFloat("Multiplier", mult);
        }
    }

    IEnumerator ResetArmAnimationWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetArmAnimation();
    }

    void ResetArmAnimation()
    {
        if (armAnimator != null)
        {
            armAnimator.SetFloat("Multiplier", 0f);
        }
    }
}
