using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class GameOverHandler : MonoBehaviour, IPointerClickHandler
{
    public static Action OnGameOverScreenShown;

    [SerializeField] private ParticleSystem _confettiEffect;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private AudioClip _confettiClip;
    [SerializeField] private AudioClip _victoryClip;

    private bool _canBeHidden;
    private bool _isSubscribed;

    private UpgradeManager _upgradeManager;

    [Inject]
    private void Construct(UpgradeManager upgradeManager)
    {
        if (SavesManager.GameOverScreenShown())
        {
            Destroy(gameObject);
        }
        else
        {
            _upgradeManager = upgradeManager;
            Upgrade.OnUpgradeLevelIncreased += LevelIncreasedHandler;
            _isSubscribed = true;
        }
    }

    private void OnDestroy()
    {
        if (_isSubscribed)
        {
            Upgrade.OnUpgradeLevelIncreased -= LevelIncreasedHandler;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerEnter == gameObject && _canBeHidden)
        {
            Destroy(gameObject);
        }
    }

    private void LevelIncreasedHandler(Upgrade upgrade)
    {
        if (_upgradeManager.Upgrades.All(upgrade => upgrade.Level > 0))
        {
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        gameObject.SetActive(true);

        OnGameOverScreenShown?.Invoke();

        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            _confettiEffect.Play();
            AudioSource.PlayClipAtPoint(_confettiClip, Camera.main.transform.position, .15f);
            AudioSource.PlayClipAtPoint(_victoryClip, Camera.main.transform.position, .15f);
        });
        sequence.AppendInterval(2f);
        sequence.Append(GetComponent<Image>().DOFade(.6f, .5f).From(0f));
        sequence.Join(_rectTransform.DOScale(1f, .5f).From(0f).SetEase(Ease.InSine));
        sequence.AppendInterval(3f);
        sequence.AppendCallback(() => _canBeHidden = true);
    }
}
