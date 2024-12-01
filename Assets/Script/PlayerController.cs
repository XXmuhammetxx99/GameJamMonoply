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
    public Animator playerAnimation;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] dialogue;
    private readonly Vector3[] directions = new Vector3[] {Vector3.left, Vector3.forward, Vector3.right, Vector3.back};

    private int _diceCounter = 0;
    private int _directionCounter = 0;
    private int _dialgoueCounter = 0;
    public bool _canThrow = false;
    public bool _isAnimPlaying = false;

    private Camera cam;

    private void Start()
    {
        cam = FindFirstObjectByType<Camera>();
        StartCoroutine(PlayConsecituveSounds());
    }
    IEnumerator PlayConsecituveSounds()
    {
        PlayClip();
        yield return new WaitForSeconds(5f);
        PlayClip();
        yield return new WaitForSeconds(2f);
        PlayClip();
        _canThrow = true;
        pressText.SetActive(true);
    }
    void Update()
    {
        //Look();
        transform.LookAt(cam.transform.position);
        //transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, 0);
        if (_canThrow && _diceCounter < riggedDice.Length)
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
            PlayClip();
        }
    }
    
    private void PlayClip()
    {
        if (_dialgoueCounter >= dialogue.Length) return;
        audioSource.clip = (dialogue[_dialgoueCounter]);
        audioSource.Play();
        _dialgoueCounter++;
    }
    /*
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
        if (Input.GetButtonDown("Jump"))
        {
            ThrowDice();
        }
    }

    private void ThrowDice()
    {
        //play die animation
        dieAnimator.SetInteger("die", riggedDice[_diceCounter]);
        dieAnimator.SetTrigger("shoot");
        Debug.Log("Dice thrown");
        _canThrow = false;
        pressText.SetActive(false);
        StartCoroutine(Step());
    }

    IEnumerator Step()
    {
        yield return new WaitForSeconds(1.5f);
        PlayClip();
        playerAnimation.SetTrigger("Move");
        for (int i = 0; i < riggedDice[_diceCounter]; i++)
        { 
            transform.DOMove(gameObject.transform.position + (directions[2] * stepDistance), stepTime);
            yield return new WaitForSeconds(stepDelay + stepTime);
        }
        _diceCounter++;
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
        PlayClip();
        yield return new WaitForSeconds(2);
        playerAnimation.SetInteger("Sequence", _diceCounter);
        playerAnimation.SetTrigger("Animation");
        yield return new WaitForSeconds(3f);
        PlayClip();
    }
}