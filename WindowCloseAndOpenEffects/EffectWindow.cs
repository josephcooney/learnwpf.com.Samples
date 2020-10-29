using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidKit.Controls;
using System.Windows;
using System.Windows.Media.Animation;

namespace WindowCloseAndOpenEffects
{
    public class EffectWindow : GlassWindow
    {
        public static readonly DependencyProperty CloseStoryboardProperty = DependencyProperty.Register(
            "CloseStoryboard", typeof(Storyboard), typeof(EffectWindow));

        public Storyboard CloseStoryboard
        {
            get { return (Storyboard)GetValue(CloseStoryboardProperty); }
            set { SetValue(CloseStoryboardProperty, value); }
        }

        bool startedClose = false;
        public EffectWindow() 
        {
            this.Closing += new System.ComponentModel.CancelEventHandler(EffectWindow_Closing);
        }

        

        void EffectWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!startedClose && CloseStoryboard != null)
            {
                e.Cancel = true;
                startedClose = true;
                CloseStoryboard.Completed += new EventHandler(CloseStoryboard_Completed);
                CloseStoryboard.Begin();
            }
        }

        void CloseStoryboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
