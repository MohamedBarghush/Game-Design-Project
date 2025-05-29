using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class NPCData
{
    public Transform npcTransform;
    public GameObject npcCanvas;
    public TMP_InputField npcInputField; // World-space input field
}

public class InGameUI : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObject;
    public PlayerInput playerInput;             // Reference to PlayerInput component
    public InputHandler inputHandler;
    public MonoBehaviour cameraController;      // Your camera control script
    public Image promptImage;                   // UI Image for interaction prompt

    [Header("NPC Settings")]
    public List<NPCData> npcEntries = new List<NPCData>();
    public float showDistance = 2f;

    private bool isPromptVisible = false;
    private NPCData currentNPC = null;
    [HideInInspector] public bool isTalkingToNPC = false;

    void Awake()
    {
        if (playerObject == null || inputHandler == null || promptImage == null)
        {
            Debug.LogError("Essential references are missing!");
        }

        if (playerInput == null && playerObject != null)
        {
            playerInput = playerObject.GetComponent<PlayerInput>();
        }

        promptImage.gameObject.SetActive(false);

        foreach (var entry in npcEntries)
        {
            if (entry.npcCanvas != null)
                entry.npcCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if (playerObject == null) return;

        if (isTalkingToNPC)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                HideNPCCanvas();

            return;
        }

        currentNPC = GetClosestNPC();
        
        if (currentNPC != null && currentNPC.npcTransform.gameObject.activeInHierarchy)
        {
            if (!isPromptVisible)
            {
                promptImage.gameObject.SetActive(true);
                isPromptVisible = true;
            }

            if (inputHandler.interactInput)
            {
                inputHandler.interactInput = false;
                ShowNPCCanvas(currentNPC);
            }
        }
        else
        {
            if (isPromptVisible)
            {
                promptImage.gameObject.SetActive(false);
                isPromptVisible = false;
            }
        }
    }

    private NPCData GetClosestNPC()
    {
        if (playerObject == null) return null;
        NPCData closestNPC = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPos = playerObject.transform.position;

        foreach (var npc in npcEntries)
        {
            if (npc.npcTransform == null) continue;

            float distance = Vector3.Distance(playerPos, npc.npcTransform.position);
            if (distance <= showDistance && distance < closestDistance)
            {
                closestDistance = distance;
                closestNPC = npc;
            }
        }

        return closestNPC;
    }

    private void ShowNPCCanvas(NPCData npc)
    {
        promptImage.gameObject.SetActive(false);
        isPromptVisible = false;

        if (playerInput != null) playerInput.enabled = false;
        inputHandler.enabled = false;
        isTalkingToNPC = true;

        foreach (var entry in npcEntries)
            entry.npcCanvas.SetActive(entry == npc);

        StartCoroutine(FocusTMPInputField(npc));
    }

    private IEnumerator FocusTMPInputField(NPCData npc)
    {
        yield return null;

        if (npc.npcInputField != null)
        {
            npc.npcInputField.Select();
            npc.npcInputField.ActivateInputField();
        }
    }

    private void HideNPCCanvas()
    {
        foreach (var entry in npcEntries)
            entry.npcCanvas.SetActive(false);

        promptImage.gameObject.SetActive(false);
        isPromptVisible = false;

        if (playerInput != null) playerInput.enabled = true;
        inputHandler.enabled = true;
        isTalkingToNPC = false;
    }
}
