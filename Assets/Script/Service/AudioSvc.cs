/****************************************************
    文件：AudioSvc.cs
	作者：www34
    邮箱: 
    日期：2025/10/12 16:44:50
	功能：声音服务
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour 
{
    public static AudioSvc instance;
    public AudioSource BGAudioSource;
    public AudioSource UIAudioSource;
    public void InitSvc()
    {
        instance = this;
        PECommon.Log("初始化声音系统");
    }

    public void StopBGMusic()
    {
        if (BGAudioSource != null)
        {
            BGAudioSource.Stop();
        }
    }
    public void PlayBGMuic(string name ,bool isLoop=true )
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        if (BGAudioSource.clip == null || BGAudioSource.clip.name != audioClip.name)
        {
            BGAudioSource.clip = audioClip;
            BGAudioSource.loop = isLoop;
            BGAudioSource.Play();
        }
    }
    public void PlayUIMusic(string name)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        UIAudioSource.clip = audioClip;
        UIAudioSource.Play();
    }
    public void PlayCharacterAudio(string name,AudioSource audioSource)
    {
        AudioClip audioClip = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    
}