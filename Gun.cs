using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject ReloadBar;
    public int MaxMagCap;
    public int MagCap;
    public string[] ShootTypes;
    public string CurShootType;
    public float FireRate;
    public List<string> Modules;
    public GameObject[] ModuleSlots;
    public int FlatDMG;
    public int Penetration;
    public float ScopeMod;
    public float Recoil;
    public int Accuracy;
    public float OverHeat = 0;
    public float DrawTime;
    public float ReloadTime;
    public RaycastHit Info;
    public GameObject Sphere;
    public float HeatMod;
    public float HeatCooling;
    public Vector3 HipHold;
    public Vector3 ScopeHold;
    public float ScopeSpeed;
    public float ScopeFallof;
    public bool Reloading;
    public float ReloadStart;

    public void Shoot()
    {
        if (MagCap != 0)
        {
            System.Random rand = new();
            if (Vector3.Distance(gameObject.transform.localPosition, ScopeHold) <= ScopeFallof)
            {
                Physics.Raycast(gameObject.transform.position, gameObject.transform.forward + gameObject.transform.right * (ScopeMod * rand.Next(-Accuracy, Accuracy) / 5000) + gameObject.transform.right * rand.Next(-100, 100) * Recoil * OverHeat / 10000 + gameObject.transform.up * rand.Next(100, 200) * Recoil * OverHeat / 10000 + gameObject.transform.up * (ScopeMod * rand.Next(-Accuracy, Accuracy) / 5000), out Info, 200f);
            }
            else
            {
                Physics.Raycast(gameObject.transform.position, gameObject.transform.forward + gameObject.transform.right * rand.Next(-Accuracy, Accuracy) / 5000 + gameObject.transform.right * rand.Next(-100, 100) * Recoil * OverHeat / 10000 + gameObject.transform.up * rand.Next(100, 200) * Recoil * OverHeat / 10000 + gameObject.transform.up * rand.Next(-Accuracy, Accuracy) / 5000, out Info, 200f);
            }
            //Debug.Log("Trickshot! " + Info.distance + " meters away!");
            gameObject.GetComponent<AudioSource>().Play();
            GameObject HitMark = Instantiate(Sphere, Info.point, new Quaternion(), Info.transform);
            HitMark.transform.LookAt(Info.point + Info.normal);
            if (HitMark.transform.parent != null)
            {
                HitMark.transform.localScale = new Vector3(0.1f / HitMark.transform.parent.lossyScale.x, 0.1f / HitMark.transform.parent.lossyScale.y, 0.1f / HitMark.transform.parent.lossyScale.z);
            }
            if (HitMark.transform.parent != null && HitMark.GetComponentInParent<Enemy>() != null)
            {
                float Damage = FlatDMG * ((100f - (HitMark.GetComponentInParent<Enemy>().Armour - Penetration)) / (100f + (HitMark.GetComponentInParent<Enemy>().Armour - Penetration)));
                Debug.Log("Нанесено " + Damage + " урона!");
                HitMark.GetComponentInParent<Enemy>().Health -= Damage;
            }
            if (Info.collider != null && Info.collider.gameObject.GetComponent<Ecusplousion>() != null)
            {
                if (rand.Next(0, Info.collider.gameObject.GetComponent<Ecusplousion>().Probability) == 0)
                {
                    Info.collider.gameObject.GetComponent<Ecusplousion>().Kaboom = true;
                }
            }
            if (!Modules.Contains("Muzzle"))
            {
                ModuleSlots[0].GetComponentInChildren<ParticleSystem>().Play();
            }
            MagCap--;
        }
    }

    public void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Mouse4) && Modules.Contains("Flashlight"))
        {
            ModuleSlots[1].SetActive(!ModuleSlots[1].activeInHierarchy);
        }
        if (Input.GetKey(KeyCode.Mouse1) && Modules.Contains("Scope")) 
        {
            gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, ScopeHold, 0.1f * ScopeSpeed * Time.deltaTime);
        }
        else
        {
            gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, HipHold, 0.1f * ScopeSpeed * Time.deltaTime);
        }
        if (OverHeat > 0)
        {
            OverHeat -= HeatCooling * Time.deltaTime;
            OverHeat *= 1 - HeatMod;
        }
        if (OverHeat < 0)
        {
            OverHeat = 0;
        }
        if (!Reloading && MagCap == 0)
        {
            Reloading = true;
            ReloadStart = Time.realtimeSinceStartup;
            ReloadBar.SetActive(true);
        }
        if (Reloading && Time.realtimeSinceStartup - ReloadStart >= ReloadTime)
        {
            MagCap = MaxMagCap;
            Reloading = false;
            ReloadBar.SetActive(false);
        }
    }
    private void Start()
    {
        ReloadBar = GameObject.Find("RelBar");
        ReloadBar.SetActive(false);
    }
}


