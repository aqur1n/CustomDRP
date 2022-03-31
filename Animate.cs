using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
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

        private MainWindow window;

        public AnimateManager(int fps, MainWindow window)
        {
            this.fps = fps;
            this.window = window;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(this.Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 / fps));
        }

        public void Play(Animate animate) { animate.Frame = 0; Animates.Add(animate); }

        public bool IsActive(Animate animate)
        { 
            return Animates.Contains(animate);
        }

        public void Start() { dispatcherTimer.Start(); }

        public void Stop() { dispatcherTimer.Stop(); }

        private int Frame = 0;
        // Остаток для фрейма анимации
        private int frame = 0;

        private static void Draw(Animate animate)
        {
            animate.ApplyFrame(animate.Frame);
            animate.Frame++;
        }

        public void Tick(object sender, EventArgs e)
        {
            try { frame = (int)(Frame % Animates.Count); } catch { return; }

            Animate animate = Animates[frame];

            if (animate.Frame > animate.MaxFrames) { Animates.Remove(animate); if (animate.OnDelete) { animate.Delete(); } return; }

            // будет ждать, если установлено значение wait
            if (animate.wait != 0)
            {
                if (animate.wait <= Frame * dispatcherTimer.Interval.Milliseconds)
                {
                    Draw(animate);

                    if (fps < animate.MaxFrames)
                    {
                        animate.wait += (int)(fps / animate.MaxFrames) * dispatcherTimer.Interval.Milliseconds;
                    }
                    else { animate.wait = 0; }
                }
            } 
            else
            {
                Draw(animate);
            }

            Frame++;
        }
    }

    internal class Animate
    {
        private int EffectType;
        public int Frame = 0;
        public int MaxFrames;
        internal UIElement Obj;

        public int wait = 1;
        public bool OnDelete = false;

        public Position Pos;

        public Animate(UIElement obj, int effectType, int frames, bool delete)
        {
            OnDelete = delete;

            EffectType = effectType;
            MaxFrames = frames;
            Obj = obj;

            StartOpacity = (float)obj.Opacity;
        }

        public Animate(UIElement obj, int effectType, int frames, Position pos)
        {
            Pos = pos;

            EffectType = effectType;
            MaxFrames = frames;
            Obj = obj;

            StartOpacity = (float)obj.Opacity;
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
            else if (EffectType == 2) // Перемещение
            {
                Obj.RenderTransform = new TranslateTransform(Effect.Racing(Pos.X * koef, Pos.X), Effect.Racing(Pos.Y * koef, Pos.Y));
            }
        }
    }
}

