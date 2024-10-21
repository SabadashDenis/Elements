using System;
using Game.Scripts.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Core
{
    public class DragView : View, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private float distanceToDetectSwipe;
        
        private Vector2 _currentDragData;
        private bool _canDrag = true;

        private bool _isInteractable = true;

        public event Action<DirectionData> OnSwipe = delegate { };

        protected bool IsInteractable
        {
            get => _isInteractable;
            set => _isInteractable = value;
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            _canDrag = true;
            _currentDragData = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_canDrag)
            {
                _currentDragData += eventData.delta;

                if (Mathf.Abs(_currentDragData.x) > distanceToDetectSwipe)
                {
                    _canDrag = false;
                    var resultDir = _currentDragData.x < 0 ? Direction.Left : Direction.Right;
                    OnSwipe.Invoke(new DirectionData(resultDir));
                } 
                else if (Mathf.Abs(_currentDragData.y) > distanceToDetectSwipe)
                {
                    _canDrag = false;
                    var resultDir = _currentDragData.y < 0 ? Direction.Down : Direction.Up;
                    OnSwipe.Invoke(new DirectionData(resultDir));
                }
            }
        }

        public void RemoveAllOnSwipeEvents() => OnSwipe = delegate { };
    }
}