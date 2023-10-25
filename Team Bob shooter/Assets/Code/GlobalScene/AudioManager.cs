using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TeamBobFPS.Save;

#region Audio ENUM
public enum EGameSFX
{
    // PLAYER SFX
    _SFX_PLAYER_RUN_GRASS,
    _SFX_PLAYER_RUN_WOOD,
    _SFX_PLAYER_RUN_STONE,
    _SFX_PLAYER_JUMP,
    _SFX_PLAYER_DOUBLE_JUMP,
    _SFX_PLAYER_DASH,
    _SFX_PLAYER_LAND,
    _SFX_PLAYER_TAKE_DAMAGE,
    _SFX_PLAYER_TAKE_DAMAGE2,
    _SFX_PLAYER_DIE,

    // WEAPON SFX
    _SFX_SHOTGUN_SHOOT,
    _SFX_SHOTGUN_RELOAD,
    _SFX_PISTOL_SHOOT,
    _SFX_PISTOL_RELOAD,
    _SFX_MINIGUN_SHOOT,
    _SFX_ROCKETLAUNCHER_SHOOT,
    _SFX_ROCKETLAUNCHER_RELOAD,
    _SFX_RAILGUN_SHOOT,
    _SFX_RAILGUN_RELOAD,

    // ENEMY SFX
    _SFX_GNOME_WALK,
    _SFX_GNOME_ATTACK,
    _SFX_GNOME_TAKE_DAMAGE,
    _SFX_GNOME_DIE,

    // COLLECTIBLE SFX
    _SFX_COLLECT_AMMO,
    _SFX_COLLECT_HEALTH,

    // UI SFX
    _SFX_TEXT_TYPE,
    _SFX_UI_SELECT,

    // NULL OPTION
    _NULL,
}
public enum EGameMusic
{
    _BOBCAT_INGAME,
}
#endregion

namespace TeamBobFPS
{
    public class AudioManager : BaseUpdateListener, ISaveableConfig
    {
        #region Mixer Groups & Audio Controllers
        [Header("Mixer Groups & Audio Controllers")]
        [SerializeField] private AudioMixerGroup mixerSFX = null;
        [SerializeField] private AudioMixerGroup mixerMusic = null;
        [SerializeField] private AudioMixer mixerMain = null;

        //[SerializeField] private AudioControl sfxControl;
        //[SerializeField] private AudioControl musicControl;
        //[SerializeField] private AudioControl masterControl;

        [SerializeField] private string sfxVolumeName;
        [SerializeField] private string musicVolumeName;
        [SerializeField] private string masterVolumeName;
        #endregion

        #region SFX SerializeFields
        [Header("Player SFX")]
        [SerializeField]
        private AudioClip playerRunGrass;
        [SerializeField]
        private AudioClip playerRunWood;
        [SerializeField]
        private AudioClip playerRunStone;
        [SerializeField]
        private AudioClip playerJump;
        [SerializeField]
        private AudioClip playerDoubleJump;
        [SerializeField]
        private AudioClip playerDash;
        [SerializeField]
        private AudioClip playerLand;
        [SerializeField]
        private AudioClip playerTakeDamage;
        [SerializeField]
        private AudioClip playerTakeDamage2;
        [SerializeField]
        private AudioClip playerDie;

        [Header("Weapon SFX")]
        [SerializeField]
        private AudioClip shotgunShoot;
        [SerializeField]
        private AudioClip shotgunReload;
        [SerializeField]
        private AudioClip pistolShoot;
        [SerializeField]
        private AudioClip pistolReload;
        [SerializeField]
        private AudioClip minigunShoot;
        [SerializeField]
        private AudioClip rocketLauncherShoot;
        [SerializeField]
        private AudioClip rocketLauncherReload;
        [SerializeField]
        private AudioClip railgunShoot;
        [SerializeField]
        private AudioClip railgunReload;

        [Header("Enemy SFX")]
        [SerializeField]
        private AudioClip gnomeWalk;
        [SerializeField]
        private AudioClip gnomeAttack;
        [SerializeField]
        private AudioClip gnomeTakeDamage;
        [SerializeField]
        private AudioClip gnomeDie;

        [Header("Collectible SFX")]
        [SerializeField]
        private AudioClip collectAmmo;
        [SerializeField]
        private AudioClip collectHealth;

        [Header("UI SFX")]
        [SerializeField]
        private AudioClip textType;
        [SerializeField]
        private AudioClip UISelect;

