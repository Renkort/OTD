using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Akkerman.FPS;

public class ObjectElevator : MonoBehaviour
{
    [SerializeField] private List<DestinationPoint> destinationPoints = new List<DestinationPoint>();

    public void MovePlayerLinear(int pointIndex)
    {
        Player.Instance.FreezePlayerActions(true, false);
        //Player.Instance.OnEnablePhysics(false);
        StartCoroutine(MoveToPoint(destinationPoints[pointIndex]));
    }

    IEnumerator MoveToPoint(DestinationPoint targetPoint)
    {
        GameObject player = Player.Instance.gameObject;
        float elapsedTime = 0f;
        Vector3 startPosition = player.transform.position;
        Quaternion startRotation = player.transform.rotation;

        while (elapsedTime < targetPoint.MovementDuration)
        {
            player.transform.position = Vector3.Lerp(startPosition, targetPoint.Transform.position, elapsedTime / targetPoint.MovementDuration);
            player.transform.rotation = Quaternion.Slerp(startRotation, targetPoint.Transform.rotation, elapsedTime / targetPoint.MovementDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        player.transform.position = targetPoint.Transform.position;
        player.transform.rotation = targetPoint.Transform.rotation;
        player.GetComponent<FPSPlayerController>().SetRotationAsGO();
    }


    [System.Serializable]
    public struct DestinationPoint
    {
        public Transform Transform;
        public float MovementDuration;
    }
}
