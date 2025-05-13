using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Automatically assigns all active colliders from a target parent object (including children)
/// to the XRGrabInteractable component attached to this GameObject.
/// Useful when enabling/disabling objects at runtime and needing to refresh grab colliders.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
public class AutoAssignChildColliders : MonoBehaviour
{
    [Tooltip("Parent object that contains all the objects with colliders to assign")]
    public Transform collidersRoot;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        RefreshColliders(); // Assign colliders on start
    }

    /// <summary>
    /// Clears and re-assigns all currently active colliders under the specified root.
    /// Should be called whenever objects are enabled/disabled at runtime.
    /// </summary>
    public void RefreshColliders()
    {
        if (collidersRoot == null || grabInteractable == null)
        {
            Debug.LogWarning("Missing reference to colliders root or XRGrabInteractable.");
            return;
        }

        // Clear previously assigned colliders
        grabInteractable.colliders.Clear();

        // Get all colliders under the root, including inactive ones
        Collider[] allColliders = collidersRoot.GetComponentsInChildren<Collider>(true);

        foreach (var col in allColliders)
        {
            // Only use colliders from active GameObjects
            if (col.gameObject.activeInHierarchy)
            {
                grabInteractable.colliders.Add(col);
            }
        }

        Debug.Log($"[ColliderRefresh] Assigned {grabInteractable.colliders.Count} active colliders.");

        // Temporarily disable and re-enable the XRGrabInteractable to force update
        grabInteractable.enabled = false;
        grabInteractable.enabled = true;
    }
}
