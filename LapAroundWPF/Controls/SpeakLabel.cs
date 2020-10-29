using System;
using System.Collections.Generic;
using System.Text;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;


namespace LapAroundWPFTechEd2007.Controls
{
    public class SpeakLabel : Label
    {
        private bool hasLoaded; // some creational events seem to be firing twice

        SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        static SpeakLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpeakLabel), new FrameworkPropertyMetadata(typeof(SpeakLabel)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += new RoutedEventHandler(SpeakButton_Loaded);
        }

        void SpeakButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (!hasLoaded)
            {
                ButtonBase b = base.GetTemplateChild("PART_SPEAKBUTTON") as ButtonBase;
                if (b != null)
                {
                    b.Click += new RoutedEventHandler(b_Click);
                }
                hasLoaded = true;
            }
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            if (this.Content is string)
            {
                speechSynthesizer.SpeakAsync(this.Content as string);
            }
            else if (this.Content is TextBlock)
            {
                TextBlock text = this.Content as TextBlock;
                speechSynthesizer.SpeakAsync(text.Text);
            }
        }

    }
}
