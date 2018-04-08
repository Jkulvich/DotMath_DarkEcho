using System;

using LMDMono2D;

namespace DarkEcho
{
    public class SoundBall
    {
        public Dot Pos = new Dot();
        public Dot Vect = new Dot();
        public float LifeTime = 0f;
        public float MaxLifeTime = 0f;
        public Boolean ToDel = false;

        public SoundBall(Dot pos, Dot vect, float time)
        {
            this.Pos = pos;
            this.Vect = vect;
            this.MaxLifeTime = time;
            this.LifeTime = time;
        }
        public void Step()
        {
            LifeTime -= 1f;
            Pos += Vect;
        }
    }
}
