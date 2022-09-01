using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isDoorRotateClockwise = true;

    [Range(0f, 360f)]
    public float maxAngleOpen = 115;
    enum DoorType : int
    {
        Nothing = 0,
        NonClosable = 1,
        Closable = 2,
        Autoclose = 3
    }

    [SerializeField] DoorType doorType;
    Rigidbody rb;
    [SerializeField] private DoorRaycast doorRaycast;
    private float originalY;
    [HideInInspector] public bool openedOutside;
    private bool changed;
    [HideInInspector] public bool collided;
    private HingeJoint hinge;
    [HideInInspector] public bool autoClose;

    [HideInInspector] public bool openedDoor, openedDoor1, openedDoor2, alreadyOpened, openedPassMinAngle;
    [HideInInspector] public bool haltIsNear, alreadyInside, isNear, halt, startAutoRotateToMax, startClosing, l;


    [HideInInspector] public bool doOnce;
    [HideInInspector] public bool doOnce2; 
    bool doOnce3; 
    [HideInInspector] public float initL;
    [HideInInspector] public bool once;
    [HideInInspector] public bool avoidFail;

    public float openForce = 0.2f; 
    public float nearForce1 = 0.6f;
    public float nearForce2 = 0.5f;
    float instantSpeed;
    float speed;
    public float distanceToDoorToApplyNearForce1 = 0.95f; 
    public float raycastLengthToDoorToApplyNearForce2 = 0.35f;
    [Header("For Non-Closable Door")]
    public float startToAutoRotateAngle;

    [Header("For Closable Door")]
    [SerializeField] bool canBeOpenedAgain;
    [SerializeField] bool rotateOtherwiseWhenClosed;

    [Header("For Autoclose Door")]
    [SerializeField] private DoorCloseTrigger triggerCloseDoor;

    [SerializeField] float speedAutoClose = 62.8f;
    int mtp = 1;

    float subY;

    [Header("Sound Effects (Optional)")]
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip creakingSound;
    [SerializeField] AudioClip closeSound;
    [SerializeField] AudioClip creakingAutoclose;
    [SerializeField] float speedToPlayCreaking = 35f;
    AudioSource doorAudioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        hinge = GetComponent<HingeJoint>();
        originalY = transform.eulerAngles.y;
        if ((int)doorType == 3)
        {
            triggerCloseDoor.door = this;
        }
        SwitchRotationDirection();
        hinge.axis = new Vector3(0, 1, 0);
        hinge.useLimits = true;
        if (doorRaycast == null)
            doorRaycast = Camera.main.GetComponent<DoorRaycast>();
        
        if (GetComponent<AudioSource>())
        {
            doorAudioSource = GetComponent<AudioSource>();
            doorAudioSource.spatialBlend = 1f;
        }
    }

    private void FixedUpdate()
    {
        speed = Mathf.Abs(rb.angularVelocity.y * 180 / (Mathf.PI));
        if(speed > speedToPlayCreaking)
        {
            if(creakingSound != null)
            {
                doorAudioSource.clip = creakingSound;
                doorAudioSource.Play();
            }
            
        }
        if (openedDoor && !changed)
        {
            AddOpenForce();
        }

        if (!halt && doorRaycast.contacting)
        {
            l = false;
            halt = true;
        }

        if (isDoorRotateClockwise)
        {
            if (originalY >= 180)
            {
                if (transform.eulerAngles.y is >= 0 and < 180)
                {
                    subY = transform.eulerAngles.y + 360;
                }
                else subY = transform.eulerAngles.y;
            }
            else
            {
                subY = transform.eulerAngles.y;
            }
        }
        else
        {
            if (originalY <= 180)
            {
                if (transform.eulerAngles.y is <= 360 and > 180)
                {
                    subY = transform.eulerAngles.y - 360;
                }
                else subY = transform.eulerAngles.y;
            }
            else subY = transform.eulerAngles.y;
        }

        switch ((int)doorType)
        {
            case 1:
                if (mtp * subY >= mtp * (originalY + mtp * startToAutoRotateAngle) && !l)
                {
                    instantSpeed = speed;
                    startAutoRotateToMax = true;
                    rb.isKinematic = true;
                    l = true;
                }

                if (startAutoRotateToMax)
                {
                    float yVelocity = 0f;
                    float smooth = 0.03f;
                    float maxAngle = 0;
                    if (isDoorRotateClockwise)
                    {
                        maxAngle = hinge.limits.max;
                    }
                    else
                    {
                        maxAngle = hinge.limits.min;
                    }
                    RotateTowards(maxAngle, ref yVelocity, smooth, instantSpeed);
                }

                break;
            case 2:
                if (alreadyOpened && mtp * subY >= mtp * (originalY + mtp * 10))
                {
                    openedPassMinAngle = true;
                }

                if (openedPassMinAngle)
                {
                    if (mtp * subY < mtp * (originalY + mtp * 8))
                    {
                        startClosing = true;
                        instantSpeed = speed;
                        rb.isKinematic = true;
                        openedPassMinAngle = false;
                        if (closeSound != null)
                        {
                            doorAudioSource.clip = closeSound;
                            doorAudioSource.Play();
                        }
                    }
                }


                if (startClosing)
                {
                    float yVelocity = 0f;
                    float smooth = 0.03f;
                    RotateTowards(originalY, ref yVelocity, smooth, instantSpeed);

                    
                    if (mtp * subY <= mtp * (originalY + mtp * 0.001f))
                    {
                        if (rotateOtherwiseWhenClosed)
                        {
                            isDoorRotateClockwise = !isDoorRotateClockwise;
                            SwitchRotationDirection();
                        }
                        startClosing = false;
                        if (canBeOpenedAgain)
                        {
                            tag = "DoorHinge";
                            ResetBools();
                        }
                        
                    }
                }
                break;
            case 3:
                if (autoClose)
                {
                    rb.isKinematic = true;
                    float yVelocity = 0f;
                    float smooth = 0.03f;

                    if (creakingAutoclose != null)
                    { 
                        if (!doOnce3)
                        {
                            var doorAddedAS = gameObject.AddComponent<AudioSource>();
                            doorAddedAS.spatialBlend = 1;
                            doorAddedAS.clip = creakingAutoclose;
                            doorAddedAS.Play();
                            doOnce3 = true;
                        }
                        
                    }

                    RotateTowards(originalY, ref yVelocity, smooth, speedAutoClose);
                    if (mtp * subY <= mtp * (originalY + mtp * 8f))
                    {
                        if (creakingAutoclose != null)
                        {
                            doorAudioSource.clip = closeSound;
                            doorAudioSource.Play();
                        }
                            
                    }

                    

                    if (mtp * subY <= mtp * (originalY + mtp * 0.001f))
                    {
                        autoClose = false;
                    }
                }
                break;
        }


        if (isNear && openedDoor1)
        {
            AddForceNear1();
        }
    }

    void SwitchRotationDirection()
    {
        JointLimits limits = new JointLimits();
        if (isDoorRotateClockwise)
        {
            limits.min = 0;
            limits.max = maxAngleOpen;
            mtp = 1;
        }
        else
        {
            limits.min = -maxAngleOpen;
            limits.max = 0;
            mtp = -1;
        }
        hinge.limits = limits;
    }

    void RotateTowards(float yTarget, ref float yVelocity, float smooth, float speed)
    {
        float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, yTarget, ref yVelocity, smooth, speed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, yAngle, transform.eulerAngles.z);
    }
    void ResetBools()
    {

        openedOutside = changed = collided = openedDoor = openedDoor1 = openedDoor2 = alreadyOpened = openedPassMinAngle
            = haltIsNear = isNear = halt = startAutoRotateToMax = startClosing = l = doOnce = doOnce2 = once = avoidFail = false;
    }

    void AddOpenForce()
    {
        var xzpair = FindXZMultiplier(gameObject.transform.eulerAngles.y);
        rb.AddForce(new Vector3(mtp * xzpair[0], 0, mtp * xzpair[1]) * openForce, ForceMode.Impulse);
        openedDoor = false;
        collided = true;
        gameObject.tag = "Opened";
        doorRaycast.crosshair.color = Color.white;
        if (!alreadyInside)
        {
            openedOutside = true;
        }
        changed = true;
        if(openSound != null)
        {
            doorAudioSource.clip = openSound;
            doorAudioSource.Play();
        }
    }

    public void AddForceNear1()
    {
        var xzpair = FindXZMultiplier(gameObject.transform.eulerAngles.y);
        rb.AddForce(new Vector3(mtp * xzpair[0], 0, mtp * xzpair[1]) * nearForce1, ForceMode.Impulse);
        openedDoor1 = false;
        //if (creakingSound1 != null)
        //{
        //    doorAudioSource.clip = creakingSound1;
        //    doorAudioSource.Play();
        //}
    }

    public void AddForceNear2()
    {
        var xzpair = FindXZMultiplier(gameObject.transform.eulerAngles.y);
        rb.AddForce(new Vector3(mtp * xzpair[0], 0, mtp * xzpair[1]) * nearForce2, ForceMode.Impulse);
        openedDoor2 = false;
        //if (creakingSound2 != null)
        //{
        //    doorAudioSource.clip = creakingSound2;
        //    doorAudioSource.Play();
        //}
    }

    float[] FindXZMultiplier(float doorAngle)
    {
        float[] xzpair = { 0, 0 };
        if (doorAngle < 180)
        {
            if (doorAngle < 90)
                xzpair[1] = 1 / (1 + Mathf.Tan(doorAngle * Mathf.PI / 180));
            else if (doorAngle >= 90)
            {
                xzpair[1] = -(1 / (1 + Mathf.Tan((180 - doorAngle) * Mathf.PI / 180)));
            }
            xzpair[0] = 1 - Mathf.Abs(xzpair[1]);
        }
        else if (doorAngle >= 180)
        {
            if (doorAngle < 270)
            {
                xzpair[1] = -(1 / (1 + Mathf.Tan((180 - (360 - doorAngle)) * Mathf.PI / 180)));
            }
            else if (doorAngle >= 270)
            {
                xzpair[1] = 1 / (1 + Mathf.Tan(((360 - doorAngle)) * Mathf.PI / 180));
            }
            xzpair[0] = -1 + Mathf.Abs(xzpair[1]);
        }
        return xzpair;
    }

}
