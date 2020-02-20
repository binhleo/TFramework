using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public enum AudioType : byte
{
    BtnClick = 0,
    LobbyBG  = 200, //Background Music
    MaxCount = 255,

}

public class GameAudioMgr 
{
    private class AudioInfo
    {
        public readonly string m_res;
        public readonly bool m_loop;
        public readonly float m_volume;
        public AudioInfo(string res, bool bLoop, float volume = 1.0f)
        {
            m_res = res;
            m_loop = bLoop;
            m_volume = volume;
        }
        public AudioInfo(string res)
        {
            m_res = res;
            m_loop = false;
            m_volume = 1.0f;
        }
    }

    private GameObject m_listenerGO;

    private Dictionary<byte, AudioInfo> m_audioClipResDic;
    private Dictionary<byte, GameObject> m_audioObjDic;
    private GameObject m_audioRootObj;
    private Dictionary<string, GameObject> m_audioObjByNameDic;



    private Dictionary<byte, AudioInfo> m_musicClipResDic;
    private GameObject m_musicRootObj;
    private GameObject m_musicObj;

    private int m_timeId;
    private float m_fadeIn;
    private float m_fadeOut;
    private float m_fadeRate;
    private AudioType m_curMusic;
    private AudioType m_lastMusic;
    public bool AudioState = true;


    public GameAudioMgr()
    {
        m_listenerGO = new GameObject("SoundListener");
        m_listenerGO.AddComponent<AudioListener>();
        GameObject.DontDestroyOnLoad(m_listenerGO);

        m_audioClipResDic = new Dictionary<byte, AudioInfo>();

        //Add AudioClip
        m_audioClipResDic.Add((byte)AudioType.BtnClick, new AudioInfo("Sound_Btn_Click"));

        m_audioObjDic = new Dictionary<byte, GameObject>();
        m_audioRootObj = new GameObject("AudioSourceRoot");
        m_audioRootObj.transform.parent = m_listenerGO.transform;
        m_audioRootObj.transform.localPosition = Vector3.zero;
        m_audioObjByNameDic = new Dictionary<string, GameObject>();


        //Add Music 
        m_musicClipResDic = new Dictionary<byte, AudioInfo>();
        m_musicClipResDic.Add((byte)AudioType.LobbyBG, new AudioInfo("LobbyBG", true, 1));

        m_musicRootObj = new GameObject("MusicSourceRoot");
        m_musicRootObj.transform.parent = m_listenerGO.transform;
        m_musicRootObj.transform.localPosition = Vector3.zero;

        m_fadeIn = 1.0f;
        m_fadeOut = 0.0f;
        m_fadeRate = 0.08f;
    }

    
    private void StartMusicFade()
    {
        m_fadeOut = GetAudioCurVloume(m_lastMusic);
        m_fadeIn = 0.0f;
        m_timeId = TimerHelper.SetRepeatTimer(MusicFadeIn, 0.1f);
    }

    private void MusicFadeIn()
    {
        if (m_musicObj != null)
        {
            if (m_fadeOut > 0.0f)
            {
                AudioSource audioSource = m_musicObj.GetComponent<AudioSource>();
                m_fadeOut -= m_fadeRate;
                audioSource.volume = Mathf.Clamp01(m_fadeOut);
            }
            else
            {
                AudioSource audioSource = m_musicObj.GetComponent<AudioSource>();
                if (m_fadeIn <= 0.0f)
                {
                    AudioClip audioClip1 = TGameCore.GetInstance().GetResourceMgr().GetResourceByPath<AudioClip>(m_musicClipResDic[(byte)m_curMusic].m_res);
                    if (audioClip1 != null)
                    {
                        audioSource.clip = audioClip1 as AudioClip;
                        if (AudioState)
                            audioSource.Play();
                    }                    
                }
                if (m_fadeIn < GetAudioVloume(m_curMusic))
                {
                    m_fadeIn += m_fadeRate;
                    audioSource.volume = Mathf.Clamp01(m_fadeIn);
                }
                else
                {
                    EndMusicFade();
                }
            }
        }
    }
    private void EndMusicFade()
    {
        TimerHelper.KillTimer(m_timeId);
    }

    public void EnableAudio(bool enable)
    {        
        m_audioRootObj.SetActive(enable);        
        CheckMute();
    }
    public void EnableMusic(bool enable)
    {     
        m_musicRootObj.SetActive(enable);
        CheckMute();
    }
    private void CheckMute()
    {
        if (m_audioRootObj.activeSelf || m_musicRootObj.activeSelf)
        {   
            AudioListener.pause = false;
        }
        else
        {            
            AudioListener.pause = true;
        }
    }

