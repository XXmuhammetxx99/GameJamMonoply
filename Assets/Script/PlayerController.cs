using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int[] riggedDice;
    [SerializeField] private float stepTime;
    [SerializeField] private float stepDelay;
    [SerializeField] private float stepDistance;
    [SerializeField] private GameObject pressText;
    [SerializeField] private Animator dieAnimator;
    [SerializeField] private Animator playerAnimation;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private readonly AudioClip[] dialogue;
    private readonly Vector3[] directions = new Vector3[] {Vector3.left, Vector3.forward, Vector3.right, Vector3.back};

    private int _dicdCounter = 0;
    private int _directionCounter = 0;
    private int _dialgoueCounter = 0;
    private bool _canThrow = true;
    private bool _isAnimPlaying = false;

    private Camera cam;

    private void Start()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    void Update()
    {
        //Look();
        transform.LookAt(cam.transform.position);
        //transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, 0);
        if (_canThrow && _dicdCounter < riggedDice.Length)
        {
            TakePlayerInput();
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Corner"))
        {
            _directionCounter++;
            if (_directionCounter > 3)
            {
                _directionCounter = 0;
            }
        }
        else if (collision.CompareTag("Dialogue"))
        {
            audioSource.clip = (dialogue[_dialgoueCounter]);
            audioSource.Play();
            _dialgoueCounter++;
        }
    }/*
    private void Look()
    {
        switch (_isAnimPlaying)
        {
            case true:
                transform.LookAt(cam.transform.position);
                transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, 0);
                break;
            default:
                transform.eulerAngles = new Vector3(90f, 0, 0);
                break;
        }
    }*/

    private void TakePlayerInput()
    { 
        pressText.SetActive(true);
        Debug.Log("can throw");
        if (Input.GetButtonDown("Jump"))
        {
            ThrowDice();
        }
    }

    private void ThrowDice()
    {
        //play die animation
        dieAnimator.SetInteger("die", riggedDice[_dicdCounter]);
        dieAnimator.SetTrigger("shoot");
        Debug.Log("Dice thrown");
        _canThrow = false;
        pressText.SetActive(false);
        StartCoroutine(Step());
    }

    IEnumerator Step()
    {
        yield return new WaitForSeconds(1.5f);
        playerAnimation.SetTrigger("Move");
        for (int i = 0; i < riggedDice[_dicdCounter]; i++)
        { 
            transform.DOMove(gameObject.transform.position + (directions[2] * stepDistance), stepTime);
            yield return new WaitForSeconds(stepDelay + stepTime);
        }
        _dicdCounter++;
        //play sad animation and update ui
        playerAnimation.SetTrigger("Stop");
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        //move camera
        //cam.transform.DOMove(gameObject.transform.position + new Vector3(0, 0, -2.5f), 1);
        //cam.transform.DORotate(Vector3.zero, 1);
        _isAnimPlaying = true;
        yield return new WaitForSeconds(1);
        //gameObject.GetComponent<Animator>().SetInteger("Track", _dicdCounter);
        //gameObject.GetComponent<Animator>().SetTrigger("Play");
        StartCoroutine(EndAnimation());
    }

    IEnumerator EndAnimation()
    {
        //move camera back
        //cam.transform.DOMove(new Vector3(0, 12, 0), 1);
        //cam.transform.DORotate(new Vector3(90, 0, 0), 1);
        yield return new WaitForSeconds(1);
        _canThrow = true;
        _isAnimPlaying = false;
    }
}