using UnityEngine;
using UnityEngine.Audio;
using NUnit.Framework;

// Tests for the AudioManager's audio control functionality
public class AudioTest : MonoBehaviour
{
    // Tests that clicking the audio off button sets master volume to -80dB
    [Test]
    public void Audio_Off_Sets_MasterVolume_To_Negative80()
    {
        GameObject obj = new GameObject();
        AudioManager manager = obj.AddComponent<AudioManager>();

        AudioMixer mixer = Resources.Load<AudioMixer>("TestMixer");
        manager.masterMixer = mixer;

        obj.tag = "AudioOff";

        manager.SendMessage("OnMouseDown");

        mixer.GetFloat("MasterVolume", out float value);

        Assert.AreEqual(-80f, value);
    }

    // Tests that clicking the audio on button sets master volume to 0dB
    [Test]
    public void SetAudio_DisablesVolume()
    {
        AudioManager manager = new GameObject().AddComponent<AudioManager>();
        AudioMixer mixer = Resources.Load<AudioMixer>("TestMixer");
        manager.masterMixer = mixer;

        manager.SetAudio(false);

        mixer.GetFloat("MasterVolume", out float value);

        Assert.AreEqual(-80f, value);
    }
}
