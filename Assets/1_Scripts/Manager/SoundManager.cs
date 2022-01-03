using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    private void Awake()
    {
        Instance = this;
        if(Instance==null)
        {
            Instance = GetComponent<SoundManager>();
        }
    }

    [SerializeField]
    AudioSource BGMSource;
    [SerializeField]
    AudioClip[] BGMClip;
    [SerializeField]
    AudioSource SFXSource;
    [SerializeField]
    AudioClip[] SFXClip;
    [SerializeField]
    AudioSource ClickSource;

    //클릭 사운드
    public void Click()
    {
        ClickSource.Play();
    }

    //효과 사운드
    public void SFXPlay(int n)
    {
        SFXSource.PlayOneShot(SFXClip[n]);
    }
    public void SFXStop()
    {
        SFXSource.Stop();
    }

    //배경음 사운드
    public void BGMPlay()
    {
        BGMSource.Play();
    }
    public void BGMStop()
    {
        BGMSource.Stop();
    }
}