    private void PlaySound(AudioType audioType)
    {
        if (audioType != AudioType.MaxCount)
        {
            AudioSource ret = null;
            if (m_audioClipResDic.ContainsKey((byte)audioType))
            {
                AudioClip audioClip1 = TGameCore.GetInstance().GetResourceMgr().GetResourceByPath<AudioClip>(m_musicClipResDic[(byte)audioType].m_res);
                if (audioClip1 == null)
                {
                    TGameCore.Log("audioType:" + audioType + " is null");
                    return;
                }
                if (!m_audioObjDic.ContainsKey((byte)audioType))
                {
                    GameObject go = new GameObject(audioType.ToString());
                    go.transform.parent = m_audioRootObj.transform;
                    go.transform.localPosition = Vector3.zero;
                    AudioSource audioSource = go.AddComponent<AudioSource>();
                    audioSource.rolloffMode = AudioRolloffMode.Linear;
                    //Debug.Log("m_audioClipResDic[aType].m_res:  " + m_audioClipResDic[aType].m_res);
                    audioSource.clip = audioClip1 as AudioClip;
                    audioSource.loop = m_audioClipResDic[(byte)audioType].m_loop;
                    audioSource.playOnAwake = false;
                    m_audioObjDic.Add((byte)audioType, go);
                }
                ret = m_audioObjDic[(byte)audioType].GetComponent<AudioSource>();
                if (ret != null)
                {
                    if (m_audioClipResDic.ContainsKey((byte)audioType))
                    {
                        ret.volume = GetAudioVloume(audioType);
                        if (ret.gameObject.activeInHierarchy)
                        {
                            //Debug.LogError("volume"+  ret.volume);
                            if (AudioState)
                                ret.Play();
                        }
                    }
                    else if (m_musicClipResDic.ContainsKey((byte)audioType))
                    {
                        StartMusicFade();
                    }
                }
            }
            else if (m_musicClipResDic.ContainsKey((byte)audioType))
            {
                if (m_musicObj == null)
                {
                    m_musicObj = new GameObject(audioType.ToString());
                    m_musicObj.transform.parent = m_musicRootObj.transform;
                    m_musicObj.transform.localPosition = Vector3.zero;
                    m_musicObj.AddComponent<AudioSource>();
                }
                ret = m_musicObj.GetComponent<AudioSource>();
                if (ret.clip != null && ret.clip.name == m_musicClipResDic[(byte)audioType].m_res && ret.isPlaying)
                {
                    ret = null;
                }
                else
                {
                    ret.rolloffMode = AudioRolloffMode.Linear;
                    ret.loop = m_musicClipResDic[(byte)audioType].m_loop;
                    m_lastMusic = (AudioType)Enum.Parse(typeof(AudioType), m_musicObj.name);
                    m_curMusic = audioType;
                    m_musicObj.name = audioType.ToString();
                }
            }
            if (ret != null)
            {
                if (m_audioClipResDic.ContainsKey((byte)audioType))
                {
                    ret.volume = GetAudioVloume(audioType);
                    if (ret.gameObject.activeInHierarchy)
                    {
                        if (AudioState)
                            ret.Play();
                    }
                }
                else if (m_musicClipResDic.ContainsKey((byte)audioType))
                {
                    StartMusicFade();
                }
            }
        }
    }

    public void PlaySound(string resPath, float volume = -1)
    {

        if (!string.IsNullOrEmpty(resPath))
        {
            AudioSource ret = null;
            if (!m_audioObjByNameDic.ContainsKey(resPath))
            {
                AudioClip audioClip1 = TGameCore.GetInstance().GetResourceMgr().GetResourceByPath<AudioClip>(resPath);
                if (audioClip1 != null)
                {
                    GameObject go = new GameObject(resPath);
                    go.transform.parent = m_audioRootObj.transform;
                    go.transform.localPosition = Vector3.zero;
                    AudioSource audioSource = go.AddComponent<AudioSource>();
                    audioSource.rolloffMode = AudioRolloffMode.Linear;
                    audioSource.clip = audioClip1 as AudioClip;
                    audioSource.loop = false;
                    audioSource.playOnAwake = false;
                    if (volume >= 0)
                    {
                        audioSource.volume = volume;
                    }
                    //Debug.LogError("go.name:" + go.name + ",resPath:" + resPath);
                    m_audioObjByNameDic.Add(resPath, go);
                    ret = m_audioObjByNameDic[resPath].GetComponent<AudioSource>();
                    if (ret != null)
                    {
                        if (AudioState)
                            ret.Play();
                    }
                }
                else
                {
                    TGameCore.Log("resPath:" + resPath + " is null");
                    return;
                }

            }
            else
            {
                ret = m_audioObjByNameDic[resPath].GetComponent<AudioSource>();
                if (ret != null)
                {
                    if (AudioState)
                        ret.Play();
                }
            }
        }
    }

