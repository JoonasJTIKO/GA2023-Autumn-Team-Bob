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
    _SFX_PLAYER_RUN_GRASS2,
    _SFX_PLAYER_RUN_GRASS3,
    _SFX_PLAYER_RUN_WOOD,
    _SFX_PLAYER_RUN_WOOD2,
    _SFX_PLAYER_RUN_WOOD3,
    _SFX_PLAYER_RUN_STONE,
    _SFX_PLAYER_RUN_STONE2,
    _SFX_PLAYER_RUN_STONE3,
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
    _SFX_GNOME_WALK_GRASS,
    _SFX_GNOME_WALK_WOOD,
    _SFX_GNOME_WALK_STONE,
    _SFX_GNOME_ATTACK,
    _SFX_GNOME_TAKE_DAMAGE,
    _SFX_GNOME_DIE,

    _SFX_DRAGON_FLY,
    _SFX_DRAGON_ATTACK,
    _SFX_DRAGON_BEAM,
    _SFX_DRAGON_TAKE_DAMAGE,
    _SFX_DRAGON_DIE,

    _SFX_LILGUY_WALK_GRASS,
    _SFX_LILGUY_WALK_WOOD,
    _SFX_LILGUY_WALK_STONE,
    _SFX_LILGUY_ATTACK,
    _SFX_LILGUY_TAKE_DAMAGE,
    _SFX_LILGUY_DIE,

    _SFX_BLOODY_DEATH,

    // COLLECTIBLE SFX
    _SFX_COLLECT_AMMO,
    _SFX_COLLECT_HEALTH,

    // UI SFX
    _SFX_UI_PRESS,
    _SFX_UI_SELECT,

    // NULL OPTION
    _NULL,
}
public enum EGameMusic
{
    _MENU_MUSIC,
    _VILLAGE_MUSIC,
    _TEMPLE_MUSIC
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
        private AudioClip playerRunGrass2;
        [SerializeField]
        private AudioClip playerRunGrass3;
        [SerializeField]
        private AudioClip playerRunWood;
        [SerializeField]
        private AudioClip playerRunWood2;
        [SerializeField]
        private AudioClip playerRunWood3;
        [SerializeField]
        private AudioClip playerRunStone;
        [SerializeField]
        private AudioClip playerRunStone2;
        [SerializeField]
        private AudioClip playerRunStone3;
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
        private AudioClip gnomeWalkGrass;
        [SerializeField]
        private AudioClip gnomeWalkWood;
        [SerializeField]
        private AudioClip gnomeWalkStone;
        [SerializeField]
        private AudioClip gnomeAttack;
        [SerializeField]
        private AudioClip gnomeTakeDamage;
        [SerializeField]
        private AudioClip gnomeDie;
        [SerializeField]
        private AudioClip dragonFly;
        [SerializeField]
        private AudioClip dragonAttack;
        [SerializeField]
        private AudioClip dragonBeam;
        [SerializeField]
        private AudioClip dragonTakeDamage;
        [SerializeField]
        private AudioClip dragonDie;
        [SerializeField]
        private AudioClip lilguyWalkGrass;
        [SerializeField]
        private AudioClip lilguyWalkWood;
        [SerializeField]
        private AudioClip lilguyWalkStone;
        [SerializeField]
        private AudioClip lilguyAttack;
        [SerializeField]
        private AudioClip lilguyTakeDamage;
        [SerializeField]
        private AudioClip lilguyDie;
        [SerializeField]
        private AudioClip bloodyDeath1;
        [SerializeField]
        private AudioClip bloodyDeath2;
        [SerializeField]
        private AudioClip bloodyDeath3;

        [Header("Collectible SFX")]
        [SerializeField]
        private AudioClip collectAmmo;
        [SerializeField]
        private AudioClip collectHealth;

        [Header("UI SFX")]
        [SerializeField]
        private AudioClip UIPress;
        [SerializeField]
        private AudioClip UISelect;

