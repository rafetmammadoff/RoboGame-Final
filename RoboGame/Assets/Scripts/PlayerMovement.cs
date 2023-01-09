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
    [SerializeField] Transform rayTransformBack;
    public RaycastHit hit;
    public bool inrope=false;
    public GameObject rope;
    [SerializeField] ParticleSystem particle2;
    [SerializeField] ParticleSystem particle1;
    public float customMagnitude;
    [SerializeField] float speed;
    float movement;
    public bool isMoveable = true;
    bool isRight;
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
        if (isRight && movement<0)
        {
            isMoveable = true;
            MovementSpeed = 3;
        }
        if (!isRight && movement>0)
        {
            isMoveable = true;
            MovementSpeed = 3;
        }
        if (movement>0)
        {
            isRight = true;
        }
        else if (movement < 0)
        {
            isRight = false;
        }
        #region RopeClimb
        if (inrope)
        {
            MovementSpeed = 0;
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
        movement = Input.GetAxis("Horizontal");
        //transform.position += new Vector3(movement, 0,0) * Time.deltaTime * MovementSpeed;
        #endregion
        //if (movement>0)
        //{
        //    movement = 0;
        //}
        #region Jump
        if (Input.GetKeyDown(KeyCode.Space) && onGround == true && !transform.GetComponent<HoldControl>().isPicked)
        {
            onGround = false;
            EventHolder.Instance.PlayerJumpStart(gameObject);
            //StartCoroutine(WaitJump());
            onGround = false;
            Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
        }
        #endregion

        #region Rotation & Run-Idle-Animation

        direction = new Vector3(movement, 0, 0);
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
                float targetAngle = Mathf.Atan2(direction.x, 0) * Mathf.Rad2Deg;
                float turnAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle+90, ref TurnSmoothVelocity, TurnSmoothTurnTime);
                transform.rotation = Quaternion.Euler(0, turnAngle, 0);

            }
            if (transform.GetComponent<HoldControl>().isPicked)
            {
                MovementSpeed = 3f;
            }
            if (isMoveable)
            {
                Rigidbody.MovePosition(transform.position += (direction * MovementSpeed * Time.deltaTime));
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
            if (!HoldControl.Instance.isPicked)
            {
                MovementSpeed = 3f;
            }
        }




        //Debug.DrawRay(rayTransformBack.position, rayTransformBack.right * 0.3f, Color.red);
        //if (Physics.Raycast(rayTransformBack.position, rayTransformBack.right, out hit, 0.3f))
        //{
        //    if (hit.transform.CompareTag("box"))
        //    {
        //        Debug.Log(hit.transform.name);
        //        MovementSpeed = 0;
        //        customMagnitude = 0;
        //        EventHolder.Instance.PlayerRunToIdle(gameObject);
        //    }


        //}
        //else
        //{
        //    if (!HoldControl.Instance.isPicked)
        //    {
        //        MovementSpeed = 3f;
        //    }
        //}
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
    //public IEnumerator WaitJump()
    //{
    //    yield return new WaitForSeconds(.5f);
    //    onGround = false;
    //    Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
    //}
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Teleport(other));
        if (!GetComponent<HoldControl>().isPicked && other.transform.CompareTag("rope") && !inrope)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            inrope = true;
            anim.SetBool("rope", true);
            anim.SetBool("ropeMove", false);
            MovementSpeed = 0;
        }


        if (other.CompareTag("fallCube"))
        {
            transform.GetChild(3).parent = null;
            EventHolder.Instance.PlayerHoldToIdle(gameObject);
           HoldControl.Instance._PickedItem.AddComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            HoldControl.Instance._PickedItem.transform.parent = null;
            HoldControl.Instance.isPicked = false;
            Destroy(other.gameObject);
        }
    }
    #region Teleport
    public IEnumerator Teleport(Collider other)
    {
        if (other.CompareTag("p1"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p2").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {
                transform.position = new Vector3(teleportTransform.position.x-2, teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });
        }

        if (other.CompareTag("p2"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p1").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {

                transform.position = new Vector3(teleportTransform.position.x-2, teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });

        }






        if (other.CompareTag("p3"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p4").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {
                transform.position = new Vector3(teleportTransform.position.x + 2, teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });
        }

        if (other.CompareTag("p4"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p3").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {

                transform.position = new Vector3(teleportTransform.position.x - 2 , teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });

        }









        if (other.CompareTag("p5"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p6").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {
                transform.position = new Vector3(teleportTransform.position.x + 2, teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });
        }

        if (other.CompareTag("p6"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p5").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {

                transform.position = new Vector3(teleportTransform.position.x - 2, teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });

        }




        if (other.CompareTag("p7"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p8").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {
                transform.position = new Vector3(teleportTransform.position.x + 2, teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });
        }

        if (other.CompareTag("p8"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p7").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {

                transform.position = new Vector3(teleportTransform.position.x - 2, teleportTransform.position.y, teleportTransform.position.z);
                transform.DOScale(1, 0.2f);
            });

        }






        
        if (other.CompareTag("p9"))
        {
            Transform teleportTransform = GameObject.FindGameObjectWithTag("p8").transform;
            transform.DOScale(0, 0.1f).OnComplete(() =>
            {

                transform.position = new Vector3(teleportTransform.position.x + 2, teleportTransform.position.y, teleportTransform.position.z);
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
            inrope = false;
            GetComponent<Rigidbody>().isKinematic = false;
            Rigidbody.AddForce(transform.up *3f, ForceMode.Impulse);
            anim.SetBool("ropeMove", false);
            anim.SetBool("rope", false);
            MovementSpeed = 3;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        

        if (other.transform.CompareTag("Ground"))
        {
            inrope = false;
        }

        if (other.transform.CompareTag("tele"))
        {
            tele.Instance.teleAnim.SetTrigger("active");
            tele.Instance.isActive = false;
            anim.SetTrigger("dead");
        }
        if (other.transform.CompareTag("blender"))
        {
            anim.SetTrigger("dead");
        }

        if (other.transform.CompareTag("box")  && HoldControl.Instance.isPicked)
        {
            Debug.Log("===========");
            isMoveable = false;
            MovementSpeed = 0;
        }
        if (other.transform.CompareTag("Ground") || other.transform.CompareTag("box"))
        {
            onGround = true;
        }
    }
    private void OnCollisionStay(Collision other)
    {
      

        
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}