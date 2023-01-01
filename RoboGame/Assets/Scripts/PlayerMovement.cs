using DG.Tweening;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using static ToonyColorsPro.ShaderGenerator.Enums;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerMovement : MonoBehaviour
{
    [Header("Global")]
    public static PlayerMovement Instance;
    [Header("Move And Jump")]
    public float JumpForce = 5f;
    public float MovementSpeed;
    public Rigidbody Rigidbody;
    public float Fallmultiplier = 2.0f;
    public bool onGround = true;
    public Animator anim;
    public Vector3 direction;
    float TurnSmoothVelocity;
    float TurnSmoothTurnTime = 0.1f;
    [SerializeField] Transform rayTransform;
    public RaycastHit hit;
    public bool inrope=false;
    public GameObject rope;
    [SerializeField] ParticleSystem particle2;
    [SerializeField] ParticleSystem particle1;
    public float customMagnitude;
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
        #region RopeClimb
        if (inrope)
        {

            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.up * 3f * Time.deltaTime;
                anim.SetBool("ropeMove", true);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                anim.SetBool("ropeMove", false);
            }
        }
        #endregion

        #region Movement
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(0, 0, -movement) * Time.deltaTime * MovementSpeed;
        #endregion

        #region Jump
        if (Input.GetKeyDown(KeyCode.Space) && onGround == true && !transform.GetComponent<HoldControl>().isPicked)
        {
            onGround = false;
            EventHolder.Instance.PlayerJumpStart(gameObject);
            StartCoroutine(WaitJump());
        }
        #endregion

        #region Rotation & Run-Idle-Animation

        direction = new Vector3(0, 0, -movement);
        customMagnitude = direction.magnitude;
        if (customMagnitude > 0.01f)
        {
            if (!HoldControl.Instance.isPicked)
            {
                EventHolder.Instance.PlayerMoveStart(gameObject);
            }
            else
            {

                EventHolder.Instance.PlayerHoldMoveStart(gameObject);
            }

            if (!transform.GetComponent<HoldControl>().isPicked && !inrope)
            {
                float targetAngle = Mathf.Atan2(direction.z, 0) * Mathf.Rad2Deg;
                float turnAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, TurnSmoothTurnTime);
                transform.rotation = Quaternion.Euler(0, turnAngle, 0);

            }
            if (transform.GetComponent<HoldControl>().isPicked)
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
        #endregion

        #region CheckRaycast
        Debug.DrawRay(rayTransform.position, -rayTransform.right * 0.3f, Color.red);
        if (Physics.Raycast(rayTransform.position, -rayTransform.right, out hit, 0.3f))
        {
            if (hit.transform.CompareTag("box"))
            {
                Debug.Log(hit.transform.name);
                MovementSpeed = 0;
                customMagnitude = 0;
                EventHolder.Instance.PlayerRunToIdle(gameObject);
            }


        }
        else
        {
            MovementSpeed = 5;
        }
        #endregion
    }
    private void FixedUpdate()
    {
        if (Rigidbody.velocity.y < 0)
        {
            Rigidbody.velocity += Vector3.up * Physics.gravity.y * Fallmultiplier * Time.deltaTime;
        }
    }
    #region WaitJump
    public IEnumerator WaitJump()
    {
        yield return new WaitForSeconds(.5f);
        onGround = false;
        Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
    }
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Teleport(other));
    }
    #region Teleport
    public IEnumerator Teleport(Collider other)
    {
        if (other.CompareTag("p1"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p2").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {
                transform.position = new Vector3(teleportTransform.position.x, teleportTransform.position.y-2, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });
        }

        if (other.CompareTag("p2"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p1").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {

                transform.position = new Vector3(teleportTransform.position.x, teleportTransform.position.y, teleportTransform.position.z + 1);
                transform.DOScale(1, 0.2f);
            });

        }
        yield return new WaitForSeconds(1);
    }
    #endregion
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ropeEnd"))
        {
            GetComponent<Rigidbody>().isKinematic = false;
            Rigidbody.AddForce(transform.up *3f, ForceMode.Impulse);
            anim.SetBool("ropeMove", false);
            anim.SetBool("rope", false);
            MovementSpeed = 5;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (!GetComponent<HoldControl>().isPicked && other.transform.CompareTag("rope") && !inrope)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            inrope = true;
            anim.SetBool("rope", true);
            MovementSpeed = 0;
        }

       
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.transform.CompareTag("Ground") || other.transform.CompareTag("box"))
        {
            onGround = true;
            inrope = false;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}