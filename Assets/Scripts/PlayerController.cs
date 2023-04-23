using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float throwForce;
    public float playerMoveSpeed = 1;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform throwDirection;
    [SerializeField]
    private List<Rigidbody> bowlingBallPrefabs;
    [SerializeField]
    private FollowTarget followTarget;
    [SerializeField]
    private GameManager gameManager;

    public bool wasBallThrown;

    public GameObject throwButton;

    public GameObject ballPrefab1;
    public GameObject ballPrefab2;
    public GameObject ballPrefab3;
    private GameObject selectedBallPrefab;
    public Button button1;
    public Button button2;
    public Button button3;

    public void Start()
    {
#if UNITY_ANDROID
        throwButton.SetActive(true);
        button1.onClick.AddListener(SelectBall1);
        button2.onClick.AddListener(SelectBall2);
        button3.onClick.AddListener(SelectBall3);
        selectedBallPrefab = ballPrefab1;
#endif
    }

    public void StartAiming()
    {
        animator.SetBool("Aiming", true);
        wasBallThrown = false;
        followTarget.targetTransform = transform;
    }

    private void Update()
    {
        var input = GetAxisInput();
        transform.position += transform.right * (input * playerMoveSpeed * Time.deltaTime);

        //TryThrowBall();
    }

#if UNITY_STANDALONE
    private void TryThrowBall()
    {
        if (wasBallThrown || !Input.GetButtonDown("Fire1")) return;

        wasBallThrown = true;
        var selectedPrefab = bowlingBallPrefabs[Random.Range(0, bowlingBallPrefabs.Count)];

        var newBallRigidbody = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
        newBallRigidbody.AddForce(throwDirection.forward * throwForce, ForceMode.Impulse);
        // newBallRigidbody.velocity = throwDirection.forward * throwForce;

        followTarget.targetTransform = newBallRigidbody.transform;
        gameManager.BallThrown(newBallRigidbody.GetComponent<BowlingBall>());

        animator.SetBool("Aiming", false);
    }
#endif

    private float GetButtonInput()
    {
        Debug.Log("Moving Using Buttons");

        if (Input.GetButtonDown("Left"))
        {
            return -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            return 1;
        }

        return 0;
    }

    private float GetAxisInput()
    {
        return Input.GetAxis("Horizontal");
    }

#if UNITY_ANDROID || UNITY_IOS
    public void ThrowMobile()
    {
        if (wasBallThrown) return;

        wasBallThrown = true;
        //var selectedPrefab = bowlingBallPrefabs[Random.Range(0, bowlingBallPrefabs.Count)];

        var newBall = Instantiate(selectedBallPrefab, transform.position, Quaternion.identity);
        var newBallRigidBody = newBall.GetComponent<Rigidbody>();

        if (newBallRigidBody != null)
        {
            Vector3 throwDirection = transform.forward;
            newBallRigidBody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }

        //newBallRigidbody.AddForce(throwDirection.forward * throwForce, ForceMode.Impulse);
        //newBallRigidbody.velocity = throwDirection.forward * throwForce;

        followTarget.targetTransform = newBall.transform;
        gameManager.BallThrown(newBall.GetComponent<BowlingBall>());

        animator.SetBool("Aiming", false);
    }
#endif

    public void MainMenu(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void SelectBall1()
    {
        selectedBallPrefab = ballPrefab1;
    }
    public void SelectBall2()
    {
        selectedBallPrefab = ballPrefab2;
    }
    public void SelectBall3()
    {
        selectedBallPrefab = ballPrefab3;
    }
}

