using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //public Button[] menuItems;
    public GameObject menuCursor;
    public Vector2 cursorOffset;
    bool scaleWithButton;

    public Menu[] menus;

    [Header("Button Behavior")]
    public ButtonSet[] buttonSet;
    Button currentButton;
    bool hasChanged;

    [Header("Song Select")]
    public AudioClip defaultSong;
    public Image selectedCursor;
    string currentSongButton;
    AudioClip currentSong;
    public TextElement playPauseTextElement;
    public TextElement songName;
    public TextElement songDescription;
    string lastScreen;
    string currentScreen;
    //public EventSystem eventSystem;
    Button lastSelect;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip selectSound;
    public AudioClip menuDenySound;
    public AudioClip menuConfirm;
    bool pressedBack;
    bool pressedConfirm;

    [Header("OptionsSliders")]
    Slider currentSlider;
    string sliderName;
    bool sliderSelected;
    bool pressedRight;
    bool pressedLeft;
    // public AudioClip[] soundTrack;
    void Start()
    {
        // lastSelect = new Button();
        // ToggleScreen(menus[0].name);
        // currentScreen = menus[0].name;
        // if (selectedCursor != null)
        //     selectedCursor.tintColor = new Color(1, 1, 1, 0f);

        // //Set song name for each button
        // // for (int i = 0; i < songButtonSet.Length; i++)
        // // {
        // //     songButtonSet[i].songName = 
        // // }
    }

    // Update is called once per frame
    void Update()
    {
        // if (EventSystem.current.currentSelectedGameObject == null)
        // {
        //     EventSystem.current.SetSelectedGameObject(lastSelect);
        // }
        // else
        // {
        //     lastSelect = EventSystem.current.currentSelectedGameObject;
        // }

        //Input
        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Confirm");
            pressedConfirm = true;
            PlaySFX(menuConfirm);
        }
        if (Input.GetButtonUp("Submit"))
            pressedConfirm = false;

        if (Input.GetButtonDown("Cancel"))
        {
            ReturnScreen();
            PlaySFX(menuDenySound);
            pressedBack = true;

        }
        if (Input.GetButtonUp("Cancel"))
        {
            // ReturnScreen();
            // PlaySFX(menuDenySound);
            pressedBack = false;

        }

        //Button Behavior
        if (currentButton != lastSelect)
        {
            currentButton = lastSelect;
            hasChanged = true;
        }

        if (hasChanged)
        {
            if (!pressedBack && !pressedConfirm)
                PlaySFX(selectSound);
            for (int i = 0; i < buttonSet.Length; i++)
            {
                if (currentButton == buttonSet[i].button)
                {
                    buttonSet[i].onHover.Invoke();
                    break;
                }
            }

            hasChanged = false;
        }



        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);

        Vector3 cursorPosition = lastSelect.contentRect.position;

        if (scaleWithButton)
        {
            // menuCursor.gameObject.GetComponent<RectTransform>().sizeDelta = lastSelect.transform.sizeDelta;
            // menuCursor.gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(20, 20);
        }
        // Rect rect = lastSelect.GetComponent<RectTransform>().rect;
        //menuCursor.transform.SetParent(lastSelect.transform);
        // Vector3 cursorPosition2 = new Vector3(rect.xMin, rect.y, cursorPosition.z);
        cursorPosition += new Vector3(cursorOffset.x, cursorOffset.y, 0);
        //menuCursor.transform.position = cursorPosition;
        menuCursor.transform.position = cursorPosition;



        if (Input.GetButtonDown("Start"))
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].name == currentScreen)
                {
                    menus[i].onPressStart.Invoke();
                    break;
                }
            }
        }

        //UnityEngine.UI.Slider behavior
        if (currentSlider != null && sliderSelected)
        {
            if (Input.GetAxisRaw("Horizontal") > 0 && !pressedRight)
            {
                currentSlider.value += 0.1f;

                if (sliderName == "Music")
                    GameplayData.instance.globalMusicVolume = currentSlider.value;
                else
                    GameplayData.instance.globalSFXVolume = currentSlider.value;

                AudioManager.audioManager.SetGlobalFromSaveData();

                pressedRight = true;
            }
            else if (Input.GetAxisRaw("Horizontal") < -0 && !pressedLeft)
            {
                currentSlider.value -= 0.1f;

                if (sliderName == "Music")
                    GameplayData.instance.globalMusicVolume = currentSlider.value;
                else
                    GameplayData.instance.globalSFXVolume = currentSlider.value;

                AudioManager.audioManager.SetGlobalFromSaveData();
                pressedLeft = true;
            }

            if (Input.GetAxis("Horizontal") == 0)
            {
                pressedLeft = false;
                pressedRight = false;
            }
        }
    }


    /***********************************************/
    /***********************************************/
    /*              HELPER FUNCTIONS               */
    /***********************************************/
    /***********************************************/
    public void RestartProgress()
    {
        GameplayData.instance.playerStartLocation = Vector2.zero;
    }


    public void ResetToDefaultAudio()
    {
        // GameplayData.instance.globalSFXVolume = 1;
        // GameplayData.instance.globalMusicVolume = 1;
        // AudioManager.audioManager.SetGlobalFromSaveData();

        // UnityEngine.UI.Slider[] sliders = FindObjectsOfType<UnityEngine.UI.Slider>();

        // foreach (UnityEngine.UI.Slider slider in sliders)
        // {
        //     string[] stringArr = slider.gameObject.name.Split('_');
        //     if (stringArr[0] == "Music")
        //         slider.value = GameplayData.instance.globalMusicVolume;
        //     else
        //         slider.value = GameplayData.instance.globalSFXVolume;
        // }
    }

    public void DeactivateSlider()
    {
        sliderSelected = false;
    }

    // public void SetActiveSlider(UnityEngine.UI.Slider slider)
    // {
    //     currentSlider = slider;
    //     sliderSelected = true;


    //     string[] stringArr = slider.gameObject.name.Split('_');
    //     sliderName = stringArr[0];


    // }
    public void ReturnScreen()
    {
        // DeactivateSlider();
        // if (lastScreen != null)
        //     ToggleScreen(lastScreen);
    }

    public void ButtonTest()
    {
        Debug.Log("Test");
    }

    public void ToggleSongName(string _songName)
    {
        songName.text = _songName;

        Debug.Log(currentButton.name);

        if (musicSource != null)
        {
            if (currentSongButton == currentButton.name)
            {
                if (musicSource.isPlaying)
                    playPauseTextElement.text = "Pause";
                else
                    playPauseTextElement.text = "Resume";
            }
            else
                playPauseTextElement.text = "Play";
        }
    }


    public void ToggleSongDescription(string _songDescription)
    {
        songDescription.text = _songDescription;

    }

    public void SetDescriptionColor(Image button)
    {
        songDescription.style.color = button.tintColor;
    }

    // public void ToggleScreen(string name)
    // {
    //     for (int i = 0; i < menus.Length; i++)
    //     {
    //         if (menus[i].name == name)
    //         {
    //             menus[i].screen.SetActive(true);
    //             menuCursor = menus[i].cursor;
    //             cursorOffset = menus[i].cursorOffset;
    //             EventSystem.current.SetSelectedGameObject(menus[i].firstSelectButton);
    //             scaleWithButton = menus[i].scaleCursorOnButton;
    //             //selectedCursor = menus[i].selectedCursor;

    //             if (menus[i].songName != null)
    //             {
    //                 songName = menus[i].songName;
    //                 songDescription = menus[i].songDescription;
    //             }

    //             if (i > 0)
    //             {
    //                 lastScreen = currentScreen;
    //                 currentScreen = menus[i].name;
    //             }
    //             else
    //             {
    //                 if (selectedCursor != null)
    //                     selectedCursor.tintColor = new Color(1, 1, 1, 0f);
    //                 currentScreen = menus[i].name;
    //                 lastScreen = null;
    //             }
    //         }
    //         else
    //         {
    //             menus[i].screen.SetActive(false);
    //         }
    //     }
    // }

    // public void SetSongName(AudioClip song) {

    // }

    public void PlaySFX(AudioClip sound)
    {
        if (sfxSource != null)
        {
            sfxSource.clip = sound;
            sfxSource.Play();
        }
        else
        {
            Sound audio = new Sound();
            audio.name = sound.name;
            audio.clip = sound;
            audio.channelType = AudioChannelType.monoSFX;
            audio.volume = 1;
            audio.loop = false;
            AudioManager.audioManager.PlaySound(audio);
        }
    }

    public void PlayMusic(AudioClip song)
    {
        if (currentButton != null)
            currentSongButton = currentButton.name;
        currentSong = song;

        if (musicSource != null)
        {
            if (musicSource.isPlaying)
            {
                if (musicSource.clip != song)
                {
                    musicSource.Stop();
                    musicSource.clip = song;
                    musicSource.Play();
                }
                else
                {
                    musicSource.Pause();
                }
            }
            else
            {
                musicSource.clip = song;
                musicSource.Play();
            }
        }
        else
        {
            Sound audio = new Sound();
            audio.name = song.name;
            audio.clip = song;
            audio.channelType = AudioChannelType.music;
            audio.volume = 1;
            audio.loop = true;
            AudioManager.audioManager.PlayMusic(audio);
        }


        if (selectedCursor != null && currentButton != null)
        {
            if (selectedCursor.tintColor.a == 0)
                selectedCursor.tintColor = new Color(1, 1, 1, 1f);


            selectedCursor.transform.position = currentButton.transform.position;
        }

        if (musicSource != null)
        {

            if (currentSongButton == currentButton.name)
            {
                if (musicSource.isPlaying)
                    playPauseTextElement.text = "Pause";
                else
                    playPauseTextElement.text = "Resume";
            }
            else
                playPauseTextElement.text = "Play";
        }
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void StartGame()
    {
        Debug.Log("Start!");
        if (currentSong == null)
            currentSong = defaultSong;
        GameplayData.instance.SetSong(currentSong);
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();

        //UnityEditor.EditorApplication.isPlaying = false;
    }
}


[System.Serializable]
public class Menu
{
    public string name;
    public GameObject screen;
    public TextElement songName;
    public TextElement songDescription;

    public GameObject cursor;
    public bool scaleCursorOnButton;
    public GameObject selectedCursor;

    public Vector2 cursorOffset;
    public GameObject firstSelectButton;
    public UnityEvent onPressStart;
}

[System.Serializable]
public class ButtonSet
{
    public UnityEvent onHover;
    public Button button;
}

// [System.Serializable]
// public class SongButtonSet : ButtonSet
// {
//     [HideInInspector]
//     public string songName;
// }