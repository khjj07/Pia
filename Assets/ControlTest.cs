using Assets.Pia.Scripts.Game;
using Assets.Pia.Scripts.General;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ControlTest : MonoBehaviour
{
    [SerializeField] private Player _player;

    [SerializeField] private LandMine _landMine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _player.UpdateAsObservable()
            .Subscribe(_ => _player.RepositioningThroughFoot(_landMine.Dirt.top))
            .AddTo(_player.gameObject);
        _player.OnStepMine();
        _player.bag.Close();
        _player.SetCursorLocked();
    }
}
