using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour {
    public VideoPlayer videoPlayer;
    public GameObject screen;

    public void Play(string path) {
        StartCoroutine(PlayAsync(path));
    }

    public void Play(VideoClip clip) {
        StartCoroutine(PlayAsync(clip));
    }

    public IEnumerator PlayAsync(string path) {
        VideoClip clip = GameManager.instance.loaded_videoClips.Get(path);
        yield return PlayAsync(clip);
    }

    public IEnumerator PlayAsync(VideoClip clip) {
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

        screen.SetActive(false);

        UIController.dialogo.ShowFalaTexto("");
        UIController.dialogo.gameObject.SetActive(true);
        GameManager.instance.SetCutsceneMode(false);
        GameManager.instance.SetInimigosAtivos(true);
    }
}