        [Header("Music Clips")]
        [SerializeField] private AudioClip level1Music = null;
        #endregion

        public SaveObjectType SaveType
        {
            get { return SaveObjectType.PlayerSettings; }
        }

        // an SFX pool using array
        private GameObject[] SFXAudioPool;
        private const int SFXPoolSize = 500;

        // Index of the next GameObject to grab from the pool.
        private int SFXAudioPoolIndex = 0;

        // We want to limit instances of the same audio effects, so 
        // store in this list, each frame, every SFX ID that we've triggered.
        // If it's in the list, it won't be played again this frame.
        private List<EGameSFX> audioEffectList = new();

        // Dictionary of looping audio sources so they can be managed
        private Dictionary<EGameSFX, AudioSource> loopingAudioList = new();

        // Music is attached to a GameObject, stored here. 
        private GameObject goMusic = null;
        private AudioSource activeMusicAudioSource = null;

        // Co_Routine has no knowlegde of game state, so we need a 
        // flag we can set/clear in order for the music fade 
        // to cancel itself if th player short-circuits out of the
        // game over screen quickly...
        private bool canFadeMusic = false;

        private Coroutine stopLoopRoutine = null;

        // Don't play audio before SetDefaults
        // relates to playerprefs
        // private bool m_bIsSetup = false;

        protected override void Awake()
        {
            base.Awake();

            SFXAudioPool = new GameObject[SFXPoolSize];
            for (int i = 0; i < SFXPoolSize; ++i)
            {
                SFXAudioPool[i] = new GameObject();
                SFXAudioPool[i].name = "SFX_Pool_" + i.ToString();
                AudioSource sfxSource = SFXAudioPool[i].AddComponent<AudioSource>();
                sfxSource.maxDistance = 30f;
                sfxSource.spatialBlend = 1f;
                sfxSource.rolloffMode = AudioRolloffMode.Linear;
                if (sfxSource != null)
                {
                    sfxSource.outputAudioMixerGroup = mixerSFX;
                }

                DontDestroyOnLoad(SFXAudioPool[i]);
            }
            SFXAudioPoolIndex = 0;

            // Setup a GameObject that will hold the music clip
            {
                goMusic = new GameObject();
                goMusic.name = "AUDIO_Music";
                activeMusicAudioSource = goMusic.AddComponent<AudioSource>();
                if (null != activeMusicAudioSource) Debug.Log("Unable to add Audio Source component to goMusic");
                activeMusicAudioSource.outputAudioMixerGroup = mixerMusic;
                activeMusicAudioSource.bypassEffects = true;
                activeMusicAudioSource.bypassListenerEffects = true;
                activeMusicAudioSource.bypassReverbZones = true;
                activeMusicAudioSource.loop = true;
                activeMusicAudioSource.spatialBlend = 0.0f;
                activeMusicAudioSource.rolloffMode = AudioRolloffMode.Custom;       // Shouldn't need to set these, but just in case...
                activeMusicAudioSource.minDistance = 0.1f;
                activeMusicAudioSource.maxDistance = 100000000000.0f;

                // Again, don't want this disappearing somewhere...
                DontDestroyOnLoad(goMusic);
            }

            // Clear the effect list.
            NextFrame();
        }

        private void Start()
        {
            //sfxControl.Setup(mixerMain, sfxVolumeName);
            //musicControl.Setup(mixerMain, musicVolumeName);
            //masterControl.Setup(mixerMain, masterVolumeName);

            //GameInstance.Instance.GetSaveManager().ConfigLoad();
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            NextFrame();
        }

        /// <summary>
        /// Clear our list of SFX we're not playing again this frame.
        /// </summary>
        public void NextFrame()
        {
            audioEffectList.Clear();
        }

        /// <summary>
        /// If the specified audio is currently playing, sets its looping state to false and stops the audio
        /// </summary>
        /// <param name="intSFX">SFX given as EGameSFX enum</param>
        public void StopLoopingAudio(EGameSFX intSFX, bool fadeOut = true)
        {
            if (!loopingAudioList.ContainsKey(intSFX)) return;

            if (!fadeOut)
            {
                loopingAudioList[intSFX].loop = false;
                loopingAudioList[intSFX].Stop();
                loopingAudioList.Remove(intSFX);
            }
            else
            {
                if (stopLoopRoutine != null)
                {
                    StopCoroutine(stopLoopRoutine);
                }
                stopLoopRoutine = StartCoroutine(LoopingAudioFadeOut(intSFX));
            }
        }