    private void CloseSound(List<AudioType> closeAudoiList)
    {
        if (closeAudoiList != null)
        {
            for (int index = 0; index < closeAudoiList.Count; index++)
            {
                if (m_audioObjDic.ContainsKey((byte)closeAudoiList[index]))
                {
                    GameObject soundGO = m_audioObjDic[(byte)closeAudoiList[index]];
                    GameObject.Destroy(soundGO);
                    m_audioObjDic.Remove((byte)closeAudoiList[index]);
                }
                else if (m_musicObj != null && m_musicObj.name == closeAudoiList[index].ToString())
                {
                    GameObject.Destroy(m_musicObj);
                    m_musicObj = null;
                }
            }
        }
    }

    public void CloseSound(string resPath)
    {
        if (!string.IsNullOrEmpty(resPath) && m_audioObjByNameDic.ContainsKey(resPath))
        {
            GameObject soundGO = m_audioObjByNameDic[resPath];
            GameObject.Destroy(soundGO);
            m_audioObjByNameDic.Remove(resPath);
        }
    }

    private void ClearSound()
    {
        List<byte> keys = m_audioObjDic.Keys.ToList();
        List<byte>.Enumerator enumerator1 = keys.GetEnumerator();
        while (enumerator1.MoveNext())
        {
            if (m_audioObjDic.ContainsKey(enumerator1.Current) && m_audioObjDic[enumerator1.Current] != null)
            {
                AudioSource audioSource = m_audioObjDic[enumerator1.Current].GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    if (audioSource.isPlaying)
                    {
                        float length = audioSource.clip == null ? 0.0f : audioSource.clip.length;
                        GameObject.Destroy(audioSource.gameObject, length);
                    }
                    else
                    {
                        GameObject.Destroy(audioSource.gameObject);
                    }
                    m_audioObjDic.Remove(enumerator1.Current);
                }
            }
        }
        Dictionary<string, GameObject>.Enumerator enumerator = m_audioObjByNameDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            GameObject.Destroy(enumerator.Current.Value);
        }
        m_audioObjByNameDic.Clear();
    }

    private void PauseSound(AudioType audioType)
    {
        if (audioType != AudioType.MaxCount)
        {
            if (m_audioObjDic.ContainsKey((byte)audioType))
            {
                m_audioObjDic[(byte)audioType].GetComponent<AudioSource>().Pause();
            }
            else if (m_musicObj != null && m_musicObj.name == audioType.ToString())
            {
                m_musicObj.GetComponent<AudioSource>().Pause();
            }
        }
    }

    private void SetVloume(AudioType type, float volume)
    {
        if (type != AudioType.MaxCount)
        {
            if (m_audioObjDic.ContainsKey((byte)type))
            {
                m_audioObjDic[(byte)type].GetComponent<AudioSource>().volume = volume;
            }
            else if (m_musicObj != null && m_musicObj.name == type.ToString())
            {
                m_musicObj.GetComponent<AudioSource>().volume = volume;
            }
        }
    }

    private float GetAudioCurVloume(AudioType type)
    {
        float volume = 0.0f;
        if (m_audioObjDic.ContainsKey((byte)type))
        {
            volume = m_audioObjDic[(byte)type].GetComponent<AudioSource>().volume;
        }
        else if (m_musicObj != null)
        {
            volume = m_musicObj.GetComponent<AudioSource>().volume;
        }
        return volume;
    }
    private float GetAudioVloume(AudioType type)
    {
        float volume = 1.0f;
        if (m_audioClipResDic.ContainsKey((byte)type))
        {
            volume = m_audioClipResDic[(byte)type].m_volume;
        }
        else if (m_musicClipResDic.ContainsKey((byte)type))
        {
            volume = m_musicClipResDic[(byte)type].m_volume;
        }
        return volume;
    }



}
