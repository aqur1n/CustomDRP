using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace CustomDRP
{
    internal class AnimateManager
    {
        public int fps = 30;
        internal DispatcherTimer dispatcherTimer;
        private List<Animate> Animates = new();

        public AnimateManager(int fps)
        {
            this.fps = fps;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(this.Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 / fps));
        }

        public void Play(Animate animate) { animate.Frame = 0; Animates.Add(animate); }
        public void Play(Animate[] animates) { foreach (Animate animate in animates) { animate.Frame = 0; Animates.Add(animate); } }

        public bool IsActive(Animate animate)
        { 
            return Animates.Contains(animate);
        }

        public void Start() { dispatcherTimer.Start(); }

        public void Stop() { dispatcherTimer.Stop(); }

        private int Frame = 0;

        private static void Draw(Animate animate)
        {
            animate.ApplyFrame(animate.Frame);
            animate.Frame++;
        }

        public void Tick(object sender, EventArgs e)
        {

            for (int i = 0; i < Animates.Count; i++)
            {
                Animate animate = Animates[i];

                if (animate.Frame > animate.MaxFrames) { Animates.Remove(animate); if (animate.OnDelete) { animate.Delete(); } return; }

                // будет ждать, если установлено значение wait
                if (animate.wait <= Frame * dispatcherTimer.Interval.Milliseconds)
                {
                    Draw(animate);

                    animate.wait = 0;

                }
                else
                {
                    Draw(animate);
                }
            }
            Frame++;
        }
    }

    internal class Animate
    {
        private int EffectType;
        public int Frame = 0;
        public int MaxFrames;
        internal FrameworkElement Obj;

        public int wait = 0;
        public bool OnDelete = false;

        public Position Pos;

        public Animate(FrameworkElement obj, int effectType, int frames, bool delete)
        {
            OnDelete = delete;

            EffectType = effectType;
            MaxFrames = frames;
            Obj = obj;

            StartOpacity = (float)obj.Opacity;
        }

        public Animate(FrameworkElement obj, int effectType, int frames, Position vector)
        {
            Pos = vector;

            EffectType = effectType;
            MaxFrames = frames;
            Obj = obj;

            StartOpacity = (float)obj.Opacity;

            if (effectType == 2) { obj.HorizontalAlignment = HorizontalAlignment.Left; obj.VerticalAlignment = VerticalAlignment.Top; }
        }

        public void Wait(int miliseconds) { this.wait = miliseconds; }

        public void Delete()
        {
            Obj.IsEnabled = false;
            Obj.Visibility = Visibility.Hidden;
        }

        private float StartOpacity = 0f;

        public void ApplyFrame(int frame)
        {
            float koef = (float)frame / (float)this.MaxFrames;

            if (EffectType == 0) // Затухание
            {
                Obj.Opacity = Other.Limit(StartOpacity - koef, 0f, 1f);
            }
            else if (EffectType == 1) // Появление
            {
                Obj.Opacity = Other.Limit(StartOpacity + koef, 0f, 1f);
            }
            //else if (EffectType == 2) // Перемещение
            //{
            //    //MessageBox.Show($"koef - {koef}, Pos.Y - {Pos.Y}");
            //    Obj.Margin = new Thickness(Effect.Linear(Pos.X * koef, Pos.X, (float)Obj.Margin.Left), Effect.Linear(Pos.Y, Pos.Y, (float)Obj.Margin.Top), 0, 0);
            //}
        }
    }
}

