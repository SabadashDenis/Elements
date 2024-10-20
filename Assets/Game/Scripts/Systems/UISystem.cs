using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class UISystem : SystemBase
    {
        [SerializeField] private List<ScreenViewBase> screens;
        
        protected override void OnInit()
        {
            foreach (var screen in screens)
            {
                screen.Init();
            }
        }

        public TScreen GetScreen<TScreen>() where TScreen : ScreenViewBase
        {
            foreach (var screen in screens)
            {
                if (screen is TScreen tScreen)
                    return tScreen;
            }

            return null;
        }

            [Button]
        private void CollectScreens()
        {
            screens.Clear();
            screens = GetComponentsInChildren<ScreenViewBase>(true).ToList();
        }
    }
}