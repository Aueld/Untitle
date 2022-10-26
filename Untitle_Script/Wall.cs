using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle; 

    // 벽 체력
    public int hp = 4;

    // 벽 히트 사운드
    public AudioClip chopSound;

    // 벽 애니메이터
    private Animator animator;
    private CameraShake cameraShake;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    // 벽 데미지
    public bool DamageWall(int loss)
    {
        particle.Play();
        SoundManager.instance.RandomizeSfx(0.75f, chopSound, chopSound);
        animator.SetTrigger("Block");

        hp -= loss;

        if (hp <= 0)
        {
            cameraShake.ShakeForTimeAdd(0.6f);
            gameObject.SetActive(false);
            return false;
        }

        return true;
    }

    public bool BoomWall()
    {
        particle.Play();
        cameraShake.ShakeForTimeAdd(0.6f);
        gameObject.SetActive(false);
        return false;
    }
}
