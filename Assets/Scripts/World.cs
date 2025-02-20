using UnityEngine;
using UnityEngine.UI; // Required for UI interactions

/// <summary>
/// Handles global game behaviors, including underwater gravity simulation 
/// and UI event handling such as quitting the game.
/// </summary>
public class World : MonoBehaviour
{
    // Reference to the player's Rigidbody (assumed to be a submarine)
    public Rigidbody subRigidbody;

    // Simulated underwater gravity (negative buoyancy)
    [SerializeField] private float gravityForce = -2.0f;

    // Drag settings to simulate water resistance
    [SerializeField] private float waterDrag = 3.0f;
    [SerializeField] private float angularDrag = 1.5f;

    // Quit button reference
    public Button quitButton;

    /// <summary>
    /// Initializes UI event listeners and ensures the Rigidbody is set up.
    /// </summary>
    private void Start()
    {
        // Ensure the submarine has a Rigidbody
        if (subRigidbody == null)
        {
            Debug.LogError("WorldManager: No Rigidbody assigned for the Sub!");
            return;
        }

        // Set Rigidbody properties to simulate water resistance
        subRigidbody.useGravity = false; // Disable Unity's default gravity
        subRigidbody.linearDamping = waterDrag;
        subRigidbody.angularDamping = angularDrag;

        // Bind the Quit button click event
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogWarning("WorldManager: Quit Button not assigned in Inspector!");
        }
    }

    /// <summary>
    /// Applies underwater gravity to the submarine in each physics update.
    /// </summary>
    private void FixedUpdate()
    {
        if (subRigidbody != null)
        {
            // Apply downward force to simulate sinking
            subRigidbody.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Handles quitting the application when the Quit button is clicked.
    /// </summary>
    private void QuitGame()
    {
        Debug.Log("Quit Button Pressed! Exiting game...");
        Application.Quit();

        // Exit play mode in the Unity Editor (for testing)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
