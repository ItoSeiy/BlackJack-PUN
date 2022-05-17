using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlackJack.View;
using UniRx;

namespace BlackJack.Presenter
{
    public class InputPresenter : MonoBehaviour
    {
        #region Properties
        #endregion

        #region Inspector Variables

        [SerializeField]
        private InputView _inputView;

        #endregion

        #region Member Variables
        #endregion

        #region Constant
        #endregion

        #region Events
        #endregion

        #region Unity Methods

        private void Start()
        {
            SubscribeInput();
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods

        private void SubscribeInput()
        {
            _inputView.ObservableGameStart.Subscribe(OnGameStart);
        }

        private void OnGameStart(int vetValue)
        {

        }

        #endregion
    }
}