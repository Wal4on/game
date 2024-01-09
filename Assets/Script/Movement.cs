using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("HorizontalMovement")]
    [SerializeField] private float _speed;

    [SerializeField] private bool _faceRight;

    [Header("Jump")][SerializeField] private float _jumpPower;
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _whatIsGround;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private AnimationState _currentState;
    private string _animatorParameterName;

    private Rigidbody2D _rigidbody2D;
    private float _direction;
    private bool _jump;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animatorParameterName = _animator.GetParameter(0).name;
    }

    private void Update()
    {
        _direction = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
            _jump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundChecker.position, _groundCheckRadius);
    }

    private void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapCircle(_groundChecker.position, _groundCheckRadius, _whatIsGround);

        if (_jump && isGrounded)
            _rigidbody2D.AddForce(Vector2.up * _jumpPower);
        _jump = false;
        SetDirection();
        Move(_direction);
        PlayAnimation(AnimationState.Jump, !isGrounded && _rigidbody2D.velocity.y > 0);
        PlayAnimation(AnimationState.Run, _direction != 0);
    }

    private void PlayAnimation(AnimationState animationState, bool active)
    {
        if (animationState < _currentState)
            return;

        if (!active)
        {
            if (animationState == _currentState)
            {
                _animator.SetInteger(_animatorParameterName, (int)AnimationState.Idle);
                _currentState = AnimationState.Idle;
            }

            return;
        }

        _animator.SetInteger(_animatorParameterName, (int)animationState);
        _currentState = animationState;
    }
    private void Move(float direction)
    {
        _rigidbody2D.velocity = new Vector2(_speed * _direction, _rigidbody2D.velocity.y);
    }
    private void SetDirection()
    {
        if (_faceRight && _direction < 0)
            Flip();
        else if (!_faceRight && _direction > 0)
            Flip();
    }

    private void Flip()
    {
        _faceRight = !_faceRight;
        transform.Rotate(0, 180, 0);
    }

    private enum AnimationState
    {
        Idle = 0,
        Run = 1,
        Jump = 2
    }
}
