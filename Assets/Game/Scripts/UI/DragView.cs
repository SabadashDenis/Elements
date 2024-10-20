using System;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Scripts.Core
{
    public class DragView : View, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private float distanceToDetectSwipe;
        
        private Vector2 currentDragData;
        private bool canDrag = true;

        public event Action<Direction> OnSwipe = delegate { };
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            canDrag = true;
            currentDragData = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (canDrag)
            {
                currentDragData += eventData.delta;

                if (Mathf.Abs(currentDragData.x) > distanceToDetectSwipe)
                {
                    canDrag = false;
                    var resultDir = currentDragData.x < 0 ? Direction.Left : Direction.Right;
                    OnSwipe.Invoke(resultDir);
                } 
                else if (Mathf.Abs(currentDragData.y) > distanceToDetectSwipe)
                {
                    canDrag = false;
                    var resultDir = currentDragData.y < 0 ? Direction.Bottom : Direction.Top;
                    OnSwipe.Invoke(resultDir);
                }
            }
        }
    }
}