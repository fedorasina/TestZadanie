using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MoneyCollector : MonoBehaviour
{
    [Header("Money UI")]
    [SerializeField] private Slider moneySlider;

    [Header("Money")]
    [SerializeField] private int maxMoney = 100;
    [SerializeField] private int moneyPerPickup = 10;
    [SerializeField] private float moneyMultiplier = 1f;

    [Header("Enemy")]
    [SerializeField] private int moneyLostFromEnemy = 20;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int scorePerMoney = 1;
    [SerializeField] private int scoreLostFromEnemy = 10;

    [Header("Score Mesh Change")]
    [SerializeField] private int scoreToChangeMesh = 100;
    [SerializeField] private GameObject targetMeshPrefab;

    [Header("Money Popups")]
    [SerializeField] private GameObject moneyPopupPrefab;
    [SerializeField] private GameObject moneyMinusPopupPrefab;
    [SerializeField] private Transform popupParent;
    [SerializeField] private float popupMoveDistance = 80f;
    [SerializeField] private float popupDuration = 1.2f;

    [Header("Collect Effect")]
    [SerializeField] private ParticleSystem collectParticle;
    [SerializeField] private AudioClip collectSound;

    [Header("Enemy Effect")]
    [SerializeField] private ParticleSystem enemyParticle;
    [SerializeField] private AudioClip enemySound;

    [Header("Animation")]
    [SerializeField] private AnimationCurve moveCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField] private AnimationCurve fadeCurve =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private int currentMoney;
    private int score;

    private bool meshChanged;

    private GameObject activePlusPopup;
    private Coroutine plusCoroutine;
    private int plusAmount;

    private GameObject activeMinusPopup;
    private Coroutine minusCoroutine;
    private int minusAmount;

    private void Start()
    {
        currentMoney = 0;

        if (moneySlider != null)
        {
            moneySlider.minValue = 0;
            moneySlider.maxValue = maxMoney;
            moneySlider.value = currentMoney;
        }

        score = 0;

        if (scoreText != null)
        {
            if (!int.TryParse(scoreText.text, out score))
                score = 0;
        }

        UpdateScoreUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            CollectMoney(other.gameObject);
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            HitEnemy(other.gameObject);
        }
    }

    public void AddReward(int moneyAmount, int scoreAmount)
    {
        currentMoney += moneyAmount;

        currentMoney = Mathf.Clamp(
            currentMoney,
            0,
            maxMoney
        );

        score += scoreAmount;

        UpdateMoneyUI();
        UpdateScoreUI();

        CheckScoreMesh();

        if (moneyAmount > 0)
        {
            ShowPlusPopup(moneyAmount);
        }
        else if (moneyAmount < 0)
        {
            ShowMinusPopup(Mathf.Abs(moneyAmount));
        }
    }

    public void SetMoneyMultiplier(float multiplier)
    {
        moneyMultiplier = Mathf.Max(0f, multiplier);
    }

    private void CollectMoney(GameObject money)
    {
        int reward = Mathf.RoundToInt(
            moneyPerPickup * moneyMultiplier
        );

        currentMoney += reward;

        currentMoney = Mathf.Clamp(
            currentMoney,
            0,
            maxMoney
        );

        UpdateMoneyUI();

        score += scorePerMoney;

        UpdateScoreUI();

        CheckScoreMesh();

        ShowPlusPopup(reward);

        PlayCollectEffect();

        Destroy(money);
    }

    private void HitEnemy(GameObject enemy)
    {
        currentMoney -= moneyLostFromEnemy;

        currentMoney = Mathf.Clamp(
            currentMoney,
            0,
            maxMoney
        );

        UpdateMoneyUI();

        score -= scoreLostFromEnemy;

        UpdateScoreUI();

        ShowMinusPopup(moneyLostFromEnemy);

        PlayEnemyEffect();

        Destroy(enemy);
    }

    private void CheckScoreMesh()
    {
        if (meshChanged)
            return;

        if (score < scoreToChangeMesh)
            return;

        if (targetMeshPrefab == null)
            return;

        MeshFilter targetMesh =
            targetMeshPrefab.GetComponentInChildren<MeshFilter>();

        if (targetMesh == null)
            return;

        MeshFilter playerMesh =
            GetComponentInChildren<MeshFilter>();

        if (playerMesh == null)
            return;

        playerMesh.sharedMesh =
            targetMesh.sharedMesh;

        Renderer playerRenderer =
            playerMesh.GetComponent<Renderer>();

        Renderer targetRenderer =
            targetMesh.GetComponent<Renderer>();

        if (playerRenderer != null &&
            targetRenderer != null)
        {
            playerRenderer.sharedMaterials =
                targetRenderer.sharedMaterials;
        }

        SkinnedMeshRenderer playerSkinned =
            GetComponentInChildren<SkinnedMeshRenderer>();

        SkinnedMeshRenderer targetSkinned =
            targetMeshPrefab
                .GetComponentInChildren<SkinnedMeshRenderer>();

        if (playerSkinned != null &&
            targetSkinned != null)
        {
            playerSkinned.sharedMesh =
                targetSkinned.sharedMesh;

            playerSkinned.sharedMaterials =
                targetSkinned.sharedMaterials;
        }

        meshChanged = true;
    }

    private void UpdateMoneyUI()
    {
        if (moneySlider != null)
        {
            moneySlider.value = currentMoney;
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void ShowPlusPopup(int amount)
    {
        if (moneyPopupPrefab == null ||
            popupParent == null)
            return;

        if (activePlusPopup != null)
        {
            plusAmount += amount;

            UpdatePopupText(
                activePlusPopup,
                "+" + plusAmount
            );

            return;
        }

        activePlusPopup =
            Instantiate(
                moneyPopupPrefab,
                popupParent
            );

        activePlusPopup.SetActive(true);

        plusAmount = amount;

        UpdatePopupText(
            activePlusPopup,
            "+" + plusAmount
        );

        plusCoroutine =
            StartCoroutine(
                AnimatePopup(
                    activePlusPopup,
                    true
                )
            );
    }

    private void ShowMinusPopup(int amount)
    {
        if (moneyMinusPopupPrefab == null ||
            popupParent == null)
            return;

        if (activeMinusPopup != null)
        {
            minusAmount += amount;

            UpdatePopupText(
                activeMinusPopup,
                "-" + minusAmount
            );

            return;
        }

        activeMinusPopup =
            Instantiate(
                moneyMinusPopupPrefab,
                popupParent
            );

        activeMinusPopup.SetActive(true);

        minusAmount = amount;

        UpdatePopupText(
            activeMinusPopup,
            "-" + minusAmount
        );

        minusCoroutine =
            StartCoroutine(
                AnimatePopup(
                    activeMinusPopup,
                    false
                )
            );
    }

    private void UpdatePopupText(
        GameObject popup,
        string text)
    {
        TextMeshProUGUI tmp =
            popup.GetComponentInChildren<
                TextMeshProUGUI>();

        if (tmp != null)
        {
            tmp.text = text;
        }
    }

    private IEnumerator AnimatePopup(
        GameObject popup,
        bool isPlus)
    {
        if (popup == null)
            yield break;

        RectTransform rect =
            popup.GetComponent<RectTransform>();

        if (rect == null)
            yield break;

        CanvasGroup canvasGroup =
            popup.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                popup.AddComponent<CanvasGroup>();
        }

        Vector2 startPosition =
            rect.anchoredPosition;

        Vector2 endPosition =
            startPosition +
            Vector2.up * popupMoveDistance;

        canvasGroup.alpha = 1f;

        float timer = 0f;

        while (timer < popupDuration)
        {
            if (popup == null)
                yield break;

            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer / popupDuration
                );

            float moveT =
                moveCurve.Evaluate(t);

            rect.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    endPosition,
                    moveT
                );

            float fadeT =
                fadeCurve.Evaluate(t);

            canvasGroup.alpha = fadeT;

            yield return null;
        }

        if (popup != null)
        {
            canvasGroup.alpha = 0f;

            Destroy(popup);
        }

        if (isPlus)
        {
            activePlusPopup = null;
            plusCoroutine = null;
            plusAmount = 0;
        }
        else
        {
            activeMinusPopup = null;
            minusCoroutine = null;
            minusAmount = 0;
        }
    }

    private void PlayCollectEffect()
    {
        if (collectParticle != null)
        {
            ParticleSystem particles =
                Instantiate(
                    collectParticle,
                    transform.position,
                    Quaternion.identity
                );

            particles.Play();

            float destroyTime =
                particles.main.duration +
                particles.main.startLifetime.constantMax +
                0.2f;

            Destroy(
                particles.gameObject,
                destroyTime
            );
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                transform.position
            );
        }
    }

    private void PlayEnemyEffect()
    {
        if (enemyParticle != null)
        {
            ParticleSystem particles =
                Instantiate(
                    enemyParticle,
                    transform.position,
                    Quaternion.identity
                );

            particles.Play();

            float destroyTime =
                particles.main.duration +
                particles.main.startLifetime.constantMax +
                0.2f;

            Destroy(
                particles.gameObject,
                destroyTime
            );
        }

        if (enemySound != null)
        {
            AudioSource.PlayClipAtPoint(
                enemySound,
                transform.position
            );
        }
    }
}