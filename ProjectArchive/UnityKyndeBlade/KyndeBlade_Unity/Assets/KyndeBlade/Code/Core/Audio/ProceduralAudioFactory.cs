using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Generates placeholder AudioClips via AudioClip.Create() with simple waveforms.
    /// Replace with real audio by assigning clips in AudioLibrary or Inspector fields.
    /// </summary>
    public static class ProceduralAudioFactory
    {
        const int SampleRate = 44100;

        public static AudioClip Hit()       => GenerateNoiseBurst("Hit", 0.12f, 0.8f);
        public static AudioClip ParrySuccess() => GenerateMetallicRing("ParrySuccess", 0.25f, 880f);
        public static AudioClip DodgeSuccess() => GenerateWhoosh("DodgeSuccess", 0.2f);
        public static AudioClip Fail()      => GenerateDescendingTone("Fail", 0.15f, 440f, 220f);
        public static AudioClip Heal()      => GenerateAscendingShimmer("Heal", 0.4f);
        public static AudioClip MenuClick() => GenerateClick("MenuClick", 0.05f, 1000f);
        public static AudioClip PageTurn() => GeneratePageTurn("PageTurn", 0.3f);
        public static AudioClip BlessingSelect() => GenerateBlessingChime("BlessingSelect", 0.6f);

        public static AudioClip VictoryFanfare() => GenerateChord("Victory", 1.2f,
            new[] { 523.25f, 659.25f, 783.99f }, ascending: true);

        public static AudioClip DefeatSting() => GenerateChord("Defeat", 1.0f,
            new[] { 392f, 349.23f, 311.13f }, ascending: false);

        public static AudioClip BurnCrackle() => GenerateBurnCrackle("BurnCrackle", 0.4f);
        public static AudioClip FrostShatter() => GenerateFrostShatter("FrostShatter", 0.3f);
        public static AudioClip StunImpact() => GenerateStunImpact("StunImpact", 0.2f);
        public static AudioClip PoisonBubble() => GeneratePoisonBubble("PoisonBubble", 0.35f);

        public static AudioClip MenuTheme(float duration = 24f) =>
            GenerateMedievalMenuTheme("MenuTheme", duration);

        public static AudioClip GreenKnightTheme(float duration = 16f) =>
            GenerateOmenTheme("GreenKnightTheme", duration);

        public static AudioClip OrfeoTheme(float duration = 20f) =>
            GenerateOtherworldTheme("OrfeoTheme", duration);

        public static AudioClip AmbientDrone(float duration = 16f)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            float baseFreq = 110f;
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Clamp01(t / 2f) * Mathf.Clamp01((duration - t) / 2f);
                float val = Mathf.Sin(2f * Mathf.PI * baseFreq * t) * 0.15f;
                val += Mathf.Sin(2f * Mathf.PI * baseFreq * 1.5f * t) * 0.08f;
                val += Mathf.Sin(2f * Mathf.PI * baseFreq * 2f * t) * 0.05f;
                float lfo = 1f + Mathf.Sin(2f * Mathf.PI * 0.1f * t) * 0.3f;
                data[i] = val * env * lfo;
            }
            return CreateClip("AmbientDrone", data);
        }

        static AudioClip GenerateNoiseBurst(string name, float duration, float volume)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            var rng = new System.Random(42);
            for (int i = 0; i < samples; i++)
            {
                float env = 1f - (i / (float)samples);
                env *= env;
                data[i] = ((float)rng.NextDouble() * 2f - 1f) * volume * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateMetallicRing(string name, float duration, float freq)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Exp(-t * 8f);
                float val = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.5f;
                val += Mathf.Sin(2f * Mathf.PI * freq * 2.76f * t) * 0.25f;
                val += Mathf.Sin(2f * Mathf.PI * freq * 5.4f * t) * 0.1f;
                data[i] = val * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateWhoosh(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            var rng = new System.Random(7);
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)samples;
                float env = Mathf.Sin(t * Mathf.PI);
                float noise = (float)rng.NextDouble() * 2f - 1f;
                float cutoff = Mathf.Lerp(0.2f, 0.8f, t);
                data[i] = noise * env * 0.4f * cutoff;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateDescendingTone(string name, float duration, float startFreq, float endFreq)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float freq = Mathf.Lerp(startFreq, endFreq, progress);
                float env = 1f - progress;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.5f * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateAscendingShimmer(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float freq = Mathf.Lerp(400f, 1200f, progress);
                float env = Mathf.Sin(progress * Mathf.PI);
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.3f * env;
                data[i] += Mathf.Sin(2f * Mathf.PI * freq * 1.5f * t) * 0.15f * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateClick(string name, float duration, float freq)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Exp(-t * 40f);
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.6f * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateChord(string name, float duration, float[] freqs, bool ascending)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float env = Mathf.Clamp01(t / 0.05f) * Mathf.Clamp01((duration - t) / 0.3f);
                float val = 0f;
                for (int f = 0; f < freqs.Length; f++)
                {
                    float noteStart = ascending ? f * 0.15f : f * 0.12f;
                    if (t < noteStart) continue;
                    float noteEnv = Mathf.Exp(-(t - noteStart) * 2f);
                    val += Mathf.Sin(2f * Mathf.PI * freqs[f] * t) * noteEnv / freqs.Length;
                }
                data[i] = val * env * 0.5f;
            }
            return CreateClip(name, data);
        }

        static AudioClip GeneratePageTurn(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            var rng = new System.Random(123);
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)samples;
                float env = Mathf.Sin(t * Mathf.PI) * Mathf.Exp(-t * 6f);
                float noise = (float)rng.NextDouble() * 2f - 1f;
                float filtered = noise * Mathf.Clamp01(1f - t * 3f) * 0.3f;
                data[i] = filtered * env * 0.5f;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateBlessingChime(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            float[] notes = { 784f, 988f, 1175f, 1319f };
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float val = 0f;
                for (int n = 0; n < notes.Length; n++)
                {
                    float onset = n * 0.1f;
                    if (t < onset) continue;
                    float dt = t - onset;
                    val += Mathf.Sin(2f * Mathf.PI * notes[n] * dt) * Mathf.Exp(-dt * 4f) * 0.2f;
                }
                data[i] = val;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateBurnCrackle(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            var rng = new System.Random(77);
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Sin(t / duration * Mathf.PI);
                float pop = rng.NextDouble() < 0.03 ? (float)rng.NextDouble() * 0.8f : 0f;
                float crackle = (float)rng.NextDouble() * 2f - 1f;
                data[i] = (crackle * 0.15f + pop) * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateFrostShatter(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            var rng = new System.Random(99);
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Exp(-t * 10f);
                float crystal = Mathf.Sin(2f * Mathf.PI * 3000f * t) * 0.2f * Mathf.Exp(-t * 20f);
                float shatter = ((float)rng.NextDouble() * 2f - 1f) * 0.4f;
                data[i] = (crystal + shatter * env) * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateStunImpact(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Exp(-t * 15f);
                float low = Mathf.Sin(2f * Mathf.PI * 80f * t) * 0.6f;
                float mid = Mathf.Sin(2f * Mathf.PI * 200f * t) * 0.3f * Mathf.Exp(-t * 20f);
                data[i] = (low + mid) * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GeneratePoisonBubble(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            var rng = new System.Random(55);
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Sin(t / duration * Mathf.PI);
                float bubbleRate = 8f + Mathf.Sin(t * 3f) * 4f;
                float bubble = Mathf.Sin(2f * Mathf.PI * 300f * t) * Mathf.Abs(Mathf.Sin(bubbleRate * t));
                float drip = rng.NextDouble() < 0.02 ? Mathf.Sin(2f * Mathf.PI * 600f * t) * 0.3f : 0f;
                data[i] = (bubble * 0.2f + drip) * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateMedievalMenuTheme(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            float[] melody = { 262f, 294f, 330f, 349f, 392f, 349f, 330f, 294f, 262f, 220f, 247f, 262f };
            float noteLen = duration / melody.Length;
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                int noteIdx = Mathf.Min((int)(t / noteLen), melody.Length - 1);
                float noteT = (t % noteLen) / noteLen;
                float freq = melody[noteIdx];
                float env = Mathf.Clamp01(t / 1f) * Mathf.Clamp01((duration - t) / 2f);
                float noteEnv = Mathf.Sin(noteT * Mathf.PI);
                float lute = Mathf.Sin(2f * Mathf.PI * freq * t) * 0.2f * Mathf.Exp(-noteT * 2f);
                lute += Mathf.Sin(2f * Mathf.PI * freq * 2f * t) * 0.08f * Mathf.Exp(-noteT * 3f);
                float drone = Mathf.Sin(2f * Mathf.PI * 130f * t) * 0.06f;
                data[i] = (lute + drone) * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateOmenTheme(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Clamp01(t / 2f) * Mathf.Clamp01((duration - t) / 2f);
                float bass = Mathf.Sin(2f * Mathf.PI * 73.4f * t) * 0.2f;
                float tritone = Mathf.Sin(2f * Mathf.PI * 103.8f * t) * 0.12f;
                float pulse = Mathf.Abs(Mathf.Sin(2f * Mathf.PI * 0.5f * t));
                float tremolo = 0.7f + 0.3f * Mathf.Sin(2f * Mathf.PI * 4f * t);
                data[i] = (bass + tritone * pulse) * env * tremolo;
            }
            return CreateClip(name, data);
        }

        static AudioClip GenerateOtherworldTheme(string name, float duration)
        {
            int samples = (int)(SampleRate * duration);
            var data = new float[samples];
            float[] harmonics = { 220f, 277.2f, 330f, 440f };
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float env = Mathf.Clamp01(t / 3f) * Mathf.Clamp01((duration - t) / 3f);
                float val = 0f;
                for (int h = 0; h < harmonics.Length; h++)
                {
                    float lfo = Mathf.Sin(2f * Mathf.PI * (0.1f + h * 0.03f) * t);
                    float freq = harmonics[h] * (1f + lfo * 0.01f);
                    val += Mathf.Sin(2f * Mathf.PI * freq * t) * (0.12f / (h + 1));
                }
                float ghost = Mathf.Sin(2f * Mathf.PI * 660f * t) * 0.03f *
                    Mathf.Abs(Mathf.Sin(2f * Mathf.PI * 0.25f * t));
                data[i] = (val + ghost) * env;
            }
            return CreateClip(name, data);
        }

        static AudioClip CreateClip(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
