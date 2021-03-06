using BlackJack.Manager;
using DG.Tweening;
using System;
using System.Collections;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BlackJack.View
{
    /// <summary>
    /// ゲーム進行に関する入力を受け付けるView
    /// </summary>
    public class InputView : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// ゲームの開始のイベントを発行する
        /// 監視可能
        /// int -> ベット金額
        /// </summary>
        public IObservable<int> ObservableGameStart => _onStartButton;

        /// <summary>
        /// Hitのイベントを発行する
        /// </summary>
        public IObservable<string> ObservableHitButton => _onHitButton;

        /// <summary>
        /// Stayのイベントを発行する
        /// </summary>
        public IObservable<string> ObservableStayButton => _onStayButton;

        #endregion

        #region Inspector Variables

        [SerializeField]
        [Header("ゲーム終了後にボタンを押せるようになるまでの時間(ミリ秒)")]
        private int _timeToStartButtonSelectable = 3000;

        [SerializeField]
        [Header("アクションのボタンを選択後から押せるようになるまでの時間(ミリ秒)")]
        private int _timeToSelectableActionButton = 1000;

        [SerializeField]
        [Header("通知のテキストを出す秒数")]
        private float _notificationDuration = 2;

        [SerializeField]
        private Text _notificationText;

        [SerializeField]
        [Header("ベッティングの金額の入力")]
        private InputField _betInput;

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        [Header("Hit, StayするボタンをまとめたCanvasGroup")]
        private CanvasGroup _actionCanvasGroup;

        [SerializeField]
        [Header("キャンバスグループをフェードする時間")]
        private float _fadeDuration = 0.3f;

        [SerializeField]
        private Button _hitButton;

        [SerializeField]
        private Button _stayButton;

        #endregion

        #region Member Variables

        private int _betValue = 0;

        #endregion

        #region Events

        /// <summary>
        /// スタートボタンが押されたときの処理
        /// int -> ベッティング金額
        /// </summary>
        private Subject<int> _onStartButton = new Subject<int>();

        private Subject<string> _onHitButton = new Subject<string>();

        private Subject<string> _onStayButton = new Subject<string>();

        #endregion

        #region Unity Methods

        private void Start()
        {
            SetUpInputEvent();
        }

        #endregion

        #region Public Methods

        /// <summary>ゲームの進行がリセットされた際に呼び出される</summary>
        public async void Init()
        {
            SetActionButton(false);
            await Task.Delay(_timeToStartButtonSelectable);
            // 初期化されたら入力可能にする
            _betInput.interactable = true;
            _startButton.interactable = true;
        }

        /// <summary>
        /// ボタンの表示,非表示を切りかえる
        /// </summary>
        /// <param name="active">
        /// ボタンを表示するかどうか
        /// true -> 表示
        /// </param>
        public void SetActionButton(bool active)
        {
            if (active == true)
            {
                _actionCanvasGroup.interactable = true;
                _actionCanvasGroup.blocksRaycasts = true;
                _actionCanvasGroup.DOFade(1, _fadeDuration);
            }
            else
            {
                _actionCanvasGroup.interactable = false;
                _actionCanvasGroup.blocksRaycasts = false;
                _actionCanvasGroup.DOFade(0, _fadeDuration);
            }
        }

        #endregion

        #region Private Methods

        private void SetUpInputEvent()
        {
            _betInput.onEndEdit.AddListener(x =>
            {
                if (string.IsNullOrWhiteSpace(x) == false)
                {
                    SetBetValue(int.Parse(x));
                }
                else
                {
                    SetBetValue(0);
                }
            });

            _startButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .ThrottleFirst(TimeSpan.FromMilliseconds(_timeToSelectableActionButton))
                .Subscribe(_ => OnStartButton());

            _hitButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .ThrottleFirst(TimeSpan.FromMilliseconds(_timeToSelectableActionButton))
                .Subscribe(_ => OnHitButton());

            _stayButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .ThrottleFirst(TimeSpan.FromMilliseconds(_timeToSelectableActionButton))
                .Subscribe(_ => OnStayButton());
        }

        private void OnStartButton()
        {
            if (_betValue <= 0)
            {
                StartCoroutine
                    (OutputNotificationText("ベット金額を入力してください"));
                return;
            }
            else if(CreditDataManager.Instance.Data.Credit < _betValue)
            {
                StartCoroutine
                    (OutputNotificationText("所持金が足りません"));
                return;
            }

            // スタート後はベット金額を入力不可能にする
            _betInput.interactable = false;
            _startButton.interactable = false;

            _onStartButton.OnNext(_betValue);
        }

        private void SetBetValue(int value)
        {
            _betValue = value;
        }

        private void OnHitButton()
        {
            _onHitButton.OnNext("Hit");
        }

        private void OnStayButton()
        {
            _onStayButton.OnNext("Stay");
        }

        private IEnumerator OutputNotificationText(object message)
        {
            _notificationText.text = message.ToString();
            yield return new WaitForSeconds(_notificationDuration);
            _notificationText.text = "";
        }

        #endregion
    }
}