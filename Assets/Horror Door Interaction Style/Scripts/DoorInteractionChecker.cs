using UnityEngine;

public class DoorInteractionChecker : MonoBehaviour
{
    private DoorRaycast doorRaycast;
    SphereCollider sphereCollider;

    private void Start()
    {
        doorRaycast = GetComponent<DoorRaycast>();
        if ((sphereCollider = GetComponent<SphereCollider>()) == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            
        }
    }

    private void Update()
    {
        if (doorRaycast.raycasted_obj)
        {
            var currentDoor = doorRaycast.raycasted_obj;
            if (currentDoor && currentDoor.collided && currentDoor.alreadyInside)
            {
                currentDoor.haltIsNear = true;
            }
        }
        if(doorRaycast.raycasted_obj)
            sphereCollider.radius = doorRaycast.raycasted_obj.distanceToDoorToApplyNearForce1;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (doorRaycast.raycasted_obj)
        {
            var currentDoor = doorRaycast.raycasted_obj;
            if (other.tag == "DoorTrigger")
            {
                currentDoor.alreadyInside = true;
            }
            if (other.tag == "DoorTrigger" && doorRaycast.raycasted_obj.collided && !currentDoor.haltIsNear)
            {
                currentDoor.isNear = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (doorRaycast.raycasted_obj)
        {
            var currentDoor = doorRaycast.raycasted_obj;
            if (other.tag == "DoorTrigger")
            {
                currentDoor.isNear = false;
                currentDoor.alreadyInside = false;
            }
        }
    }
}
