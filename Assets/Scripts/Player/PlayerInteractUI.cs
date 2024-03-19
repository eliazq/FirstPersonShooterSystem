using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerInteractUI : MonoBehaviour {

    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactTextMeshProUGUI;
    [SerializeField] private WeaponHandling weaponHandling;

    private void Update() {
        if (playerInteract.GetInteractableObject() != null) {
            Show(playerInteract.GetInteractableObject());

            // If interactable object is the same weapon as the one player is holding, dont show Interact text
            if (playerInteract.GetInteractableObject().GetTransform().TryGetComponent(out Weapon weapon)){
                if (weapon == weaponHandling.Weapon)
                    Hide();
            }
        }
        else
        {
            Hide();
        }
    }

    private void Show(IInteractable interactable) {
        containerGameObject.SetActive(true);
        interactTextMeshProUGUI.text = interactable.GetInteractText();
    }

    private void Hide() {
        containerGameObject.SetActive(false);
    }

}