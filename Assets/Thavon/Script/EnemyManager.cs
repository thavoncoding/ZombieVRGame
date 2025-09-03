using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 3f;
    public LayerMask playerLayer;
    public int spawnLimit = 5;

    [Header("UI")]
    public Button toggleButton;
    public TextMeshProUGUI buttonText;

    private float timer = 0f;
    private bool isSpawning = false;
    private Transform player;

    private List<GameObject> activeEnemies = new List<GameObject>();

    [Header("Lighting")]
    public Light directionalLight;
    public Color nightColor = Color.black;
    private Color originalLightColor;
    public float transitionDuration = 1f;

    private Coroutine lightTransitionCoroutine;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    void Start()
    {
        player = FindPlayerByLayer(playerLayer);

        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleSpawning);

        if (directionalLight != null)
        {
            originalLightColor = directionalLight.color;
        }

        UpdateButtonText();
    }

    void Update()
    {
        CleanUpDeadEnemies();

        if (!isSpawning || activeEnemies.Count >= spawnLimit) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0 || player == null || activeEnemies.Count >= spawnLimit) return;

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, spawn.rotation);

        var follow = enemy.GetComponent<EnemyWander>();
        if (follow != null)
        {
            follow.target = player;
        }

        activeEnemies.Add(enemy);
    }

    void CleanUpDeadEnemies()
    {
        // Remove any enemies that were destroyed
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    public void ToggleSpawning()
    {
        isSpawning = !isSpawning;

        // Play or pause music
        if (backgroundMusic != null)
        {
            if (isSpawning)
                backgroundMusic.Play();
            else
                backgroundMusic.Pause();
        }

        // Light transition
        if (directionalLight != null)
        {
            Color targetColor = isSpawning ? nightColor : originalLightColor;

            if (lightTransitionCoroutine != null)
                StopCoroutine(lightTransitionCoroutine);

            lightTransitionCoroutine = StartCoroutine(
                TransitionLightColor(directionalLight, directionalLight.color, targetColor, transitionDuration)
            );
        }

        // Destroy all enemies when stopping
        if (!isSpawning)
        {
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    EnemyWander ew = enemy.GetComponent<EnemyWander>();
                    if (ew != null)
                    {
                        ew.Die();
                    }
                    else
                    {
                        Destroy(enemy); // Fallback
                    }
                }
            }

            activeEnemies.Clear();

        }

        UpdateButtonText();
    }

    private IEnumerator TransitionLightColor(Light light, Color fromColor, Color toColor, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            light.color = Color.Lerp(fromColor, toColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        light.color = toColor;
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = isSpawning ? "Stop Spawning" : "Start Spawning";
        }
    }

    Transform FindPlayerByLayer(LayerMask layerMask)
    {
        GameObject[] all = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in all)
        {
            if (((1 << obj.layer) & layerMask) != 0)
                return obj.transform;
        }
        return null;
    }

}
