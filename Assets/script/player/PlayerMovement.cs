using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player Camera.")]
    [SerializeField] new Camera camera;
    [SerializeField] float FOV;
    [Tooltip("Physics Simulation.")]
    [SerializeField] Rigidbody rb;
    [Tooltip("Player Direction.")]
    [SerializeField] Transform orientation;
    [Tooltip("Stamina Bar.")]
    [SerializeField] RawImage staminaBar;

    [Header("Movement")]
    [Tooltip("Player speed.")]
    [SerializeField] float speed = 8f;
    float x;
    float z;
    [Header("Running")]
    [Tooltip("The time the player can keep running.")]
    [SerializeField] float runFOV;
    [SerializeField] float stamina = 100f;
    [SerializeField] float sDecrease = .125f;
    [SerializeField] float sIncrease = .50f;
    [SerializeField] float maxStamina = 100f;
    [Tooltip("The run key.")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    float runMulti;
    bool running;

    [Header("Jump")]
    [Tooltip("Jump key.")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [Tooltip("Strength of the jump.")]
    [SerializeField] float jumpForce = 5f;
    [Tooltip("If player is grounded, then true.")]
    [SerializeField] bool grounded;
    [Tooltip("Used in the sphere radius check.")]
    [SerializeField] float groundDistance = 1f;
    [Tooltip("The ground Layer.")]
    [SerializeField] LayerMask groundLayer;
    bool jump;
    [Header("Camera")]
    [Tooltip("Camera Velocity.")]
    [Range(0.1f, 9f)][SerializeField] float sensitivity = 4f;
	[Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
	[Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;
    [Header("Rendering")]
    [SerializeField] VolumeProfile vol;
    LensDistortion ld;

	Vector2 rotation = Vector2.zero;

    void Start()
    {
        //set cursor invisible and locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        vol.TryGet<LensDistortion>(out ld);
    }

    void Update()
    {
        //call functions
        MyInput();
        isGrounded();
        camMov();
        GUIManager();
        PostProcessingManager();
    }

    void PostProcessingManager()
    {
        if(Input.GetAxisRaw("LShift") >= 1 && running)
        {
            ld.intensity.value = Mathf.Lerp(ld.intensity.value, -.2f, .2f);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, runFOV, .2f);
        }
        else
        {
            ld.intensity.value = Mathf.Lerp(ld.intensity.value, 0, .2f);
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, FOV, .2f);
        }
    }
    void GUIManager()
    {
        if(stamina >= 99) staminaBar.rectTransform.sizeDelta = new Vector2(1980, Mathf.Lerp(staminaBar.rectTransform.sizeDelta.y, 0, .2f));
        else staminaBar.rectTransform.sizeDelta = new Vector2(1980, Mathf.Lerp(staminaBar.rectTransform.sizeDelta.y, 20, .2f));

        staminaBar.rectTransform.sizeDelta = new Vector2(1980 /100 * stamina, staminaBar.rectTransform.sizeDelta.y);
    }

    void isGrounded()
    {
        //checks if grounded by drawing a sphere check
        grounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundLayer);
    }

    void FixedUpdate()
    {
        //call functions
        Movement();
    }

    void camMov()
    {
        //set variables
		rotation.x += Input.GetAxis("Mouse X") * sensitivity;
		rotation.y += Input.GetAxis("Mouse Y") * sensitivity;
		rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
		var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        //change rotation
		orientation.localRotation = xQuat;
        camera.transform.localRotation = yQuat;
    }

    void Movement()
    {
        //move
        if(running) rb.velocity = (orientation.forward * (x * runMulti)) + (orientation.right * (z * runMulti)) + (rb.velocity.y * orientation.up);
        else rb.velocity = (orientation.forward * (x)) + (orientation.right * (z)) + (rb.velocity.y * orientation.up);
        rb.velocity.Normalize();
        if(jump) Jump();
    }

    void MyInput()
    {
        //get input
        x = speed * Input.GetAxis("Vertical");
        z = speed * Input.GetAxis("Horizontal");

        if(Input.GetKey(jumpKey) && grounded) jump=true;
        RunManager();
    }

    void RunManager()
    {
        runMulti = 2 * Input.GetAxis("LShift");
        running = Input.GetKey(sprintKey) && stamina > 0;
        if(running)
        {
            stamina -= sDecrease;
        }
        else if(!running && !Input.GetKey(sprintKey))
        {
            stamina += sIncrease;
        }
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }
    void Jump()
    {
        //set velocity
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        //prevent floating
        jump = false;
    }
}
