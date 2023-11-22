using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Custom asset menu to create Audio Library assets
[CreateAssetMenu(fileName = "AudioLib", menuName = "Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [SerializeField] private AudioData[] audioList; // List of audio data

    public static List<string> audioNamesList = new List<string>(); // List of audio names
 
    // Get an audio data by name
    public AudioData GetAudioByName(string name)
    {
        foreach (var audio in audioList)
        {
            if (audio.AudioName == name)
                return audio;
        }

        return null; // Return null if the audio clip is not found
    }

    // Called when the script is validated in the Unity Editor
    // This updates the audioNamesList with the names of audio clips in the library
    private void OnValidate()
    {
        audioNamesList.Clear();

        foreach (var audio in audioList)
        {
            audioNamesList.Add(audio.AudioName);
        }
    }

    // Awake method to update the audioNamesList when the script is loaded
    private void Awake()
    {
        OnValidate();
    }
}

// Serializable class to represent an audio clip reference
[System.Serializable]
public class AudioGetter
{
    [SerializeField] private int id; // Index of the audio clip in the audio library

    // Get the audio name by index
    public string AudioName
    {
        get
        {
            if (id >= 0 && id < AudioLibrary.audioNamesList.Count)
            {
                return AudioLibrary.audioNamesList[id];
            }
            else
            {
                return "Invalid Audio"; // Return a message for an invalid audio index
            }
        }
    }
}
