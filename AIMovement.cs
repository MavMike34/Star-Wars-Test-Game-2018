using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIMovement : MonoBehaviour
{
    public Animator anim;
    public bool dead;

    public float health;
    public float startingHealth;

    public float damage;
    public float initialDamage;
    public float forceDamage;
    public float forceAmount;
    public float startingForceAmount;

    public bool saberThrow;

    public Text healthText;
    public Image healthBar;

    public bool hit;
    public float hitTimer;
    public float hitTime;

    public bool lightSide;

    public bool melee;
    public bool gunner;

    public bool attack;
    public GameObject barrelOfGun;
    public Rigidbody bullet;
    public float bulletSpeed;

    public GameObject player;
    public float aiMovement;
    public bool aiCoolDown;
    public float distFromPlayer;
    public float aiCoolDownTimer;
    public float aiCooldownTime;

    public int chooseAIMove;

    //public Image forceBar;
    //public Text forceText;

    //public int numberOfHits;
    //public bool canHit;

    public float attackTimer;
    public float attackTime1;
    public float attackTime2;
    public float attackTime3;
    public float attackTime4;
    public bool startAttackTimer;

    public GameObject lightSaberBlade;
    public GameObject gun;
    public AudioSource saberSwing;

    public GameObject audioSHit;
    public AudioClip saberHitSound;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        //numberOfHits = 0;
        //canHit = true;

        health = startingHealth;
        forceAmount = startingForceAmount;
    }

    // Update is called once per frame
    void Update()
    {

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if(melee == true)
            {
                lightSaberBlade.SetActive(true);
            }
            
        }


            audioSHit = GameObject.FindGameObjectWithTag("Saber Hit");

        if(hit == true)
        {
            hitTimer += Time.deltaTime;

            if(hitTimer >= hitTime)
            {
                hitTimer = 0.0f;
                hit = false;
                anim.SetBool("Hit", false);
            }
        }

        if(health <= 0)
        {
            dead = true;
            health = 0.0f;
           
        }

        if (forceAmount <= 0)
        {
            forceAmount = 0;
        }

        healthText.text = health.ToString();
        healthBar.fillAmount = health / startingHealth;

        //forceText.text = forceAmount.ToString("f0");
        //forceBar.fillAmount = forceAmount / startingForceAmount;

        if (forceAmount <= startingForceAmount && anim.GetBool("Special") == false)
        {
            forceAmount += Time.deltaTime * (startingForceAmount / 16);

            if (forceAmount >= startingForceAmount)
            {
                forceAmount = startingForceAmount;
            }
        }

        if (anim.GetBool("Special") == true)
        {
            forceAmount -= Time.deltaTime * (startingForceAmount / 2);
        }

        if(anim.GetBool("Force") == true)
        {
            forceAmount -= Time.deltaTime * (startingForceAmount / 2);
        }

        if (dead == false)
        {
            Movements();
        }

        if (dead == true)
        {
            healthBar.transform.parent.gameObject.SetActive(false);

            if (melee == true)
            {
                lightSaberBlade.SetActive(false);
            }

            if(gunner == true)
            {
                gun.SetActive(false);
            }
            
            anim.SetBool("Die", true);
            anim.SetBool("Hit", false);
            anim.SetBool("Choke", false);

            Destroy(this.gameObject, 5f);
        }

        //if (startAttackTimer == true)
        //{
        //    attackTimer += Time.deltaTime;

        //    if (attackTimer >= 4f)
        //    {
        //        anim.SetInteger("Attack", 0);
        //        canHit = true;
        //        numberOfHits = 0;
         //       lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
        //        saberSwing.Stop();
         //       attackTimer = 0.0f;
         //       startAttackTimer = false;
         //   }
       // }

    }

    public void Movements()
    {
        // Movement Walking Forward, Back, Left, and Right.

        //anim.SetFloat("MovementX", 1f, 1f, Time.deltaTime * 20f);

            aiMovement = Mathf.Clamp(aiMovement, 0f, 1f);

            player = GameObject.FindGameObjectWithTag("Player");


            if (player.GetComponent<CharacterMovement>().health == 0.0f)
            {
                anim.SetFloat("MovementY", 0f);
                anim.SetInteger("Attack", 0);
            }

        if (dead == false && hit == false)
        {
            // Movements();
            // this.gameObject.GetComponent<SimpleMouseRotator>().enabled = true;
            if (melee == true &&  !anim.GetCurrentAnimatorStateInfo(0).IsName("Choking"))
            {
                lightSaberBlade.SetActive(true);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Choking"))
            {
                if(melee == true)
                {
                    lightSaberBlade.SetActive(false);
                }
      
            }

            // AI Movement and Decisions

            if (aiCoolDown == true)
            {
                aiCoolDownTimer += Time.deltaTime;

                if(melee == true)
                {
                    lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
                }
            

                if (aiCoolDownTimer >= aiCooldownTime)
                {
                    if(melee == true)
                    {
                        chooseAIMove = Random.Range(1, 8);
                    }
                    aiCoolDown = false;
                    aiCoolDownTimer = 0.0f;
                }
            }

            Vector3 direction = player.transform.position - this.transform.position;
            direction.y = 0.0f;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);

            if (direction.magnitude > distFromPlayer)
            {
                aiMovement += 1f * Time.deltaTime;

               // if (aiMovement >= 1f)
               // {
               //     aiMovement = 1.0000f;
               // }

                anim.SetFloat("MovementY", aiMovement);
                anim.SetInteger("Attack", 0);
                attack = false;
                attackTimer = 0.0f;
                startAttackTimer = false;
                aiCoolDown = false;
                aiCoolDownTimer = 0.0f;

                if (melee == true)
                {
                    lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
                }
                

            }

            if (direction.magnitude < distFromPlayer)
            {
                aiMovement -= 20f * Time.deltaTime;

               // if (aiMovement <= 0f)
               // {
               //     aiMovement = 0.0000f;
               // }

                anim.SetFloat("MovementY", aiMovement);
                  if(attack == false && hit == false)
                  {
                  chooseAIMove = Random.Range(1, 8);
                  }

                if (melee == true && hit == false && anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {



                    if (chooseAIMove >= 1 && chooseAIMove <= 2 && aiCoolDown == false)
                    {

                        // anim.SetFloat("MovementY", 0);
                        //basicattack = true;
                        anim.SetInteger("Attack", 1);
                    }

                    if (chooseAIMove >= 3 && chooseAIMove <= 4 && aiCoolDown == false)
                    {

                        // anim.SetFloat("MovementY", 0);
                        //heavyattack = true;
                        anim.SetInteger("Attack", 1);
                    }

                    if (chooseAIMove >= 5 && chooseAIMove <= 6 && aiCoolDown == false)
                    {

                        // anim.SetFloat("MovementY", 0);
                        // comboattack = true;
                        anim.SetInteger("Attack", 1);
                    }

                    if (chooseAIMove >= 7 && chooseAIMove <= 8 && aiCoolDown == false)
                    {

                        // anim.SetFloat("MovementY", 0);
                        //forceattack = true;
                        anim.SetInteger("Attack", 1);
                    }
                }

                if (melee == true && aiCoolDown == false && hit == false)
                {
                    attack = true;



                    if (attack == true)
                    {

                        lightSaberBlade.GetComponent<MeshCollider>().enabled = true;

                        startAttackTimer = true;

                        if(startAttackTimer == true)
                        {
                            attackTimer += Time.deltaTime;

                            if(chooseAIMove >= 1 && chooseAIMove <= 2 && attackTimer >= attackTime1)
                            {
                                anim.SetInteger("Attack", 0);
                                attack = false;
                                attackTimer = 0.0f;
                                startAttackTimer = false;
                                aiCoolDown = true;
                                lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
                            }

                            if (chooseAIMove >= 3 && chooseAIMove <= 4 && attackTimer >= attackTime2)
                            {
                                anim.SetInteger("Attack", 0);
                                attack = false;
                                attackTimer = 0.0f;
                                startAttackTimer = false;
                                aiCoolDown = true;
                                lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
                            }

                            if (chooseAIMove >= 5 && chooseAIMove <= 6 && attackTimer >= attackTime3)
                            {
                                anim.SetInteger("Attack", 0);
                                attack = false;
                                attackTimer = 0.0f;
                                startAttackTimer = false;
                                aiCoolDown = true;
                                lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
                            }

                            if (chooseAIMove >= 7 && chooseAIMove <= 8 && attackTimer >= attackTime4)
                            {
                                anim.SetInteger("Attack", 0);
                                attack = false;
                                attackTimer = 0.0f;
                                startAttackTimer = false;
                                aiCoolDown = true;
                                lightSaberBlade.GetComponent<MeshCollider>().enabled = false;
                            }


                        }
                        

                        
                    }
                }

                if (gunner == true && aiCoolDown == false && hit == false)
                {
                    attack = true;



                    if(attack == true)
                    {
                        Rigidbody bulletInstance;
                        bulletInstance = Instantiate(bullet, barrelOfGun.transform.position, barrelOfGun.transform.rotation) as Rigidbody;
                        anim.SetTrigger("Attack");

                        bulletInstance.AddForce(barrelOfGun.transform.forward * bulletSpeed);

                        Destroy(bulletInstance, 4f);

                        aiCoolDown = true;
                    }
                }
            }
        }

        }

    public void ComboCrack()
    {
        return;
    }


    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "LightSaberBlade" && other.gameObject.transform.root.tag == "Player" && dead == false && dead == false)
        {
            if ((other.gameObject.transform.root.GetComponent<CharacterMovement>().lightSide == true && lightSide == false) || (other.gameObject.transform.root.GetComponent<CharacterMovement>().lightSide == false && lightSide == true))
            {
                Vector3 spawnHere = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z);

                health -= other.gameObject.transform.root.GetComponent<CharacterMovement>().damage;
                hit = true;
                anim.SetBool("Hit", true);
                audioSHit.GetComponent<AudioSource>().PlayOneShot(saberHitSound);
                GameObject sparkEffecti = Instantiate(GameObject.FindGameObjectWithTag("Saber Sparks"), spawnHere, other.gameObject.transform.rotation);
                Destroy(sparkEffecti, 1f);
            }
        }

        if(other.gameObject.tag == "LightSaberBlade" && other.gameObject.transform.root.tag == "AI"   && dead == false)
        {

            if ((other.gameObject.transform.root.GetComponent<AIMovement>().lightSide == true && lightSide == false) || (other.gameObject.transform.root.GetComponent<AIMovement>().lightSide == false && lightSide == true))
            {
                Vector3 spawnHere = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z);

                health -= other.gameObject.transform.root.GetComponent<AIMovement>().damage;
                hit = true;
                anim.SetBool("Hit", true);
                audioSHit.GetComponent<AudioSource>().PlayOneShot(saberHitSound);
                GameObject sparkEffecti = Instantiate(GameObject.FindGameObjectWithTag("Saber Sparks"), spawnHere, other.gameObject.transform.rotation);
                Destroy(sparkEffecti, 1f);
            }
           
        }

        if (other.gameObject.tag == "Force Power" && other.gameObject.transform.root.tag == "Player" && dead == false && dead == false)
        {
            if ((other.gameObject.transform.root.GetComponent<CharacterMovement>().lightSide == true && lightSide == false) || (other.gameObject.transform.root.GetComponent<CharacterMovement>().lightSide == false && lightSide == true))
            {
                //Vector3 spawnHere = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z);

                health -= other.gameObject.transform.root.GetComponent<CharacterMovement>().forceDamage;
                hit = true;
                if(other.gameObject.transform.root.GetComponent<CharacterMovement>().forceChoke == true)
                {
                    anim.SetTrigger("Choke");
                    //lightSaberBlade.SetActive(false);
                }
                
                //audioSHit.GetComponent<AudioSource>().PlayOneShot(saberHitSound);
                //GameObject sparkEffecti = Instantiate(GameObject.FindGameObjectWithTag("Saber Sparks"), spawnHere, other.gameObject.transform.rotation);
                //Destroy(sparkEffecti, 1f);
            }
        }
    }
}
