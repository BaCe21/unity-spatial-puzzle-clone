using UnityEngine;
using Oculus.Interaction;

public class AutoOrientSnap : MonoBehaviour
{
    [Tooltip("Smart_Anchor")]
    public Transform attachPoint; 

    [Tooltip("Snap Interactor")]
    public SnapInteractor snapInteractor;

    void Update()
    {
        if (snapInteractor.State == InteractorState.Select)
            return;

        Vector3 currentEuler = transform.eulerAngles;

        float snappedX = Mathf.Round(currentEuler.x / 90f) * 90f;
        float snappedY = Mathf.Round(currentEuler.y / 90f) * 90f;
        float snappedZ = Mathf.Round(currentEuler.z / 90f) * 90f;


        Quaternion targetRotation = Quaternion.Euler(snappedX, snappedY, snappedZ);

        attachPoint.localRotation = Quaternion.Inverse(targetRotation) * transform.rotation;
        
        attachPoint.localRotation = Quaternion.Inverse(targetRotation);
    }
}