using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterControl : MonoBehaviour
{
    public GameObject Camera;
    public float Acceleration;
    public float Speed;
    public float RunSpeedCoefficient;
    public float RunStaminaDepleat;
    public float Glide;
    float Grav;
    public float Gravity;
    public float JumpStrength;
    public float JumpStaminaDepleat;
    public float MaxStamina;
    public float StaminaDelay;
    public float StaminaRegen;
    RaycastHit info;
    float Stamina;
    public GameObject StaminaBar;
    Vector3 MoveDirection;
    Vector3 LastDirection;
    bool OnGround;
    bool PrevOnGround;
    float LastActivity;
    public float HorisontalSensetivity;
    public float VerticalSensetivity;
    public float MinCameraAngle;
    public float MaxCameraAngle;
    public bool Controlability;
    bool PausedStaminaDelay;
    float PauseStart;
    float VerticalRotation;
    public float StaminaAppear;
    public float StaminaDisappear;
    public GameObject FPSInterface;
    public GameObject PauseInterface;
    //Shooting
    public float Delay;
    public float AnotherPauseStart;
    public float LastShot;
    public float BurstDelay;
    public GameObject Scope;
    public GameObject Ammo;
    public GameObject E;

    private void Awake()
    {
        Controlability = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Stamina = MaxStamina;
        Ammo = GameObject.Find("Ammo");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Controlability)
            {
                AnotherPauseStart = Time.realtimeSinceStartup;
                if (Time.realtimeSinceStartup < LastActivity + StaminaDelay)
                {
                    PauseStart = Time.realtimeSinceStartup;
                    PausedStaminaDelay = true;
                }
            }
            else
            {
                Delay = Time.realtimeSinceStartup - AnotherPauseStart;
                if (PausedStaminaDelay)
                {
                    LastActivity += Time.realtimeSinceStartup - PauseStart;                    
                    PausedStaminaDelay = false;
                }
            }
            Controlability = !Controlability;
            FPSInterface.SetActive(Controlability);
            PauseInterface.SetActive(!Controlability);
        }
        if (Controlability)
        {
            {
                MoveDirection = Vector3.zero + Input.GetAxis("Horizontal") * gameObject.transform.right + Input.GetAxis("Vertical") * gameObject.transform.forward;
                MoveDirection = Vector3.ClampMagnitude(MoveDirection, 1);
                if (OnGround)
                {
                    if (Input.GetKey(KeyCode.LeftShift) && Stamina > 0 && MoveDirection.magnitude > 0)
                    {
                        MoveDirection *= RunSpeedCoefficient;
                        MoveDirection = Vector3.Lerp(LastDirection, MoveDirection, Acceleration);
                        gameObject.GetComponent<CharacterController>().Move(MoveDirection * Time.deltaTime * Speed);
                        LastActivity = Time.realtimeSinceStartup;
                        Stamina = Mathf.Clamp(Stamina - Time.deltaTime * RunStaminaDepleat, 0, MaxStamina);
                    }
                    else
                    {
                        MoveDirection = Vector3.Lerp(LastDirection, MoveDirection, Acceleration);
                        gameObject.GetComponent<CharacterController>().Move(MoveDirection * Time.deltaTime * Speed);
                    }
                }
                else
                {
                    MoveDirection = Vector3.Lerp(LastDirection, MoveDirection, Acceleration * Glide);
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        MoveDirection *= RunSpeedCoefficient;
                    }
                    gameObject.GetComponent<CharacterController>().Move(MoveDirection * Time.deltaTime * Speed);
                }
                LastDirection = MoveDirection;
                Grav += Gravity * Time.deltaTime;
                if (Physics.SphereCast(gameObject.transform.position, gameObject.GetComponent<CharacterController>().radius, Vector3.down, out info, gameObject.GetComponent<CharacterController>().height / 2 - gameObject.GetComponent<CharacterController>().radius + gameObject.GetComponent<CharacterController>().skinWidth + 0.05f))
                {
                    OnGround = true;
                    Grav = 0;
                }
                if (Input.GetKeyDown(KeyCode.Space) && Stamina >= JumpStaminaDepleat && OnGround)
                {
                    OnGround = false;
                    Grav -= JumpStrength;
                    Stamina -= JumpStaminaDepleat;
                    LastActivity = Time.realtimeSinceStartup;
                }
                if (OnGround && !PrevOnGround)
                {
                    LastActivity = Time.realtimeSinceStartup;
                }
                gameObject.GetComponent<CharacterController>().Move(Vector3.down * Grav);
                if (Time.realtimeSinceStartup >= LastActivity + StaminaDelay)
                {
                    Stamina = Mathf.Clamp(Stamina + StaminaRegen * Time.deltaTime, 0, MaxStamina);
                }
                StaminaBar.transform.localScale = new Vector3(Stamina / MaxStamina * 2, 1, 1);
                PrevOnGround = OnGround;
            }//Movement
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    E.SetActive(!E.activeInHierarchy);
                }
            }
            {
                gameObject.transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * HorisontalSensetivity);
                VerticalRotation = -Input.GetAxis("Mouse Y") * VerticalSensetivity;
                if (Camera.transform.rotation.eulerAngles.x <= -MinCameraAngle && Camera.transform.rotation.eulerAngles.x - VerticalRotation >= -MinCameraAngle)
                {
                    VerticalRotation = MinCameraAngle + Camera.transform.rotation.eulerAngles.x;
                }
                VerticalRotation += Camera.transform.rotation.eulerAngles.x;
                if (VerticalRotation > 180)
                {
                    if (VerticalRotation < 360 - MaxCameraAngle)
                        VerticalRotation = 360 - MaxCameraAngle;
                }
                else
                {
                    if (VerticalRotation > -MinCameraAngle)
                        VerticalRotation = -MinCameraAngle;
                }
                if (VerticalRotation < 0)
                    VerticalRotation += 360;
                if (VerticalRotation > 360)
                    VerticalRotation -= 360;
                Camera.transform.localRotation = Quaternion.Euler(new Vector3(VerticalRotation, 0, 0));
            }//Turning
            {
                if (Stamina == MaxStamina)
                {
                    StaminaBar.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Clamp01(StaminaBar.GetComponent<CanvasRenderer>().GetAlpha() - StaminaDisappear * Time.deltaTime));
                    StaminaBar.transform.parent.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Clamp01(StaminaBar.transform.parent.GetComponent<CanvasRenderer>().GetAlpha() - StaminaDisappear * Time.deltaTime));
                }
                else
                {
                    StaminaBar.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Clamp01(StaminaBar.GetComponent<CanvasRenderer>().GetAlpha() + StaminaAppear * Time.deltaTime));
                    StaminaBar.transform.parent.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Clamp01(StaminaBar.transform.parent.GetComponent<CanvasRenderer>().GetAlpha() + StaminaAppear * Time.deltaTime));
                }
            }//StaminaBarDecay
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (gameObject.GetComponentInChildren<Gun>() != null)
                    {
                        switch (gameObject.GetComponentInChildren<Gun>().CurShootType)
                        {
                            case "auto":
                                if (LastShot + gameObject.GetComponentInChildren<Gun>().FireRate + Delay < Time.realtimeSinceStartup)
                                {
                                    gameObject.GetComponentInChildren<Gun>().Shoot();
                                    LastShot = Time.realtimeSinceStartup;
                                    gameObject.GetComponentInChildren<Gun>().OverHeat++;
                                    Delay = 0;
                                }
                                break;
                            case "semi":
                                if (LastShot + gameObject.GetComponentInChildren<Gun>().FireRate + Delay < Time.realtimeSinceStartup && Input.GetKeyDown(KeyCode.Mouse0))
                                {
                                    gameObject.GetComponentInChildren<Gun>().Shoot();
                                    LastShot = Time.realtimeSinceStartup;
                                    gameObject.GetComponentInChildren<Gun>().OverHeat++;
                                    GetComponent<CharacterControl>().Delay = 0;
                                }
                                break;
                        }
                    }
                }
            }//Shooting
            {
                if(gameObject.GetComponentInChildren<Gun>() != null)
                {
                    RaycastHit R;
                    Physics.Raycast(Camera.transform.position, Camera.transform.forward, out R, 500f);
                    if(R.transform != null)
                    {
                        gameObject.GetComponentInChildren<Gun>().gameObject.transform.LookAt(R.point);
                    }
                    else
                    {
                        gameObject.GetComponentInChildren<Gun>().gameObject.transform.LookAt(Camera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(UnityEngine.Camera.main.pixelWidth/2, UnityEngine.Camera.main.pixelHeight/2,500f)));
                    }
                }
            }//Aiming
            {
                if(gameObject.GetComponentInChildren<Gun>() == null)
                {
                    Ammo.SetActive(false);
                }
                else
                {
                    Ammo.SetActive(true);
                    Ammo.GetComponent<TMP_Text>().text = gameObject.GetComponentInChildren<Gun>().MagCap.ToString();
                }
            }
        }
    }
}
