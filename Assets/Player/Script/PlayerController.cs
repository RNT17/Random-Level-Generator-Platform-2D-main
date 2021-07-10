using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpMultiplier = 10f;

    private static Vector2 _playerInitialPosition;
    private Rigidbody2D _rigidbody2d;
    private Vector2 _moveForce;
    private bool _jump;
    private PlayerAnimController  _playerAnimController;

    private bool _isInitialPositionOk = false;

    void Awake() 
    {  
        _rigidbody2d = GetComponent<Rigidbody2D>(); 
        _playerAnimController = GetComponent<PlayerAnimController>();
        _playerAnimController.PlayIdle();
    }

    void Update()
    {
        if(!_isInitialPositionOk)
        {
            transform.position = _playerInitialPosition;
            _isInitialPositionOk = true;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        transform.eulerAngles = new Vector2(transform.rotation.x, transform.rotation.y);
        if (horizontal < 0)
        {
            transform.eulerAngles = new Vector2(transform.rotation.x, -180);
        } 

        _moveForce = new Vector2(horizontal * _moveSpeed, _rigidbody2d.velocity.y);

        if (Input.GetKeyDown(KeyCode.UpArrow) && IsGrounded())
        {
            _jump = true;
        }
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    bool IsGrounded() => true;

    void Move()
    {
        _rigidbody2d.velocity = _moveForce;
        _playerAnimController.PlayRun();
    }

    void Jump()
    {
        if (_jump)
        {
            _rigidbody2d.velocity = new Vector2(_rigidbody2d.velocity.x, _jumpMultiplier);
            _playerAnimController.PlayJump();
            _jump = false;
        }
    }

    public static void OnFirstRoomWasCreated(Vector2 pos)
    {
        Debug.Log("Player first room was created!");
        print(pos);
        _playerInitialPosition = pos;
    }

}
