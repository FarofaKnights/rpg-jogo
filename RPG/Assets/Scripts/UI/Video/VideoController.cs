using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour {
    public VideoPlayer videoPlayer;
    public GameObject screen;

    public void Play(string path, bool fadeOut = true) {
        StartCoroutine(PlayAsync(path, fadeOut));
    }

    public void Play(VideoClip clip, bool fadeOut = true) {
        StartCoroutine(PlayAsync(clip, fadeOut));
    }

    public IEnumerator PlayAsync(string path, bool fadeOut = true) {
        VideoClip clip = GameManager.instance.loaded_videoClips.Get(path);
        yield return PlayAsync(clip, fadeOut);
    }

    public IEnumerator PlayAsync(VideoClip clip, bool fadeOut = true) {
        yield return UIController.instance.FadeInAsync();
        
        UIController.dialogo.ShowFalaTexto("");
        UIController.dialogo.gameObject.SetActive(false);
        GameManager.instance.SetCutsceneMode(true);
        GameManager.instance.SetInimigosAtivos(false);


        videoPlayer.clip = clip;
        videoPlayer.Play();
        screen.SetActive(true);

        yield return UIController.instance.FadeOutAsync();

        while (videoPlayer.isPlaying) {
            yield return null;
        }

        yield return UIController.instance.FadeInAsync();

        screen.SetActive(false);

        UIController.dialogo.ShowFalaTexto("");
        UIController.dialogo.gameObject.SetActive(true);
        GameManager.instance.SetCutsceneMode(false);
        GameManager.instance.SetInimigosAtivos(true);
    }
}
