using BlackJack.View;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace BlackJack.Model
{
    public class BoardResultPresenter : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField]
        private BoardResultView _boardResultView;

        [SerializeField]
        [Header("UIを初期化するまでの時間(ミリ秒)")]
        private int _timeToDoInit = 2000; 

        #endregion

        #region Member Variables
        #endregion

        #region Unity Methods

        private void Start()
        {
            Subscribe();
            SetEvent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// IObservableでイベントを購読
        /// </summary>
        private void Subscribe()
        {
            BoardModel.Instance.ObservablePlayerCardNum
                .Where(x => x != 0)
                .Subscribe(_boardResultView.SetPlayerHandTextNum);

            BoardModel.Instance.ObservableDealerCardNum
                .Where(x => x != 0)
                .Skip(1) //一度目のイベント発行時はディーラーはまだカードを公開していないので無視をする
                .Subscribe(_boardResultView.SetDealerHandTextNum);

            BoardModel.Instance.ObservableJudgeResult
                .Subscribe(_boardResultView.SetResultText);
        }

        private void SetEvent()
        {
            BoardModel.Instance.OnInitialize += () =>
            {
                _ = OnInit();
                Subscribe();
            };

            // ディーラーのアップカードが公開されたタイムミングでテキストに数字を反映させる
            BoardModel.Instance.OnOpenUpCard += () =>
            {
                _boardResultView.SetDealerHandTextNum(BoardModel.Instance.DealerCardNum);
            };

            // [BoardModel.DealerCardNum]に加算されていなかったホールカードを公開時にテキストに反映させる
            BoardModel.Instance.OnOpenHoleCard += () =>
            {
                _boardResultView.SetDealerHandTextNum(BoardModel.Instance.DealerCardNum);
            };
        }

        private async Task OnInit()
        {
            await Task.Delay(_timeToDoInit);
            _boardResultView.Init();
        }

        #endregion
    }
}