using Oculus.Interaction;
using UnityEngine;

public class AutoOrientSnap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attachPoint;
    [SerializeField] private SnapInteractor snapInteractor;

    private void Update()
    {
        if (attachPoint == null || snapInteractor == null)
            return;

        if (snapInteractor.State == InteractorState.Select)
            return;

        Vector3 currentEuler = transform.eulerAngles;

        float snappedX = Mathf.Round(currentEuler.x / 90f) * 90f;
        float snappedY = Mathf.Round(currentEuler.y / 90f) * 90f;
        float snappedZ = Mathf.Round(currentEuler.z / 90f) * 90f;

        Quaternion snappedRotation = Quaternion.Euler(
            snappedX,
            snappedY,
            snappedZ
        );

        attachPoint.localRotation = Quaternion.Inverse(snappedRotation);
    }
}