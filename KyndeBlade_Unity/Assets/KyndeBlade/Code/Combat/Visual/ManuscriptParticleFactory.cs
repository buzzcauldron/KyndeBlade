using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Creates procedural particle effects matching the illuminated manuscript art style.</summary>
    public static class ManuscriptParticleFactory
    {
        static Material _particleMat;

        static Material GetParticleMaterial()
        {
            if (_particleMat != null) return _particleMat;
            _particleMat = new Material(Shader.Find("Sprites/Default"));
            return _particleMat;
        }

        public static ParticleSystem CreateHitImpact(Vector3 position)
        {
            var go = new GameObject("HitImpact");
            go.transform.position = position;
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 0.3f;
            main.startLifetime = 0.4f;
            main.startSpeed = 3f;
            main.startSize = 0.15f;
            main.startColor = new Color(0.2f, 0.15f, 0.1f, 0.9f);
            main.maxParticles = 12;
            main.loop = false;
            main.playOnAwake = true;
            main.stopAction = ParticleSystemStopAction.Destroy;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 8, 12) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;

            var colorOverLife = ps.colorOverLifetime;
            colorOverLife.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(new Color(0.2f, 0.15f, 0.1f), 0f), new GradientColorKey(new Color(0.1f, 0.08f, 0.05f), 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLife.color = grad;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.material = GetParticleMaterial();

            return ps;
        }

        public static ParticleSystem CreateBurningEmbers(Transform parent)
        {
            return CreateStatusIndicator(parent, "BurningEmbers",
                new Color(1f, 0.5f, 0.1f, 0.8f), new Color(1f, 0.2f, 0f, 0f),
                rateOverTime: 6, startSpeed: 1.5f, startSize: 0.08f, lifetime: 0.8f);
        }

        public static ParticleSystem CreateFrostCrystals(Transform parent)
        {
            return CreateStatusIndicator(parent, "FrostCrystals",
                new Color(0.7f, 0.85f, 1f, 0.8f), new Color(0.5f, 0.7f, 1f, 0f),
                rateOverTime: 4, startSpeed: 0.5f, startSize: 0.1f, lifetime: 1.2f);
        }

        public static ParticleSystem CreatePoisonDrip(Transform parent)
        {
            return CreateStatusIndicator(parent, "PoisonDrip",
                new Color(0.3f, 0.7f, 0.2f, 0.8f), new Color(0.2f, 0.5f, 0.1f, 0f),
                rateOverTime: 3, startSpeed: -1f, startSize: 0.06f, lifetime: 0.6f);
        }

        public static ParticleSystem CreateStunStars(Transform parent)
        {
            return CreateStatusIndicator(parent, "StunStars",
                new Color(1f, 1f, 0.5f, 0.9f), new Color(1f, 1f, 0.3f, 0f),
                rateOverTime: 5, startSpeed: 0.8f, startSize: 0.12f, lifetime: 0.5f, orbital: true);
        }

        public static ParticleSystem CreateBlessedGlow(Transform parent)
        {
            return CreateStatusIndicator(parent, "BlessedGlow",
                new Color(1f, 0.9f, 0.5f, 0.6f), ManuscriptUITheme.Gold,
                rateOverTime: 8, startSpeed: 0.3f, startSize: 0.15f, lifetime: 1f);
        }

        public static ParticleSystem CreateBlessingPickup(Vector3 position)
        {
            var go = new GameObject("BlessingPickup");
            go.transform.position = position;
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 1f;
            main.startLifetime = 1.2f;
            main.startSpeed = 2f;
            main.startSize = 0.1f;
            main.startColor = ManuscriptUITheme.Gold;
            main.maxParticles = 30;
            main.loop = false;
            main.playOnAwake = true;
            main.stopAction = ParticleSystemStopAction.Destroy;
            main.gravityModifier = -0.5f;

            var emission = ps.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 20, 30) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.3f;

            var sizeOverLife = ps.sizeOverLifetime;
            sizeOverLife.enabled = true;
            sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

            var colorOverLife = ps.colorOverLifetime;
            colorOverLife.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(ManuscriptUITheme.Gold, 0f), new GradientColorKey(Color.white, 0.5f), new GradientColorKey(ManuscriptUITheme.Gold, 1f) },
                new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLife.color = grad;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.material = GetParticleMaterial();

            return ps;
        }

        public static ParticleSystem CreateDeathDissolve(Transform parent)
        {
            var go = new GameObject("DeathDissolve");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 1.5f;
            main.startLifetime = 2f;
            main.startSpeed = 0.5f;
            main.startSize = 0.08f;
            main.startColor = new Color(0.15f, 0.1f, 0.08f, 0.7f);
            main.maxParticles = 50;
            main.loop = false;
            main.playOnAwake = false;
            main.stopAction = ParticleSystemStopAction.Destroy;
            main.gravityModifier = -0.3f;

            var emission = ps.emission;
            emission.rateOverTime = 30;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Rectangle;
            shape.scale = new Vector3(0.5f, 0.8f, 0.1f);

            var colorOverLife = ps.colorOverLifetime;
            colorOverLife.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(new Color(0.15f, 0.1f, 0.08f), 0f), new GradientColorKey(new Color(0.1f, 0.08f, 0.05f), 1f) },
                new[] { new GradientAlphaKey(0.7f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLife.color = grad;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.material = GetParticleMaterial();

            return ps;
        }

        static ParticleSystem CreateStatusIndicator(Transform parent, string name,
            Color startColor, Color endColor,
            int rateOverTime, float startSpeed, float startSize, float lifetime,
            bool orbital = false)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 5f;
            main.startLifetime = lifetime;
            main.startSpeed = startSpeed;
            main.startSize = startSize;
            main.startColor = startColor;
            main.maxParticles = rateOverTime * 4;
            main.loop = true;
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = rateOverTime;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.2f;

            if (orbital)
            {
                var velocity = ps.velocityOverLifetime;
                velocity.enabled = true;
                velocity.orbitalX = 3f;
                velocity.orbitalY = 1f;
            }

            var colorOverLife = ps.colorOverLifetime;
            colorOverLife.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(startColor, 0f), new GradientColorKey(endColor, 1f) },
                new[] { new GradientAlphaKey(startColor.a, 0f), new GradientAlphaKey(0f, 1f) }
            );
            colorOverLife.color = grad;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.material = GetParticleMaterial();

            return ps;
        }
    }
}
