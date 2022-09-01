using System.Collections; 
using UnityEngine;

public class DetectRaycast : MonoBehaviour
{
    [Header("Raycast Parameters")] 
    [SerializeField] private int rayLength = 3;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private string exludeLayerName;
    DoorRaycast doorRaycast;

    private void Awake()
    {
        doorRaycast = GetComponent<DoorRaycast>();  
    }
    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(exludeLayerName) | layerMaskInteract.value;



        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            if (hit.collider.CompareTag("DoorTrigger"))
            {
                if (!doorRaycast.raycasted_obj.doOnce)
                {
                    if (doorRaycast.raycasted_obj.haltIsNear && !doorRaycast.raycasted_obj.openedOutside)
                    {
                        if (!doorRaycast.raycasted_obj.once)
                        {
                            doorRaycast.raycasted_obj.initL = hit.distance;
                            doorRaycast.raycasted_obj.once = true;
                        }
                        StartCoroutine(avoidFail());
                        if (hit.distance < doorRaycast.raycasted_obj.initL && doorRaycast.raycasted_obj.avoidFail)
                        {
                            doorRaycast.raycasted_obj.AddForceNear1();
                            doorRaycast.raycasted_obj.doOnce = true;
                        }

                        IEnumerator avoidFail()
                        {
                            yield return new WaitForSeconds(0.4f);
                            doorRaycast.raycasted_obj.avoidFail = true;
                        }
                    } 
                }
                if (!doorRaycast.raycasted_obj.doOnce2)
                {
                    if (hit.distance < doorRaycast.raycasted_obj.raycastLengthToDoorToApplyNearForce2)
                    {
                        doorRaycast.raycasted_obj.AddForceNear2();
                        doorRaycast.raycasted_obj.doOnce2 = true;
                    }
                } 
            }
        }
    }

}

