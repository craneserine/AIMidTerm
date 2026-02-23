using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour
{

    [Header("Visual Settings")]
    public Material playerMaterial;
    
    private NavMeshAgent agent;
    
    [Header("Movement Settings")]
    public float speed = 10f;
    public float jumpPower = 2f; 
    public float rotationSpeed = 10f;
    
    private bool isGrounded = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        agent.updateRotation = false; 
        agent.acceleration = 100f;
        agent.stoppingDistance = 0f;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector3 direction = Vector3.zero;
        if (keyboard.wKey.isPressed) direction += Vector3.forward;
        if (keyboard.sKey.isPressed) direction += Vector3.back;
        if (keyboard.aKey.isPressed) direction += Vector3.left;
        if (keyboard.dKey.isPressed) direction += Vector3.right;

        if (direction.sqrMagnitude > 0.1f)
        {
            direction = direction.normalized;
            
            // Move the agent
            agent.Move(direction * speed * Time.deltaTime);

            // Smoothly rotate the player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            StartCoroutine(PlayerJumpRoutine());
        }
    }

    IEnumerator PlayerJumpRoutine()
    {
        isGrounded = false;
        float elapsed = 0;
        float duration = 0.5f; // Length of the jump
        float originalOffset = agent.baseOffset;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            
            // Using a Sine wave for a smooth up-and-down arc
            agent.baseOffset = originalOffset + Mathf.Sin(percent * Mathf.PI) * jumpPower;
            yield return null;
        }

        agent.baseOffset = originalOffset;
        isGrounded = true;
    }
}