using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public AudioClip swordSFX;
    Animator m_Animator;

    float elapsedTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            AttackAnimation();
            if (elapsedTime >= 1.2f) {
                playSwordSwooshAudio();
                elapsedTime = 0;
            }
        }
        if (Input.GetKey(KeyCode.F) && LevelManager.glovePickedUp) {
            HeavyAttackAnimation();
        }
        elapsedTime += Time.deltaTime;
    }
    private void AttackAnimation()
    {
        m_Animator.SetInteger("animState", 3);
    }

    private void HeavyAttackAnimation()
    {
        m_Animator.SetInteger("animState", 8);
    }

    private void playSwordSwooshAudio() {
        AudioSource.PlayClipAtPoint(swordSFX, Camera.main.transform.position);
    }
}
