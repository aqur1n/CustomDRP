using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace CustomDRP
{
    internal class AnimateManager
    {
        public int fps = 30;
        private DispatcherTimer dispatcherTimer;
        private List<Animate> Animates = new List<Animate>();

        public AnimateManager(int fps)
        {
            this.fps = fps;

            this.dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(this.Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 / fps));
        }

        public void Play(Animate animate) { Animates.Add(animate); }

        public void Start() { this.dispatcherTimer.Start(); }

        public void Stop() { this.dispatcherTimer.Stop(); }

        private int Frame = 0;
        // Остаток для фрейма анимации
        private int frame = 0;

        public void Tick(object sender, EventArgs e)
        {
            try { frame = (int)(Frame % Animates.Count); } catch { return; }

            Animate animate = Animates[frame];

            if (animate.Frame > animate.MaxFrames) { Animates.Remove(animate); if (animate.OnDelete) { animate.Delete(); } return; }

            // будет ждать, если установлено значение wait
            if (animate.wait <= ((float)Frame * (float)this.fps) / 1000f + 0.1f)
            {
                animate.ApplyFrame(animate.Frame);

                animate.Frame++;
            }

            Frame++;
        }
    }

    internal class Animate
    {
        private int EffectType;
        public int Frame = 0;
        public int MaxFrames;
        private dynamic Obj;

        public int wait = 0;
        public bool OnDelete = false;

        public Animate(dynamic obj, int effectType, int frames, bool delete)
        {
            this.OnDelete = delete;

            this.EffectType = effectType;
            this.MaxFrames = frames;
            this.Obj = obj;

            this.StartOpacity = (float)obj.Opacity;
        }

        public void Wait(int seconds) { this.wait = seconds; }

        public void Delete()
        { 
            this.Obj.IsEnabled = false;
            this.Obj.Visibility = Visibility.Hidden;
        }

        private static float Limit(float x, float z, float y)
        {
            if (x <= y && x >= z) { return x; }
            else if (x > y) { return y; }
            else { return z; }
        }

        private float StartOpacity = 0f;

        public void ApplyFrame(int frame)
        {
            if (EffectType == 0)
            {
                this.Obj.Opacity = Limit(StartOpacity - (float)frame / (float)this.MaxFrames, 0f, 1f);
            }
            else if (EffectType == 1)
            {
                this.Obj.Opacity = Limit(StartOpacity + ((float)frame / (float)this.MaxFrames), 0f, 1f);
            }
        }
    }
}