        private IEnumerator LoopingAudioFadeOut(EGameSFX intSFX)
        {
            while (loopingAudioList[intSFX].volume > 0)
            {
                loopingAudioList[(intSFX)].volume -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale * 3;
                if (loopingAudioList[(intSFX)].volume < 0)
                {
                    loopingAudioList[(intSFX)].volume = 0;
                }
                yield return null;
            }
            loopingAudioList[intSFX].loop = false;
            loopingAudioList[intSFX].Stop();
            loopingAudioList.Remove(intSFX);
        }

        /// <summary>
        /// Grabs a new GameObject from the pool, gives it a new AudioClip and plays it.
        /// </summary>
        /// <param name="intSFX">SFX given as EGameSFX enum</param>
        /// <param name="pos">Vector3 where SFX should be played</param>
        public void PlayAudioAtLocation(EGameSFX intSFX, Vector3 pos, float volume = 1f, bool loop = false, bool make2D = false, bool canStack = false)
        {
            // TODO: check if player prefs setup is done
            // if (!m_bIsSetup) return;

            // check that this effect is not played duplicate in current frame
            // GameInstance clears audioEffectList every frame
            if (audioEffectList.Contains(intSFX) && !canStack) return;
            else audioEffectList.Add(intSFX);


            AudioSource audioSourceSFX = SFXAudioPool[SFXAudioPoolIndex].GetComponent<AudioSource>();
            audioSourceSFX.transform.parent = null;
            DontDestroyOnLoad(audioSourceSFX);
            audioSourceSFX.Stop();

            switch (intSFX)
            {
                #region SFX switch case using ENUM
                case EGameSFX._SFX_PLAYER_RUN_GRASS: audioSourceSFX.clip = playerRunGrass; break;
                case EGameSFX._SFX_PLAYER_RUN_WOOD: audioSourceSFX.clip = playerRunWood; break;
                case EGameSFX._SFX_PLAYER_RUN_STONE: audioSourceSFX.clip = playerRunStone; break;
                case EGameSFX._SFX_PLAYER_JUMP: audioSourceSFX.clip = playerJump; break;
                case EGameSFX._SFX_PLAYER_DOUBLE_JUMP: audioSourceSFX.clip = playerDoubleJump; break;
                case EGameSFX._SFX_PLAYER_DASH: audioSourceSFX.clip = playerDash; break;
                case EGameSFX._SFX_PLAYER_LAND: audioSourceSFX.clip = playerLand; break;
                case EGameSFX._SFX_PLAYER_TAKE_DAMAGE: audioSourceSFX.clip = playerTakeDamage; break;
                case EGameSFX._SFX_PLAYER_TAKE_DAMAGE2: audioSourceSFX.clip = playerTakeDamage2; break;
                case EGameSFX._SFX_PLAYER_DIE: audioSourceSFX.clip = playerDie; break;

                // WEAPON SFX
                case EGameSFX._SFX_SHOTGUN_SHOOT: audioSourceSFX.clip = shotgunShoot; break;
                case EGameSFX._SFX_SHOTGUN_RELOAD: audioSourceSFX.clip = shotgunReload; break;
                case EGameSFX._SFX_PISTOL_SHOOT: audioSourceSFX.clip = pistolShoot; break;
                case EGameSFX._SFX_PISTOL_RELOAD: audioSourceSFX.clip = pistolReload; break;
                case EGameSFX._SFX_MINIGUN_SHOOT: audioSourceSFX.clip = minigunShoot; break;
                case EGameSFX._SFX_ROCKETLAUNCHER_SHOOT: audioSourceSFX.clip = rocketLauncherShoot; break;
                case EGameSFX._SFX_ROCKETLAUNCHER_RELOAD: audioSourceSFX.clip = rocketLauncherReload; break;
                case EGameSFX._SFX_RAILGUN_SHOOT: audioSourceSFX.clip = railgunShoot; break;
                case EGameSFX._SFX_RAILGUN_RELOAD: audioSourceSFX.clip = railgunReload; break;

                // ENEMY SFX
                case EGameSFX._SFX_GNOME_WALK: audioSourceSFX.clip = gnomeWalk; break;
                case EGameSFX._SFX_GNOME_ATTACK: audioSourceSFX.clip = gnomeAttack; break;
                case EGameSFX._SFX_GNOME_TAKE_DAMAGE: audioSourceSFX.clip = gnomeTakeDamage; break;
                case EGameSFX._SFX_GNOME_DIE: audioSourceSFX.clip = gnomeDie; break;

                // COLLECTIBLE SFX
                case EGameSFX._SFX_COLLECT_AMMO: audioSourceSFX.clip = collectAmmo; break;
                case EGameSFX._SFX_COLLECT_HEALTH: audioSourceSFX.clip = collectHealth; break;

                // UI SFX
                case EGameSFX._SFX_TEXT_TYPE: audioSourceSFX.clip = textType; break;
                case EGameSFX._SFX_UI_SELECT: audioSourceSFX.clip = UISelect; break;

                    #endregion
            }

            SFXAudioPool[SFXAudioPoolIndex].transform.position = pos;
            audioSourceSFX.volume = volume;
            audioSourceSFX.loop = loop;

            if (loop)
            {
                if (loopingAudioList.ContainsKey(intSFX))
                {
                    if (stopLoopRoutine != null)
                    {
                        StopCoroutine(stopLoopRoutine);
                    }
                    loopingAudioList[intSFX].loop = false;
                    loopingAudioList[intSFX].Stop();
                    loopingAudioList.Remove(intSFX);
                }
                loopingAudioList.Add(intSFX, audioSourceSFX);
            }

            if (make2D)
            {
                audioSourceSFX.spatialBlend = 0f;
            }
            else
            {
                audioSourceSFX.spatialBlend = 1f;
            }
            audioSourceSFX.Play();

            // circular buffer
            ++SFXAudioPoolIndex;
            if (SFXAudioPoolIndex >= SFXPoolSize) SFXAudioPoolIndex = 0;
        }

