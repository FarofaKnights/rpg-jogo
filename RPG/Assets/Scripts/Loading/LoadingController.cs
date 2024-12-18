using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController {
    public System.Action<float> onProgress;
    public System.Action onLoaded;

    bool isLoading = false;
    int steps = 0;
    int maxSteps = 0;
    float pesoCena = 0.6f;

    public bool IsLoading { get { return isLoading; } }
    public LevelInfo[] levels = null;
    Dictionary<string, GameObject> loadedPrefabs = new Dictionary<string, GameObject>();

    public LoadingController() {
        LoadLevelsInfo();
    }

    public void LoadLevelsInfo() {
        levels = Resources.LoadAll<LevelInfo>("Levels");
    }

    public LevelInfo[] GetLevels() {
        if (levels == null) {
            LoadLevelsInfo();
        }

        return levels;
    }

    protected Coroutine StartCoroutine(IEnumerator routine) {
        return GameManager.instance.StartCoroutine(routine);
    }

    public LevelInfo GetLevelInfo(string levelName) {
        return System.Array.Find(levels, level => level.nomeCena == levelName);
    }

    public LevelInfo GetLevelInfoByName(string levelName) {
        return System.Array.Find(levels, level => level.name == levelName);
    }

    public void LoadScene(string sceneName) {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public IEnumerator LoadSceneAsync(string sceneName) {
        yield return LoadSceneAsync(GetLevelInfo(sceneName));
    }

    public void LoadScene(LevelInfo level) {
        StartCoroutine(LoadSceneAsync(level));
    }

    GameObject GetParent(string nome) {
        GameObject[] easterEggs = GameObject.FindGameObjectsWithTag("EasterEgg");
        foreach (GameObject easterEgg in easterEggs) {
            if (easterEgg.name == nome) {
                return easterEgg;
            }
        }

        return null;
    }

    public int CalculateSteps(LevelInfo level, System.Action[] onLoaded = null) {
        int steps = 0;

        if (level.prefabsAInstanciar != null) {
            steps += level.prefabsAInstanciar.Length;
        }

        if (level.audioClipPaths != null && level.audioClipPaths.Length > 0) {
            steps += level.audioClipPaths.Length;
        }

        if (level.videoClipPaths != null && level.videoClipPaths.Length > 0) {
            steps += level.videoClipPaths.Length;
        }

        if (onLoaded != null) {
            steps += onLoaded.Length;
        }

        return steps;
    }

    public void SetPercentage(float percentage) {
        onProgress?.Invoke(percentage);
    }


    public void IncrementStep() {
        steps++;
        SetPercentage((1.0f * steps / maxSteps) * (1-pesoCena) + pesoCena);
    }

    
    public IEnumerator LoadSceneAsync(LevelInfo level, System.Action[] onBeforeFinish = null) {
        isLoading = true;
        yield return GameManager.instance.loadingUI.StartLoadingAsync();

        maxSteps = CalculateSteps(level, onBeforeFinish);
        steps = 0;

        var asyncLoadLevel = SceneManager.LoadSceneAsync(level.nomeCena, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone){
            SetPercentage(asyncLoadLevel.progress * pesoCena);
            yield return null;
        }

        SetPercentage(pesoCena);
        
        // Se tiver algum prefab pra instanciar
        if (level.prefabsAInstanciar != null) {
            foreach (RelacaoPrefabELocal relacao in level.prefabsAInstanciar) {
                GameObject easterEgg = GetParent(relacao.local);
                if (easterEgg == null) {
                    IncrementStep();
                    continue;
                }

                GameObject prefab;

                // Se não tiver nenhum prefab cacheado, carrega ele assincronamente
                if (!loadedPrefabs.ContainsKey(relacao.prefabPath)) {
                    var load = Resources.LoadAsync<GameObject>(relacao.prefabPath);
                    while (!load.isDone) {
                        yield return null;
                    }

                    prefab = load.asset as GameObject;
                    loadedPrefabs.Add(relacao.prefabPath, prefab);
                } else {
                    // Se tiver cacheado, usa o cache
                    prefab = loadedPrefabs[relacao.prefabPath];
                }

                GameObject instance = GameObject.Instantiate(prefab, easterEgg.transform);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;

                IncrementStep();
            }

            // Limpa o cache
            loadedPrefabs.Clear();
        }

        // Se tiver algum audio pra carregar
        if (level.audioClipPaths != null && level.audioClipPaths.Length > 0) {
            foreach (string path in level.audioClipPaths) {
                yield return GameManager.instance.loaded_audioClips.LoadAsync(path);

                IncrementStep();
            }
        }

        // Se tiver algum video pra carregar
        if (level.videoClipPaths != null && level.videoClipPaths.Length > 0) {
            foreach (string path in level.videoClipPaths) {
                yield return GameManager.instance.loaded_videoClips.LoadAsync(path);

                IncrementStep();
            }
        }

        // Se tiver alguma ação pra executar
        if (onBeforeFinish != null) {
            foreach (System.Action action in onBeforeFinish) {
                action?.Invoke();
                IncrementStep();
            }
        }

        SetPercentage(1.0f);

        isLoading = false;
        yield return GameManager.instance.loadingUI.LoadingEndedAsync();
        
        onLoaded?.Invoke();
    }
}
