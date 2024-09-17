using Assets.Pia.Scripts.Effect;
using Default.Scripts.Util.StatePattern;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Pia.Scripts.General
{
    public class LandMine : StateManager<LandMineState>
    {
        public DirtController dirtController;
        private bool _available = false;

        void Awake()
        {
            this.UpdateAsObservable()
                .Where(_=> dirtController.holeDepth>=1)
                .First()
                .Subscribe(_ => BecomeAvailable());
            
        }

        public override void Start()
        {
            states = GetComponentsInChildren<LandMineState>(true);
            if (states.Length > 0)
            {
                currentState = states[currentIndex] as LandMineState;
                if (currentState != null)
                {
                    currentState.gameObject.SetActive(true);
                    currentState.OnEnter();
                }
            }
            this.UpdateAsObservable()
                .Where(_ => currentState.IsClear())
                .Subscribe(_ => Next());
        }

        private void BecomeAvailable()
        {
            _available = true;
        }

        public bool IsAvailable()
        {
            return _available;
        }
    }
}