        [Header("Music Clips")]
        [SerializeField] private AudioClip menuMusic = null;
        [SerializeField] private AudioClip villageMusic = null;
        [SerializeField] private AudioClip templeMusic = null;
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
        private List<AudioSource> loopingAudioList = new();

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
        public void StopLoopingAudio(AudioSource audioSource, bool fadeOut = true)
        {
            if (!loopingAudioList.Contains(audioSource)) return;

            if (!fadeOut)
            {
                audioSource.loop = false;
                audioSource.Stop();
                loopingAudioList.Remove(audioSource);
            }
            else
            {
                //if (stopLoopRoutine != null)
                //{
                //    StopCoroutine(stopLoopRoutine);
                //}
                stopLoopRoutine = StartCoroutine(LoopingAudioFadeOut(audioSource));
            }
        }

        private IEnumerator LoopingAudioFadeOut(AudioSource audioSource)
        {
            while (audioSource.volume > 0)
            {
                audioSource.volume -= Time.deltaTime * GameInstance.Instance.GetUpdateManager().timeScale * 3;
                if (audioSource.volume < 0)
                {
                    audioSource.volume = 0;
                }
                yield return null;
            }
            audioSource.loop = false;
            audioSource.Stop();
            loopingAudioList.Remove(audioSource);
        }

