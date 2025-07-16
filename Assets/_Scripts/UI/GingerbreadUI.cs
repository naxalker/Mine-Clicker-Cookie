using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class GingerbreadUI : MonoBehaviour, IPointerClickHandler
{
    public Action OnClicked;

    private const float IDLE_ROTATION_ANGLE = 7.5f;
    private const float IDLE_ROTATION_DURATION = 3f;

    private const float CLICK_SCALE_DOWN = 0.92f;
    private const float CLICK_SCALE_UP = 1.08f;
    private const float CLICK_DURATION = 0.5f;

    [SerializeField] private TMP_Text _incomeTextPrefab;
    [SerializeField] private ParticleSystem _clickParticles;
    [SerializeField] private RectTransform _imageRectTransform;

    private RectTransform _rectTransform;
    private BalanceManager _balanceManager;

    private Sequence _currentClickSequence;

    [Inject]
    private void Construct(BalanceManager balanceManager)
    {
        _balanceManager = balanceManager;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        StartIdleAnimation();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AnimateClick();
        SpawnIncomeText();
        SpawnParticles();

        OnClicked?.Invoke();
    }

    private void AnimateClick()
    {
        if (_currentClickSequence != null)
        {
            _currentClickSequence.Kill();
        }

        _currentClickSequence = DOTween.Sequence();

        _currentClickSequence.Append(_imageRectTransform
            .DOScale(CLICK_SCALE_DOWN, CLICK_DURATION * 0.3f)
            .From(Mathf.Min(_imageRectTransform.localScale.x + .03f, 1f), false)
            .SetEase(Ease.InOutSine));

        _currentClickSequence.Append(_imageRectTransform
            .DOScale(CLICK_SCALE_UP, CLICK_DURATION * 0.35f)
            .SetEase(Ease.InOutSine));

        _currentClickSequence.Append(_imageRectTransform
            .DOScale(1f, CLICK_DURATION * 0.35f)
            .SetEase(Ease.InOutSine));
    }

    private void StartIdleAnimation()
    {
        _imageRectTransform.DORotate(new Vector3(0, 0, IDLE_ROTATION_ANGLE), IDLE_ROTATION_DURATION)
            .From(new Vector3(0, 0, -IDLE_ROTATION_ANGLE))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void SpawnIncomeText()
    {
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0;

        TMP_Text incomeText = Instantiate(
            _incomeTextPrefab,
            spawnPosition,
            Quaternion.identity,
            _rectTransform.parent);

        incomeText.text = $"+{DoubleUtilities.ToCustomScientificNotation(_balanceManager.ClickIncome)}";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(incomeText.rectTransform.DOAnchorPosY(incomeText.rectTransform.anchoredPosition.y + 40f, .7f).SetEase(Ease.Linear));
        sequence.Join(incomeText.DOFade(0f, .3f).SetDelay(.4f));
        sequence.OnComplete(() => Destroy(incomeText.gameObject));
    }

    private void SpawnParticles()
    {
        Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spawnPosition.z = 0;

        Instantiate(_clickParticles, spawnPosition, Quaternion.identity, _rectTransform);
    }
}
