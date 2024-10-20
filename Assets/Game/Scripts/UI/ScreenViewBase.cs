using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class ScreenViewBase : View
    {
        [SerializeField, FoldoutGroup("Screen Settings"), PropertyOrder(-99)] private bool showOnInit;

        protected override void OnInit()
        {
            HideImmediately();
            
            if (showOnInit)
                ShowImmediately();
        }
    }
}