        /// <summary>
        /// Grabs a new GameObject from the pool, gives it a new AudioClip and plays it.
        /// </summary>
        /// <param name="intSFX">SFX given as EGameSFX enum</param>
        /// <param name="pos">Vector3 where SFX should be played</param>
        public AudioSource PlayAudioAtLocation(EGameSFX intSFX, Vector3 pos, float volume = 0.5f, bool loop = false, bool make2D = false, bool canStack = false)
        {
            // TODO: check if player prefs setup is done
            // if (!m_bIsSetup) return;

            // check that this effect is not played duplicate in current frame
            // GameInstance clears audioEffectList every frame
            if (audioEffectList.Contains(intSFX) && !canStack) return null;
            else audioEffectList.Add(intSFX);


            AudioSource audioSourceSFX = SFXAudioPool[SFXAudioPoolIndex].GetComponent<AudioSource>();
            audioSourceSFX.transform.parent = null;
            DontDestroyOnLoad(audioSourceSFX);
            audioSourceSFX.Stop();

            switch (intSFX)
            {
                #region SFX switch case using ENUM
                case EGameSFX._SFX_PLAYER_RUN_GRASS: audioSourceSFX.clip = playerRunGrass; break;
                case EGameSFX._SFX_PLAYER_RUN_GRASS2: audioSourceSFX.clip = playerRunGrass2; break;
                case EGameSFX._SFX_PLAYER_RUN_GRASS3: audioSourceSFX.clip = playerRunGrass3; break;
                case EGameSFX._SFX_PLAYER_RUN_WOOD: audioSourceSFX.clip = playerRunWood; break;
                case EGameSFX._SFX_PLAYER_RUN_WOOD2: audioSourceSFX.clip = playerRunWood2; break;
                case EGameSFX._SFX_PLAYER_RUN_WOOD3: audioSourceSFX.clip = playerRunWood3; break;
                case EGameSFX._SFX_PLAYER_RUN_STONE: audioSourceSFX.clip = playerRunStone; break;
                case EGameSFX._SFX_PLAYER_RUN_STONE2: audioSourceSFX.clip = playerRunStone2; break;
                case EGameSFX._SFX_PLAYER_RUN_STONE3: audioSourceSFX.clip = playerRunStone3; break;
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
                case EGameSFX._SFX_GNOME_WALK_GRASS: audioSourceSFX.clip = gnomeWalkGrass; break;
                case EGameSFX._SFX_GNOME_WALK_WOOD: audioSourceSFX.clip = gnomeWalkWood; break;
                case EGameSFX._SFX_GNOME_WALK_STONE: audioSourceSFX.clip = gnomeWalkStone; break;
                case EGameSFX._SFX_GNOME_ATTACK: audioSourceSFX.clip = gnomeAttack; break;
                case EGameSFX._SFX_GNOME_TAKE_DAMAGE: audioSourceSFX.clip = gnomeTakeDamage; break;
                case EGameSFX._SFX_GNOME_DIE: audioSourceSFX.clip = gnomeDie; break;
                case EGameSFX._SFX_DRAGON_FLY: audioSourceSFX.clip = dragonFly; break;
                case EGameSFX._SFX_DRAGON_ATTACK: audioSourceSFX.clip = dragonAttack; break;
                case EGameSFX._SFX_DRAGON_BEAM: audioSourceSFX.clip = dragonBeam; break;
                case EGameSFX._SFX_DRAGON_TAKE_DAMAGE: audioSourceSFX.clip = dragonTakeDamage; break;
                case EGameSFX._SFX_DRAGON_DIE: audioSourceSFX.clip = dragonDie; break;
                case EGameSFX._SFX_LILGUY_WALK_GRASS: audioSourceSFX.clip = lilguyWalkGrass; break;
                case EGameSFX._SFX_LILGUY_WALK_WOOD: audioSourceSFX.clip = lilguyWalkWood; break;
                case EGameSFX._SFX_LILGUY_WALK_STONE: audioSourceSFX.clip = lilguyWalkStone; break;
                case EGameSFX._SFX_LILGUY_ATTACK: audioSourceSFX.clip = lilguyAttack; break;
                case EGameSFX._SFX_LILGUY_TAKE_DAMAGE: audioSourceSFX.clip = lilguyTakeDamage; break;
                case EGameSFX._SFX_LILGUY_DIE: audioSourceSFX.clip = lilguyDie; break;
                case EGameSFX._SFX_BLOODY_DEATH:
                    int random = UnityEngine.Random.Range(0, 3);
                    switch (random)
                    {
                        case 0:
                            audioSourceSFX.clip = bloodyDeath1;
                            break;
                        case 1:
                            audioSourceSFX.clip = bloodyDeath2;
                            break;
                        case 2:
                            audioSourceSFX.clip = bloodyDeath3;
                            break;
                    }
                    break;

                // COLLECTIBLE SFX
                case EGameSFX._SFX_COLLECT_AMMO: audioSourceSFX.clip = collectAmmo; break;
                case EGameSFX._SFX_COLLECT_HEALTH: audioSourceSFX.clip = collectHealth; break;

                // UI SFX
                case EGameSFX._SFX_UI_PRESS: audioSourceSFX.clip = UIPress; break;
                case EGameSFX._SFX_UI_SELECT: audioSourceSFX.clip = UISelect; break;

                    #endregion
            }

            SFXAudioPool[SFXAudioPoolIndex].transform.position = pos;
            audioSourceSFX.volume = volume;
            audioSourceSFX.loop = loop;

            if (loop)
            {
                if (loopingAudioList.Contains(audioSourceSFX))
                {
                    //if (stopLoopRoutine != null)
                    //{
                    //    StopCoroutine(stopLoopRoutine);
                    //}
                    audioSourceSFX.loop = false;
                    audioSourceSFX.Stop();
                    loopingAudioList.Remove(audioSourceSFX);
                }
                loopingAudioList.Add(audioSourceSFX);
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

            return audioSourceSFX;
        }

        public void PlayAudio(EGameSFX intSFX)
        {
            //if (!m_bIsSetup) return;
            PlayAudioAtLocation(intSFX, Camera.current.transform.position);
        }

        public void PlayMusic(EGameMusic iTrackIndex, float volume = 0.1f)
        {
            if (null == activeMusicAudioSource) Debug.Log("Music Game Object is missing an audio source component");
            switch (iTrackIndex)
            {
                case EGameMusic._MENU_MUSIC: activeMusicAudioSource.clip = menuMusic; break;
                case EGameMusic._VILLAGE_MUSIC: activeMusicAudioSource.clip = villageMusic; break;
                case EGameMusic._TEMPLE_MUSIC: activeMusicAudioSource.clip = templeMusic; break;
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
