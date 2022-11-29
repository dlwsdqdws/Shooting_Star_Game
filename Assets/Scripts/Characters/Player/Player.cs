using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : Character
{
    [SerializeField] StatsBar_HDR statsBar_HDR;

    [SerializeField] bool regenerateHealth = true;

    [SerializeField] float healthRegenrateTime;

    [SerializeField, Range(0f, 1f)] float healthRegenratePercent;

    [Header("---- INPUT ----")]

    [SerializeField] PlayerInput input;

    [Header("---- MOVE ----")]

    [SerializeField] float moveSpeed = 10f;

    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;

    [SerializeField] float moveRotationAngle = 50f;

    [SerializeField] float paddingX = 0.8f;
    [SerializeField] float paddingY = 0.22f;

    [Header("---- FIRE ----")]

    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;

    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleBottom;

    [SerializeField] AudioData projectileLaunchSFX;

    [SerializeField, Range(0,2)] int weaponPower = 0;

    [SerializeField] float fireInterval = 0.2f;

    [Header("---- DODGE ----")]

    [SerializeField] AudioData dodgeSFX;

    [SerializeField, Range(0, 100)] int dodgeEnergyCost = 25;

    [SerializeField] float maxRoll = 720f;

    [SerializeField] float rollSpeed = 360f;

    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("---- OVERDRIVE ----")]

    [SerializeField] int overdriveDodgeFactor = 2;

    [SerializeField] float overdriveSpeedFactor = 1.2f;

    [SerializeField] float overdriveFireFactor = 1.2f;

    bool isDodging = false;

    bool isOverdriving = false;

    float currentRoll;

    float dodgeDuration;

    float t;

    Vector2 previosVelocity;

    Quaternion previosRotation;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    new Rigidbody2D rigidbody;

    new Collider2D collider;

    Coroutine moveCoroutine;

    Coroutine healthRegenerateCoroutine;

    WaitForSeconds waitForFireInterval;

    WaitForSeconds waitForOverdriveFireInterval;

    WaitForSeconds waitHealthRegenrateTime;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        dodgeDuration = maxRoll / rollSpeed;

        rigidbody.gravityScale = 0f;

        waitForFireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitHealthRegenrateTime = new WaitForSeconds(healthRegenrateTime);
    }

    protected override void OnEnable(){

        base.OnEnable();

        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverdrive += Overdrive;

        PlayerOverdrive.on += OverdriveOn;
        PlayerOverdrive.off += OverdriveOff;
    }

    void OnDisable(){
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverdrive -= Overdrive;

        PlayerOverdrive.on -= OverdriveOn;
        PlayerOverdrive.off -= OverdriveOff;
    }

    void Start()
    {
        statsBar_HDR.Initialize(health, maxHealth);

        input.EnableGameplayInput();

        // TakeDamage(50f);
    }

    // //just for test
    // public override void RestoreHealth(float value){
    //     base.RestoreHealth(value);

    //     Debug.Log("Regenerate health! Current Health : " + health + " \nTime: " + Time.time);
    // }   

    public override void TakeDamage(float damage){
        base.TakeDamage(damage);

        statsBar_HDR.UpdateStats(health, maxHealth);

        if(gameObject.activeSelf){
            if(regenerateHealth){
                if(healthRegenerateCoroutine != null){
                    StopCoroutine(healthRegenerateCoroutine);
                }

                healthRegenerateCoroutine = StartCoroutine(HealthRegenerateCoroutine(waitHealthRegenrateTime, healthRegenratePercent));
            }
        }
    }

    // void Update(){
    //     transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position);
    // }

    public override void RestoreHealth(float value){
        base.RestoreHealth(value);

        statsBar_HDR.UpdateStats(health, maxHealth);
    }

    public override void Die(){
        statsBar_HDR.UpdateStats(0f, maxHealth);

        base.Die();
    }


    #region MOVE
    void Move(Vector2 moveInput){
        // Vector2 moveAmount = moveInput * moveSpeed;

        // rigidbody.velocity = moveAmount;

        if(moveCoroutine != null){
            StopCoroutine(moveCoroutine);
        }

        Quaternion moveRotation = Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right);

        moveCoroutine = StartCoroutine(MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed, moveRotation));
        StartCoroutine(nameof(MovePositionLimitCoroutine));
    }

    void StopMove(){
        // rigidbody.velocity = Vector2.zero;

        if(moveCoroutine != null){
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
        StopCoroutine(nameof(MovePositionLimitCoroutine));
    }

    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation){
        //Linear interpolation
        t = 0f;

        previosVelocity = rigidbody.velocity;
        previosRotation = transform.rotation;

        while(t < 1f){

            //acceleration / deceleration
            t += Time.fixedDeltaTime / time;
            rigidbody.velocity = Vector2.Lerp(previosVelocity, moveVelocity, t);

            //rotate
            transform.rotation = Quaternion.Lerp(previosRotation, moveRotation, t);

            yield return waitForFixedUpdate;
        }
    }


    IEnumerator MovePositionLimitCoroutine(){
        //avoid update position in update function
        while(true){
            transform.position = Viewport.Instance.PlayerMoveablePosition(transform.position, paddingX, paddingY);

            yield return null;
        }
    }
    #endregion

    #region FIRE
    void Fire(){
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire(){
        StopCoroutine(nameof(FireCoroutine));
    }

    IEnumerator FireCoroutine(){

        // WaitForSeconds waitForFireInterval = new WaitForSeconds(fireInterval);
        while(true){
            //generate projectile

            // switch(weaponPower){
            //     case 0:
            //         Instantiate(projectile1, muzzleMiddle.position, Quaternion.identity);
            //         break;
            //     case 1:
            //         Instantiate(projectile1, muzzleTop.position, Quaternion.identity);
            //         Instantiate(projectile1, muzzleBottom.position, Quaternion.identity);
            //         break;
            //     case 2:
            //         Instantiate(projectile1, muzzleMiddle.position, Quaternion.identity);
            //         Instantiate(projectile2, muzzleTop.position, Quaternion.identity);
            //         Instantiate(projectile3, muzzleBottom.position, Quaternion.identity);
            //         break;
            //     default:
            //         break;
            // }

            //use pool system
            switch(weaponPower){
                case 0:
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(projectile1, muzzleTop.position);
                    PoolManager.Release(projectile1, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(projectile1, muzzleMiddle.position);
                    PoolManager.Release(projectile2, muzzleTop.position);
                    PoolManager.Release(projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }

            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);

            // yield return waitForFireInterval;

            // if(isOverdriving){
            //     yield return waitForOverdriveFireInterval;
            // }else{
            //     yield return waitForFireInterval;
            // }

            yield return isOverdriving? waitForOverdriveFireInterval : waitForFireInterval;
        }
    }
    #endregion

    #region DODGE
    void Dodge(){

        if(isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return ;

        //when dodge->
        StartCoroutine(nameof(DodgeCoroutine));
    }

    IEnumerator DodgeCoroutine(){
        isDodging = true;

        AudioManager.Instance.PlayRandomSFX(dodgeSFX);

        //cost energy
        PlayerEnergy.Instance.Use(dodgeEnergyCost);

        //make player invincible
        //set Trigger true -> avoid collision -> avoid hurt
        collider.isTrigger = true;

        //make player rotate along x-axis
        currentRoll = 0f;

        var scale = transform.localScale;

        while(currentRoll < maxRoll){
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

            //change player's scale
            // if(currentRoll < maxRoll / 2f){
            //     // * method 1
            //     // // scale -= (Tiem.deltaTime / dodgeDuration) * Vector3.one;
            //     // scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
            //     // scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
            //     // scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            //     // * method 2
            //     t1 += Time.deltaTime / dodgeDuration;
            //     transform.localScale = Vector3.Lerp(transform.localScale, dodgeScale, t1);
            // }else{
            //     // * method 1
            //     // // scale += (Tiem.deltaTime / dodgeDuration) * Vector3.one;
            //     // scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
            //     // scale.y = Mathf.Clamp(scale.y + Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
            //     // scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
            //     // * method 2
            //     t2 += Time.deltaTime / dodgeDuration;
            //     transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t1);
            // }

            // * method 1
            // transform.localScale = scale;

            // * method 3 -> much more smoothly
            transform.localScale = BezierCurve.QuadraticPoint(Vector3.one, Vector3.one, dodgeScale, currentRoll / maxRoll);

            yield return null;
        }

        //quit invincible state
        collider.isTrigger = false;
        isDodging = false;
    }
    #endregion


    #region OVERDRIVE
    void Overdrive(){
        //only works when full energy
        if(!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return ;

        PlayerOverdrive.on.Invoke();
    }

    void OverdriveOn(){
        isOverdriving = true;

        //when overdrive, dodge ability drop, speed and fire ability increse
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;

    }

    void OverdriveOff(){
        isOverdriving = false;

        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }
    #endregion
}
