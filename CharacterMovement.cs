using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

public class CharacterMovement : MonoBehaviour
{
    Animator anim;
    bool dead;

    public float health;
    public float startingHealth;

    public float damage;
    public float initialDamage;
    public float forceDamage;
    public float forceAmount;
    public float startingForceAmount;

    public bool saberThrow;
    public bool forceChoke;
    public GameObject forcePower;
    public AudioClip saberHitSound;

    public bool lightSide;

    bool hit;
    float hitTimer;
    public float hitTime;
    public GameObject audioSHit;

    public Text healthText;
    public Image healthBar;

    public Image forceBar;
    public Text forceText;

    int numberOfHits;
    bool canHit;

    float attackTimer;
    bool startAttackTimer;

    public GameObject lightSaberBlade;
    public AudioSource saberSwing;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        numberOfHits = 0;
        canHit = true;

        health = startingHealth;
        forceAmount = startingForceAmount;

        audioSHit = GameObject.FindGameObjectWithTag("Saber Hit");
    }

    // Update is called once per frame
    void Update()
    {
        //Comment from 1/8/25: THERE IS TOO MUCH STUFF IN THE UPDATE FUNCTION! Thank goodness I do not code like this anymore.
        if (forceAmount <= 0)
        {
            forceAmount = 0;
        }

        if (hit == true)
        {
            hitTimer += Time.deltaTime;
            attackTimer = 0.0f;
            startAttackTimer = false;
            numberOfHits = 0;
            anim.SetInteger("Attack", 0);

            if (hitTimer >= hitTime)
            {
                hitTimer = 0.0f;
                hit = false;
                anim.SetBool("Hit", false);
                canHit = true;
                attackTimer = 0.0f;
                startAttackTimer = false;
                numberOfHits = 0;
                anim.SetInteger("Attack", 0);
            }
        }

        if(health <= 0)
        {
            health = 0.0f;
            dead = true;
        }

        healthText.text = health.ToString();
        healthBar.fillAmount = health / startingHealth;

        forceText.text = forceAmount.ToString("f0");
        forceBar.fillAmount = forceAmount / startingForceAmount;

        if (forceAmount <= startingForceAmount && anim.GetBool("Special") == false)
        {
            forceAmount += Time.deltaTime * (startingForceAmount / 16);

            if(forceAmount >= startingForceAmount)
            {
                forceAmount = startingForceAmount;
            }
        }

        if(anim.GetBool("Special") == true)
        {
            forceAmount -= Time.deltaTime * (startingForceAmount / 2);
        }

        if(anim.GetBool("Force") == true)
        {
            forceAmount -= Time.deltaTime * (startingForceAmount / 2.5f);
        }

        if(dead == false)
        {
            Movements();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
        }

        if(dead == true)
        {
            lightSaberBlade.SetActive(false);
            anim.SetBool("Die", true);
            GetComponent<SimpleMouseRotator>().enabled = false;
        }

        if(startAttackTimer == true)
        {
            attackTimer += Time.deltaTime;

            if(attackTimer >= 4f)
            {
                anim.SetInteger("Attack", 0);
                canHit = true;
                numberOfHits = 0;
                lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
                saberSwing.Stop();
                attackTimer = 0.0f;
                startAttackTimer = false;
            }
        }

        //Quit Game. Added 1/8/25.
        if (Input.GetKey(KeyCode.Escape)) QuitGame();
    }

    public void QuitGame()
    {
       Application.Quit();
    }

    public void Movements()
    {
        // Movement Walking Forward, Back, Left, and Right.

        anim.SetFloat("MovementX", Input.GetAxis("Horizontal"), 1f, Time.deltaTime * 20f);

        if (anim.GetFloat("MovementY") >= 0.1 && Input.GetButton("Sprint") == true)
        {
            anim.SetFloat("MovementY", Input.GetAxis("Vertical") * 2f, 1f, Time.deltaTime * 20f);
        }

        else
        {
            anim.SetFloat("MovementY", Input.GetAxis("Vertical"), 1f, Time.deltaTime * 20f);
        }


        if (Input.GetButtonDown("Jump") && numberOfHits == 0 && hit == false)
        {
            anim.SetBool("Jumping", true);
            canHit = false;
            numberOfHits = 0;
            if(saberThrow == true)
            {
                lightSaberBlade.transform.parent.GetComponent<Animator>().SetBool("Throw", false);
                lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            }
        }

        else
        {
            anim.SetBool("Jumping", false);
            canHit = true;
        }

        if (Input.GetButtonDown("Attack") && hit == false && anim.GetBool("Special") == false)
        {
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || numberOfHits >= 1)
            {
                ComboStart();
                if(startAttackTimer == false)
                {
                    startAttackTimer = true;
                }
               
            }
        }

        if(numberOfHits >= 3)
        {
            numberOfHits = 3;
        }

      //  if(numberOfHits == 0)
       // {
        //    if(anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 1") || anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 2") || anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 3"))
        //    {
       //         canHit = false;
       //         numberOfHits = 0;
       //     }
       // }

        if(Input.GetButtonDown("Special Attack") && saberThrow == true && numberOfHits == 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && forceAmount >= 250 && hit == false)
        {
            anim.SetBool("Special", true);
            lightSaberBlade.transform.parent.GetComponent<Animator>().SetBool("Throw", true);
            lightSaberBlade.GetComponent<MeshCollider>().enabled = true;
            gameObject.GetComponent<SimpleMouseRotator>().enabled = false;
            damage = damage * 5f;

        }

        if((Input.GetButtonUp("Special Attack") && saberThrow == true ) || forceAmount < 0 || hit == true)
        {
            anim.SetBool("Special", false);
            lightSaberBlade.transform.parent.GetComponent<Animator>().SetBool("Throw", false);
            lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            gameObject.GetComponent<SimpleMouseRotator>().enabled = true;
            damage = initialDamage;
        }

        if (Input.GetButtonDown("Force Attack") && forceChoke == true && numberOfHits == 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && forceAmount >= 450 && hit == false)
        {
            anim.SetBool("Force", true);
            forcePower.SetActive(true);
            //lightSaberBlade.transform.parent.GetComponent<Animator>().SetBool("Throw", true);
            //lightSaberBlade.GetComponent<MeshCollider>().enabled = true;
            gameObject.GetComponent<SimpleMouseRotator>().enabled = false;
            //damage = damage * 2f;

        }

        if ((Input.GetButtonUp("Force Attack") && forceChoke == true) || forceAmount < 0 || hit == true)
        {
            anim.SetBool("Force", false);
            forcePower.SetActive(false);
            //lightSaberBlade.transform.parent.GetComponent<Animator>().SetBool("Throw", false);
            //lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            gameObject.GetComponent<SimpleMouseRotator>().enabled = true;
            //damage = initialDamage;
        }
    }

    public void ComboStart()
    {
        if (canHit)
        {
            numberOfHits++;
        }

        if(numberOfHits == 1)
        {
            anim.SetInteger("Attack", 1);
            lightSaberBlade.GetComponent<MeshCollider>().enabled = true;
            saberSwing.Play();

        }


    }

    public void ComboCrack()
    {
        canHit = false;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 1") && numberOfHits == 1)
        {
            anim.SetInteger("Attack", 0);
            canHit = false;
            numberOfHits = 0;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            saberSwing.Stop();
            startAttackTimer = false;
            attackTimer = 0.0f;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 1") && numberOfHits >= 2)
        {
            anim.SetInteger("Attack", 2);
            canHit = true;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = true;
            saberSwing.Play();
        }

        else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 2") && numberOfHits == 2)
        {
            anim.SetInteger("Attack", 0);
            canHit = false;
            numberOfHits = 0;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            saberSwing.Stop();
            startAttackTimer = false;
            attackTimer = 0.0f;
        }

        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 2") && numberOfHits >= 3)
        {
            anim.SetInteger("Attack", 3);
            canHit = true;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = true;
            saberSwing.Play();
        }

        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Combo Part 3"))
        {
            anim.SetInteger("Attack", 0);
            canHit = false;
            numberOfHits = 0;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            saberSwing.Stop();
            startAttackTimer = false;
            attackTimer = 0.0f;
        }




        if (anim.GetCurrentAnimatorStateInfo(0).IsName("R Combo Part 1") && numberOfHits == 1)
        {
            anim.SetInteger("Attack", 0);
            canHit = false;
            numberOfHits = 0;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            saberSwing.Stop();
            startAttackTimer = false;
            attackTimer = 0.0f;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("R Combo Part 1") && numberOfHits >= 2)
        {
            anim.SetInteger("Attack", 2);
            canHit = true;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = true;
            saberSwing.Play();
        }

        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("R Combo Part 2") && numberOfHits == 2)
        {
            anim.SetInteger("Attack", 0);
            canHit = false;
            numberOfHits = 0;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            saberSwing.Stop();
            startAttackTimer = false;
            attackTimer = 0.0f;
        }

        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("R Combo Part 2") && numberOfHits >= 3)
        {
            anim.SetInteger("Attack", 3);
            canHit = true;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = true;
            saberSwing.Play();
        }

        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("R Combo Part 3"))
        {
            anim.SetInteger("Attack", 0);
            canHit = false;
            numberOfHits = 0;
            lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
            saberSwing.Stop();
            startAttackTimer = false;
            attackTimer = 0.0f;
        }


    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "LightSaberBlade" && other.gameObject.transform.root.tag == "Player" && dead == false && other.gameObject.transform.root.GetComponent<CharacterMovement>().lightSide != lightSide && dead == false)
        {
            Vector3 spawnHere = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z);

            health -= other.gameObject.transform.root.GetComponent<CharacterMovement>().damage;
            if (hit == false)
            {
                hit = true;
                anim.SetBool("Hit", true);
            }
            audioSHit.GetComponent<AudioSource>().PlayOneShot(saberHitSound);
            GameObject sparkEffecti = Instantiate(GameObject.FindGameObjectWithTag("Saber Sparks"), spawnHere, other.gameObject.transform.rotation);
            Destroy(sparkEffecti, 1f);
            attackTimer = 0.0f;
            startAttackTimer = false;
        }

        if (other.gameObject.tag == "LightSaberBlade" && other.gameObject.transform.root.tag == "AI"  && dead == false && other.gameObject.transform.root != this.gameObject)
        {
            if ((other.gameObject.transform.root.GetComponent<AIMovement>().lightSide == true && lightSide == false) || (other.gameObject.transform.root.GetComponent<AIMovement>().lightSide == false && lightSide == true))
            {
                Vector3 spawnHere = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z);

                health -= other.gameObject.transform.root.GetComponent<AIMovement>().damage;
                if(hit == false)
                {
                    hit = true;
                    anim.SetBool("Hit", true);
                    audioSHit.GetComponent<AudioSource>().PlayOneShot(saberHitSound);
                }
                GameObject sparkEffecti = Instantiate(GameObject.FindGameObjectWithTag("Saber Sparks"), spawnHere, other.gameObject.transform.rotation);
                Destroy(sparkEffecti, 1f);
                attackTimer = 0.0f;
                startAttackTimer = false;
            }
        }

        if (other.gameObject.tag == "Bullet" && other.gameObject.transform.root.GetComponent<BulletScript>().lightSide != lightSide && dead == false)
        {
            Vector3 spawnHere = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z);

            health -= other.gameObject.transform.root.GetComponent<BulletScript>().bulletDamage;
            if(hit == false)
            {
                hit = true;
                anim.SetBool("Hit", true);
            }
            audioSHit.GetComponent<AudioSource>().PlayOneShot(saberHitSound);
            GameObject sparkEffecti = Instantiate(GameObject.FindGameObjectWithTag("Saber Sparks"), spawnHere, other.gameObject.transform.rotation);
            Destroy(sparkEffecti, 1f);
            attackTimer = 0.0f;
            startAttackTimer = false;
        }
    }
}
