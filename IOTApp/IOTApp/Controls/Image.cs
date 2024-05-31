using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace IOTApp.Controls
{
    public class Image : Xamarin.Forms.Image
    {
        private bool blinkLock = false;

        public static readonly BindableProperty IsBlinkingProperty = BindableProperty.Create(
            "IsBlinking",
            typeof(bool),
            typeof(Image),
            false
        );
        public bool IsBlinking
        {
            get => (bool)GetValue(IsBlinkingProperty);
            set => SetValue(IsBlinkingProperty, value);
        }

        public static readonly BindableProperty BlinkDurationProperty = BindableProperty.Create(
            "BlinkDuration",
            typeof(uint),
            typeof(Image),
            (uint)500
        );

        public uint BlinkDuration
        {
            get => (uint)GetValue(BlinkDurationProperty);
            set => SetValue(BlinkDurationProperty, value);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if(propertyName == IsBlinkingProperty.PropertyName)
            {
                SetBlinking(IsBlinking);
            }
            else if(propertyName == BlinkDurationProperty.PropertyName)
            {
                if(blinkLock)
                {
                    SetBlinking(false);
                    SetBlinking(IsBlinking);
                }
            }
        }

        void SetBlinking(bool shouldBlink)
        {
            if(shouldBlink && !blinkLock)
            {
                blinkLock = true;

                var blinkAnimation = new Animation((d) => {
                    Opacity = d;
                }, 0f, 1f, Easing.SinInOut);

                this.Animate("EBImageBlink", blinkAnimation, length: BlinkDuration, finished: (_, __) => { 
                    blinkLock = false;    
                });
            }
            else if(!shouldBlink && blinkLock)
            {
                blinkLock = false;
            }
        }
    }
}
