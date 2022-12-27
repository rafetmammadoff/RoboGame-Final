using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    [Header("Move And Jump")]
    public float JumpForce = 5f;
    public float MovementSpeed = 5f;
    public Rigidbody Rigidbody;
    public float Fallmultiplier = 2.0f;
    private bool onGround = true;
    public Animator anim;
    public Vector3 direction;
    float TurnSmoothVelocity;
    float TurnSmoothTurnTime = 0.1f;
    [SerializeField] Transform rayTransform;
    public RaycastHit hit;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
        }
    }


    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //Hereket
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;

        //Lazerle yoxlanis
        
        Debug.DrawRay(rayTransform.position, -rayTransform.right * 0.3f, Color.red);
        if (Physics.Raycast(rayTransform.position, -rayTransform.right, out hit, 0.3f))
        {
            if (hit.transform.CompareTag("wall"))
            {
                Debug.Log(hit.transform.name);
                MovementSpeed = 0;
            }
            

        }
        else
        {
            MovementSpeed = 5;
        }

        //atlama

        if (Input.GetKeyDown(KeyCode.Space) == true && onGround == true && !transform.GetComponent<HoldControl>().isPicked)
        {
            EventHolder.Instance.PlayerJumpStart(gameObject);
            StartCoroutine(WaitJump());
        }
        
        direction = new Vector3(movement, 0, 0);

        //if (HoldControl.Instance.isPicked && direction.magnitude<0.01f)
        //{
        //    EventHolder.Instance.PlayerHoldIdleStart(gameObject);
        //}
        //if (!HoldControl.Instance.isPicked && direction.magnitude < 0.01f)
        //{
        //    EventHolder.Instance.PlayerIdleStart(gameObject);
        //}

        //rotate
        if (direction.magnitude > 0.01f)
        {
            if (!HoldControl.Instance.isPicked)
            {
                EventHolder.Instance.PlayerMoveStart(gameObject);
            }
            else
            {

               EventHolder.Instance.PlayerHoldMoveStart(gameObject);
            }
            
            if (!transform.GetComponent<HoldControl>().isPicked)
            {
                float targetAngle = Mathf.Atan2(direction.x, 0) * Mathf.Rad2Deg;
                float turnAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle + 90, ref TurnSmoothVelocity, TurnSmoothTurnTime);
                transform.rotation = Quaternion.Euler(0, turnAngle, 0);
                MovementSpeed = 5f;
            }
            else
            {
                
                MovementSpeed = 4f;
            }
            

        }
        else
        {
            if (!GetComponent<HoldControl>().isPicked)
            {
                EventHolder.Instance.PlayerRunToIdle(gameObject);
            }
            if (GetComponent<HoldControl>().isPicked)
            {
                EventHolder.Instance.OnPlayerHoldMoveToHoldIdle(gameObject);

            }
        }
    }

    public IEnumerator WaitJump()
    {
        yield return new WaitForSeconds(.5f);
        onGround = false;
        Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
    }




























    private void FixedUpdate()
    {
        if (Rigidbody.velocity.y < 0)
        {
            Rigidbody.velocity += Vector3.up * Physics.gravity.y * Fallmultiplier * Time.deltaTime;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        onGround = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        onGround = false;
    }

    
}