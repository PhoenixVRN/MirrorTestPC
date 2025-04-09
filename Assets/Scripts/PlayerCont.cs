using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCont : NetworkBehaviour
{
    public Camera playerCamera;
    public float speed = 7f;
    public float blinkSpeed = 100f;
    public float blinkTime = 0.1f;
    private bool _blinked;
    
    [SerializeField, Header("Время неуязвимости"), Space] private float _timeInvulnerability;
    [HideInInspector] public bool invulnerability;
    
    [SyncVar][SerializeField]
    private Color _color;
    [SyncVar][SerializeField]
    private Color _colorInst;
    
    private float _blinkOut;
    private int _score;

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!isLocalPlayer) playerCamera.gameObject.SetActive(false);
        _score = 0;
    }

   
    void Update()
    {
        if (isLocalPlayer) GetInput();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
            if (!other.gameObject.GetComponent<PlayerCont>().invulnerability && _blinked)
            {
                CmdHit(other.gameObject);
                _score++;
                if (_score == 3)
                {
                    //TODO иммя выводим победителя и перезапуск сцены через 5 сек
 //                   NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
                }
            }
    }
    
    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _blinked = true;
            _blinkOut = blinkTime;
            StartCoroutine(Blink());
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Restart");
            NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += transform.forward * speed * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition += -transform.forward * speed * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition += -transform.right * speed * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition += transform.right * speed * Time.deltaTime;
        }
    }
    
    [Command]
    public void CmdHit(GameObject collor)
    {
        if(!isServer) return;
        invulnerability = true;
        Invul(collor);
//        StartCoroutine(Invul());
    }
   
    [ClientRpc]
 //   IEnumerator Invul()
 public void Invul(GameObject obj)
    {
        obj.GetComponent<Renderer>().material.color = Color.black;
//        yield return new WaitForSeconds(_timeInvulnerability);
//        GetComponent<Renderer>().material.color = _colorInst;
        invulnerability = false;
    }

    IEnumerator Blink()
    {
        while (_blinkOut > 0)
        {
 //           transform.localPosition += transform.forward * blinkSpeed * Time.deltaTime;
            rb.AddForce(transform.forward * blinkSpeed);
            _blinkOut -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

 //       yield return new WaitForSeconds(0.5f);
 //       _blinked = false;
    }
}
