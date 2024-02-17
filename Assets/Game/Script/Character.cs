using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    private PlayerInput _playerInput;
    private Animator _Ani;

    //Enemy
    private NavMeshAgent _navMeshAgent;
    private Transform targetPlayer;

    public float moveSpeed = 5f;
    public float gravity = -9.8f;
    public bool isPlayer;

    private float _verticalVelocity;
    private Vector3 _movementVelocity;

    //State Machine
    public enum CharacterState
    {
        Normal,
        Attacking,
        Dead,
        BeingHit,
        Slide,
        Spawn
    }
    public CharacterState curState;

    //Player Slides
    private float attackStartTime;
    public float attackSlideDuration = 0.4f;
    public float attacckSlideSpeed = 0.06f;

    //Health
    private Health _health;

    //Coin
    public int coin = 0;

    //DamageCaster
    private DamageCaster _damageCaster;

    //Material animation
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    //Drop Item
    public GameObject dropItem;

    //Hit Impact
    private Vector3 impactOnCharacter;

    //invincible
    private bool isInvincible;
    public float invincibleDuration = 2f;

    //Combo
    private float attackAniamtionDuration;

    //Dash
    public float slideSpeed = 9f;

    //Spawn
    public float spawnDuration=2f;
    private float currentSpawnTime;

    private void Awake()
    {
        _Ani = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();

        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        if (!isPlayer) 
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            targetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = moveSpeed;
            SwitchToState(CharacterState.Spawn);
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayer)
        {
            if (_playerInput.spaceButtonDown && _cc.isGrounded)
            {
                _Ani.StopPlayback();
                SwitchToState(CharacterState.Slide);
                return;
            }
        }
    }

    private void CalculatePlayerMovement()
    {
        if (_playerInput.mouseButtonDown && _cc.isGrounded)
        {
            SwitchToState(CharacterState.Attacking);
            return;
        }
        /*
        else if (_playerInput.spaceButtonDown && _cc.isGrounded)
        {
            _Ani.StopPlayback();
            SwitchToState(CharacterState.Slide);
            return;
        }
        */
        _movementVelocity.Set(_playerInput.horizontalInput, 0f, _playerInput.verticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;

        _Ani.SetFloat("Speed", _movementVelocity.magnitude);

        _movementVelocity *= moveSpeed * Time.deltaTime;
        if (_movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }
        _Ani.SetBool("Fall", !_cc.isGrounded);
    }

    private void CalculateEnemyMovement()
    {
        if (Vector3.Distance(transform.position, targetPlayer.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(targetPlayer.position);
            _Ani.SetFloat("Speed", 0.2f);
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
            _Ani.SetFloat("Speed", 0f);
            SwitchToState(CharacterState.Attacking);
        }
    }

    private void FixedUpdate()
    {
        switch (curState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                {
                    CalculatePlayerMovement();
                }
                else
                {
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if (isPlayer)
                {
                    if (Time.time < attackStartTime + attackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / attackSlideDuration;
                        _movementVelocity = Vector3.Lerp(transform.forward * attacckSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if (_playerInput.mouseButtonDown && _cc.isGrounded)
                    {
                        string currentClipName = _Ani.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        attackAniamtionDuration = _Ani.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if (currentClipName != "LittleAdventurerAndie_ATTACK_03" && attackAniamtionDuration > 0.5f && attackAniamtionDuration < 0.7f)
                        {
                            _playerInput.mouseButtonDown = false;
                            SwitchToState(CharacterState.Attacking);
                            //CalculatePlayerMovement();
                        }
                    }
                    _playerInput.mouseButtonDown = false;
                }
                break;
            case CharacterState.Dead:
                break;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                _movementVelocity = transform.forward * slideSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawn:
                currentSpawnTime -= Time.deltaTime;
                if (currentSpawnTime <= 0)
                {
                    SwitchToState(CharacterState.Normal); 
                }
                break;
        }

        if (impactOnCharacter.magnitude > 0.2f)
        {
            _movementVelocity = impactOnCharacter * Time.deltaTime;
        }
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);

        if (isPlayer)
        {
            if (_cc.isGrounded == false)
            {
                _verticalVelocity = gravity;
            }
            else
            {
                _verticalVelocity = gravity * 0.3f;
            }
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;
            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        }
        else
        {
            if (curState != CharacterState.Normal)
            {
                _cc.Move(_movementVelocity);
                _movementVelocity = Vector3.zero;
            }
        }
    }

    public void SwitchToState(CharacterState newState)
    {
        if (isPlayer)
        {
            _playerInput.ClearCache();
        }
        //Exit State
        switch (curState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (_damageCaster != null)
                {
                    DisableDamageCaster();
                }
                if (isPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.Dead:
                break;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                isInvincible = false;
                break;
        }

        //Switch State
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (!isPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(targetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }
                _Ani.SetTrigger("Attack");
                if (isPlayer)
                {
                    attackStartTime = Time.time;
                    RotateToCursor();
                }
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _Ani.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                SkinnedMeshRenderer mesh = GetComponentInChildren<SkinnedMeshRenderer>();
                mesh.gameObject.layer = 0;
                break;
            case CharacterState.BeingHit:
                _Ani.SetTrigger("BeingHit");
                if (isPlayer)
                {
                    isInvincible = true;
                    StartCoroutine(DelayCancelInvincible());    
                }
                break;
            case CharacterState.Slide:
                _Ani.SetTrigger("Slide");
                break;
            case CharacterState.Spawn:
                isInvincible = true;
                currentSpawnTime = spawnDuration;
                StartCoroutine(MaterialAppear());   
                break;
        }

        curState = newState;

        //Debug.Log("Switch to" + newState);
    }

    public void AttackAnimationEnds()
    {
        SwitchToState(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchToState(CharacterState.Normal);
    }

    public void SlideAnimationEnds()
    {
        SwitchToState(CharacterState.Normal);
    }

    public void ApplyDame(int damage, Vector3 attackPos = new Vector3())
    {
        if (isInvincible)
        {
            return;
        }
        if (_health != null)
        {
            _health.ApplyDamage(damage);
        }

        if (!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackPos);
        }

        StartCoroutine(MaterialBlink());

        if (isPlayer && _health.currentHealth > 0)
        {
            SwitchToState(CharacterState.BeingHit);
            AddImpact(attackPos, 10f);
        }
        else if (!isPlayer)
        {
            AddImpact(attackPos, 2.5f);
        }
    }

    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void AddImpact(Vector3 attackPos, float force)
    {
        Vector3 impactDir = transform.position - attackPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    }

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2f);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0f;
        float dissolveHeight_Start = 20f;
        float dissolveHeight_target = -10f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_Start, dissolveHeight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        DropItem();
    }

    public void DropItem()
    {
        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.value);
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(item.value);
                break;
        }
    }

    public void AddHealth(int value)
    {
        _health.AddHealth(value);
        GetComponent<PlayerVFXManager>().PlayHeal();
    }

    public void AddCoin(int value)
    {
        coin += value;
    }

    public void RotateToPlayer()
    {
        if (curState != CharacterState.Dead)
        {
            transform.LookAt(targetPlayer, Vector3.up);
        }
    }

    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = spawnDuration;
        float currentDissolveTime = 0f;
        float dissolveHeight_Start = -10f;
        float dissolveHeight_target = 20f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_Start, dissolveHeight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, 1000, 1<<LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cursorPos, 1);
        }
    }

    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitResult;

        if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hitResult.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
    }    
}
