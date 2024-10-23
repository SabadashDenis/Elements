using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Scripts.Core
{
    public class BalloonView : View
    {
        [SerializeField] private List<Sprite> balloonVariants;
        [SerializeField] private Image viewImage;

        public void SetupRandomView()
        {
            var rndIndex = Random.Range(0, balloonVariants.Count);
            viewImage.sprite = balloonVariants[rndIndex];
        }
    }
}