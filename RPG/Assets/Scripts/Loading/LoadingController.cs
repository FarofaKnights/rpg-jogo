using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController {
    public System.Action<float> onProgress;
    public System.Action onLoaded;

    bool isLoading = false;
    public bool IsLoading { get { return isLoading; } }
    public LevelInfo[] levels = null;
    Dictionary<string, GameObject> loadedPrefabs = new Dictionary<string, GameObject>();

    public LoadingController() {
        LoadLevelsInfo();
    }

    public void LoadLevelsInfo() {
        levels = Resources.LoadAll<LevelInfo>("Levels");
    }

    protected Coroutine StartCoroutine(IEnumerator routine) {
        return GameManager.instance.StartCoroutine(routine);
    }

    public LevelInfo GetLevelInfo(string levelName) {
        return System.Array.Find(levels, level => level.nomeCena == levelName);
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

    public IEnumerator LoadSceneAsync(LevelInfo level) {
        isLoading = true;
        GameManager.instance.loadingUI.StartLoading();
        
        var asyncLoadLevel = SceneManager.LoadSceneAsync(level.nomeCena, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone){
            yield return null;
        }
        
        // Se tiver algum prefab pra instanciar
        if (level.prefabsAInstanciar != null) {
            foreach (RelacaoPrefabELocal relacao in level.prefabsAInstanciar) {
                GameObject easterEgg = GetParent(relacao.local);
                if (easterEgg == null) continue;

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
            }

            // Limpa o cache
            loadedPrefabs.Clear();
        }

        isLoading = false;
        GameManager.instance.loadingUI.LoadingEnded();
        
        onLoaded?.Invoke();
    }
}