        public void PlayAudio(EGameSFX intSFX)
        {
            //if (!m_bIsSetup) return;
            PlayAudioAtLocation(intSFX, Camera.current.transform.position);
        }

        public void PlayMusic(EGameMusic iTrackIndex, float volume = 0.2f)
        {
            if (null == activeMusicAudioSource) Debug.Log("Music Game Object is missing an audio source component");
            switch (iTrackIndex)
            {
                case EGameMusic._BOBCAT_INGAME: activeMusicAudioSource.clip = level1Music; break;
            }
            if (null == activeMusicAudioSource.clip) Debug.Log("Audio source missing for music");
            activeMusicAudioSource.volume = volume;
            activeMusicAudioSource.Play();
        }

        public void StopMusic()
        {
            if (null == activeMusicAudioSource) Debug.Log("Music Game Object is missing an audio source component");
            activeMusicAudioSource.Stop();
        }

        public System.Collections.IEnumerator FadeMusicOut(float fadeTime, bool bUseUITimer)
        {
            if (null == activeMusicAudioSource) Debug.Log("Music Game Object is missing an audio source component");
            canFadeMusic = true;

            while (activeMusicAudioSource.volume > 0)
            {
                if (!bUseUITimer) activeMusicAudioSource.volume -= Time.deltaTime / fadeTime;
                else activeMusicAudioSource.volume -= Time.deltaTime / fadeTime;

                if (canFadeMusic) yield return null;
                else break;
            }
            StopMusic();
        }

        public void FadeMusicOutInstant()
        {
            canFadeMusic = false;
            StopMusic();
        }

        public void PauseMusic()
        {
            if (null != activeMusicAudioSource) Debug.Log("Music Game Object is missing an audio source component");
            activeMusicAudioSource.Pause();
        }

        public void UnPauseMusic()
        {
            if (null != activeMusicAudioSource) Debug.Log("Music Game Object is missing an audio source component");
            activeMusicAudioSource.UnPause();
        }

        public void MuteMusic(bool bState)
        {
            //if (bState) m_Master.SetFloat("VOL_Music", -80f); else m_Master.SetFloat("VOL_Music", GameGlobals.s_fVOL_Master);
        }

        public void MuteSFX(bool bState)
        {
            //if (bState) m_Master.SetFloat("VOL_SFX", -80f); else m_Master.SetFloat("VOL_SFX", GameGlobals.s_fVOL_Master);
        }

        public void ApplyAudioControl()
        {
            //musicControl.Apply();
            //sfxControl.Apply();
            //masterControl.Apply();
        }

        public void Save(ISaveWriter writer)
        {
            //writer.WriteInt((int)SaveType);
            //musicControl.Save(writer);
            //sfxControl.Save(writer);
            //masterControl.Save(writer);
        }

        public void Load(ISaveReader reader)
        {
            //musicControl.Load(reader);
            //sfxControl.Load(reader);
            //masterControl.Load(reader);
        }
    }
}
