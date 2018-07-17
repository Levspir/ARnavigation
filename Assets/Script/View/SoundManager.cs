using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public string[] audioFileToLoadName = {
		"00 silence",
        "01 init_app", // ส่องกล้องไปยังจุดต่างๆตามอาคาร เพื่อระบุตำแหน่งของคุณ
		"02 init_fornav", // ส่องกล้องไปยังจุดต่างๆตามอาคาร เพื่อระบุตำแหน่ง และเริ่มต้นการนำทาง
        "03 located", // ระบุตำแหน่งแล้ว
		"04 start_nav", // เริ่มต้นการนำทางแล้ว
		"08 cancle_nav", // ยกเลิกการนำทางแล้ว
		"09 reached", // ถึงปลายทางแล้ว
		"10 end_nav", // สิ้นสุดการนำทาง
		"11 turn_right", // เลี้ยวขวา
		"12 turn_left", // เลี้ยวซ้าย
		"13 go_back", // ไปด้านหลัง
		"14 go_ahead", // ตรงไปด้านหน้า
		"15 upstairs", // ขึ้นชั้นบน
		"16 downstairs" // ลงชั้นล่าง
		};

    public enum SoundType
    {
		Silence,
        InitApp,
        InitFornav,
        Located,
        StartNav,
        CancleNav,
        Reached,
        EndNav,
        TurnRight,
        TurnLeft,
        GoBack,
        GoAhead,
        Upstairs,
        Downstairs
    }

    private AudioClip[] audioClipList;
    AudioSource audioSource;
    public List<int> soundQueue;
    public int playindex = 0;

    // Use this for initialization
    void Start()
    {
        if (audioSource == null)
        {
            InitSoundManager();
        }
    }

    void Awake()
    {
        if (audioSource == null)
        {
            InitSoundManager();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (soundQueue != null)
        {
            if (!audioSource.isPlaying && playindex <= soundQueue.Count - 1)
            {
				Debug.Log("playing index " + playindex + " : " + audioClipList[soundQueue[playindex]]);
                audioSource.Stop();
                audioSource.clip = audioClipList[soundQueue[playindex]];
				playindex += 1;
                audioSource.Play();
                
            }
        }

    }

    /* init and load sound to list */
    private void InitSoundManager()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        soundQueue = new List<int>();
        soundQueue = null;
        audioClipList = new AudioClip[audioFileToLoadName.Length];
		audioClipList[0] = null; //set to silence
        for (int i = 0; i < audioFileToLoadName.Length; i++)
        {
            audioClipList[i] = (AudioClip)Resources.Load("Sounds/" + audioFileToLoadName[i]);
        }
		Debug.Log("InitSoundManager audi lenght" + audioClipList.Length);
    }

    public void Play(SoundType soundtype)
    {
        if (audioSource == null)
        {
            InitSoundManager();
        }
        AudioClip clip = audioClipList[(int)soundtype];
        audioSource.PlayOneShot(clip);
    }

    /*stop currenly playing and play audio in queue */
    public void PlayQueue(List<int> newsoundqueue)
    {
        audioSource.Stop();
        this.soundQueue = newsoundqueue;
        playindex = 0;
    }
}
