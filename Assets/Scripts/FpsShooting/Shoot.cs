using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public float fireRate;
    public float damage;
    public float range;
    public bool isFullAuto;
    public Vector2 fireError;

    [SerializeField] bool debugMode;


    AudioSource audioShoot;
    float nextShoot;

    [SerializeField] Camera fpsCamera;

    // Start is called before the first frame update
    void Awake()
    {
        audioShoot = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextShoot)
        {
            if (isFullAuto)
            {
                if (Input.GetButton("Fire1"))
                {
                    nextShoot = Time.time + 1 / fireRate;
                    shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    nextShoot = Time.time + 1 / fireRate;
                    shoot();
                }
            }
        }
    }

    void shoot()
    {
        playShootSound();

        Vector3 shootingDirection = fpsCamera.transform.forward + MovFireError();

        startShooting(fpsCamera.transform.position, shootingDirection, damage);


    }


    void playShootSound()
    {
        if (audioShoot != null)
        {
            audioShoot.Play();
        }
    }

    Vector3 MovFireError()
    {
        //
        if (fpsCamera.velocity == Vector3.zero)
        {
            return Vector3.zero;
        }
        else
        {
            return new Vector3(Random.Range(-fireError.x, fireError.x), Random.Range(-fireError.y, fireError.y), 0);
        }

    }

    void startShooting(Vector3 shootingPoint, Vector3 angle, float thisDamage)
    {
        RaycastHit thisHit;

        if (debugMode) Debug.DrawRay(shootingPoint, angle * 10, new Color(255, 0, 0, 255), 10f, false);

        if (Physics.Raycast(shootingPoint, angle, out thisHit))
        {
            Target hittedTarget = thisHit.transform.GetComponent<Target>();
            if (hittedTarget != null)
            {
                hittedTarget.targetHitted(thisDamage);
            }

            Wallbang wallBangCheck = thisHit.transform.GetComponent<Wallbang>();
            if (wallBangCheck != null && wallBangCheck.isWallbengable)
            {
                thisDamage *= wallBangCheck.wallMultiplier;
                startShooting(thisHit.point, angle, thisDamage);
            }

            if (debugMode) Debug.DrawRay(thisHit.point, thisHit.normal, new Color(0, 255, 0, 255), 10f, false);
        }
    }

}
