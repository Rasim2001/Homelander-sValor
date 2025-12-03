using CutScenes;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameplayUI.CristalUI
{
    public class Cristal : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _fillImage;

        private readonly float _coolDown = 1;

        private ICristalMove _cristalMove;

        private Coroutine _coroutine;
        private ICristalTimeline _cristalTimeline;


        public void Initialize(PlayerMove playerMove, ICristalTimeline cristalTimeline)
        {
            _cristalTimeline = cristalTimeline;
            _cristalMove = new CristalMove(playerMove, transform);
        }

        private void Awake()
        {
            _canvas.worldCamera = Camera.main;
            _fillImage.fillAmount = 1;
        }

        public void Update()
        {
            if (_cristalMove == null || _cristalTimeline.IsPlaying)
                return;

            _cristalMove.Update();

            if (_fillImage.fillAmount < 1)
                _fillImage.fillAmount += Time.deltaTime / _coolDown;
        }

        public void CustomUpdate(Vector2 targetPosition) =>
            _cristalMove.Update(targetPosition);

        public bool SkillIsReady() =>
            _fillImage.fillAmount >= 0.97f;

        public void ResetCoolDown() =>
            _fillImage.fillAmount = 0;
    }
}