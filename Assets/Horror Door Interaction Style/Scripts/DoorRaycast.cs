using UnityEngine;
using UnityEngine.UI;

public class DoorRaycast : MonoBehaviour
{ 
    [SerializeField] private int rayLength = 3;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private string exludeLayerName; 
     
    
    [HideInInspector] public Door raycasted_obj;

    public Image crosshair = null;
    [HideInInspector] public bool contacting;
    private bool once;

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(exludeLayerName) | layerMaskInteract.value;



        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            if (hit.collider.CompareTag("DoorHinge"))
            {
                if (!once)
                {
                    raycasted_obj = hit.collider.gameObject.GetComponent<Door>();
                    CrosshairChange(true);
                }

                contacting = true;
                once = true;



                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    raycasted_obj.GetComponent<Rigidbody>().isKinematic = false;
                    raycasted_obj.openedDoor = true;
                    raycasted_obj.openedDoor1 = true;
                    raycasted_obj.openedDoor2 = true;
                    raycasted_obj.alreadyOpened = true;
                }
            }
        }

        else
        {
            if (contacting)
            {
                CrosshairChange(false);
                once = false;
            }
        }
    }

    void CrosshairChange(bool on)
    {
        if (on && !once)
        {
            crosshair.color = Color.red; 
        }
        else
        { 
            crosshair.color = Color.white;
            
            raycasted_obj.halt = false;
            contacting = false;
        }
    }
}