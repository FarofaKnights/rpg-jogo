using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheLoader<T> where T : Object {
    public string nome = "";
    public Dictionary<string, T> clips = new Dictionary<string, T>();

    public CacheLoader(string nome) {
        this.nome = nome;
    }
    
    public IEnumerator LoadAsync(string path) {
        if (clips.ContainsKey(path)) {
            yield break;
        }

        ResourceRequest request = Resources.LoadAsync<T>(path);
        yield return request;

        if (request.asset == null) {
            Debug.LogError(nome + " não encontrado: " + path);
            yield break;
        }

        clips[path] = request.asset as T;
    }

    public void Load(string path) {
        if (clips.ContainsKey(path)) {
            return;
        }

        T clip = Resources.Load<T>(path);
        if (clip == null) {
            Debug.LogError(nome + " não encontrado: " + path);
            return;
        }

        clips[path] = clip;
    }

    public T Get(string path) {
        if (clips.ContainsKey(path)) {
            return clips[path];
        }

        Load(path);
        return clips[path];
    }

    public IEnumerator GetAsync(string path, System.Action<T> callback) {
        if (!clips.ContainsKey(path)) {
            yield return LoadAsync(path);
        }

        callback(clips[path]);
    }
}
