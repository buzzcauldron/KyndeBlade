using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>
    /// Procedural animation for characters: idle bob, attack lunge,
    /// hit recoil, and death fade. No animation clips required.
    /// </summary>
    public class SimpleAnimator : MonoBehaviour
    {
        [Header("Idle")]
        public float IdleBobAmount = 0.05f;
        public float IdleBobSpeed = 2f;

        [Header("Attack")]
        public float AttackLungeDistance = 0.5f;
        public float AttackDuration = 0.25f;

        [Header("Hit")]
        public float HitRecoilDistance = 0.2f;
        public float HitDuration = 0.15f;

        [Header("Death")]
        public float DeathFadeDuration = 0.8f;

        [Header("Cast")]
        public float CastGlowDuration = 0.4f;
        public Color CastGlowColor = new Color(0.8f, 0.7f, 0.3f, 0.8f);

        [Header("Dodge")]
        public float DodgeSlideDistance = 0.6f;
        public float DodgeDuration = 0.2f;

        Vector3 _basePosition;
        SpriteRenderer _sr;
        Color _baseColor;
        float _bobPhase;
        AnimState _state = AnimState.Idle;
        float _timer;
        float _direction = 1f;
        GameObject _statusParticles;
        SpriteRenderer _styleAura;
        SpriteRenderer _attackCue;
        PiersAppearanceData _appearanceData;
        Coroutine _cueRoutine;
        bool _styleInitialized;

        enum AnimState { Idle, Attacking, Recoiling, Dying, Casting, Dodging }

        void Start()
        {
            _basePosition = transform.localPosition;
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null) _baseColor = _sr.color;
            _appearanceData = GetComponent<PiersAppearanceData>();
            _bobPhase = Random.value * Mathf.PI * 2f;
            InitializeCharacterStyle();
        }

        void Update()
        {
            if (!_styleInitialized)
                InitializeCharacterStyle();

            UpdatePlayerStylePulse();

            switch (_state)
            {
                case AnimState.Idle:
                    UpdateIdle();
                    break;
                case AnimState.Attacking:
                    UpdateAttack();
                    break;
                case AnimState.Recoiling:
                    UpdateRecoil();
                    break;
                case AnimState.Dying:
                    UpdateDeath();
                    break;
                case AnimState.Casting:
                    UpdateCast();
                    break;
                case AnimState.Dodging:
                    UpdateDodge();
                    break;
            }
        }

        void UpdateIdle()
        {
            _bobPhase += Time.deltaTime * IdleBobSpeed;
            float yOffset = Mathf.Sin(_bobPhase) * IdleBobAmount;
            transform.localPosition = _basePosition + new Vector3(0f, yOffset, 0f);
        }

        void UpdateAttack()
        {
            _timer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_timer / AttackDuration);
            float curve = t < 0.5f ? t * 2f : (1f - t) * 2f;
            transform.localPosition = _basePosition + new Vector3(AttackLungeDistance * curve * _direction, 0f, 0f);
            if (_timer <= 0f) ReturnToIdle();
        }

        void InitializeCharacterStyle()
        {
            if (_styleInitialized) return;
            bool isPlayer = _appearanceData != null && _appearanceData.IsPlayer;
            if (!isPlayer) return;

            _styleInitialized = true;
            if (_sr != null)
                _sr.sortingOrder = Mathf.Max(_sr.sortingOrder, 10);

            var auraGo = new GameObject("PlayerStyleAura");
            auraGo.transform.SetParent(transform, false);
            auraGo.transform.localPosition = new Vector3(0f, 0f, 0.02f);
            auraGo.transform.localScale = new Vector3(1.25f, 1.25f, 1f);
            _styleAura = auraGo.AddComponent<SpriteRenderer>();
            _styleAura.sprite = PlaceholderSpriteFactory.GetSpriteForCharacter("wille", true);
            _styleAura.color = new Color(0.25f, 0.85f, 0.95f, 0.22f);
            _styleAura.sortingOrder = _sr != null ? _sr.sortingOrder - 1 : 9;

            var cueGo = new GameObject("AttackCue");
            cueGo.transform.SetParent(transform, false);
            cueGo.transform.localPosition = new Vector3(0f, 0.7f, -0.01f);
            cueGo.transform.localScale = Vector3.one * 0.5f;
            _attackCue = cueGo.AddComponent<SpriteRenderer>();
            _attackCue.sprite = PlaceholderSpriteFactory.GetSpriteForCharacter("envy", false);
            _attackCue.color = new Color(1f, 1f, 1f, 0f);
            _attackCue.sortingOrder = _sr != null ? _sr.sortingOrder + 2 : 12;
        }

        void UpdatePlayerStylePulse()
        {
            if (_styleAura == null) return;
            float pulse = 0.55f + Mathf.Sin(Time.time * 3.1f) * 0.45f;
            var c = _styleAura.color;
            c.a = 0.14f + 0.12f * pulse;
            _styleAura.color = c;
        }

        public void ShowAttackCue(bool incoming, float duration = 0.4f)
        {
            if (_attackCue == null) return;
            if (_cueRoutine != null) StopCoroutine(_cueRoutine);
            _cueRoutine = StartCoroutine(AttackCueRoutine(incoming, duration));
        }

        System.Collections.IEnumerator AttackCueRoutine(bool incoming, float duration)
        {
            if (_attackCue == null) yield break;

            Color cueColor = incoming
                ? new Color(0.95f, 0.25f, 0.2f, 1f)
                : new Color(1f, 0.85f, 0.25f, 1f);

            float total = Mathf.Max(0.12f, duration);
            float t = 0f;
            while (t < total)
            {
                t += Time.deltaTime;
                float n = Mathf.Clamp01(t / total);
                float pulse = Mathf.Sin(n * Mathf.PI * 3f) * (1f - n);
                _attackCue.color = new Color(cueColor.r, cueColor.g, cueColor.b, Mathf.Clamp01(0.25f + pulse * 0.75f));
                float scale = incoming ? 0.45f + pulse * 0.35f : 0.4f + pulse * 0.25f;
                _attackCue.transform.localScale = Vector3.one * scale;
                _attackCue.transform.localPosition = new Vector3(0f, 0.7f + pulse * 0.08f, -0.01f);
                yield return null;
            }

            _attackCue.color = new Color(1f, 1f, 1f, 0f);
            _attackCue.transform.localScale = Vector3.one * 0.5f;
            _attackCue.transform.localPosition = new Vector3(0f, 0.7f, -0.01f);
            _cueRoutine = null;
        }

        void UpdateRecoil()
        {
            _timer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_timer / HitDuration);
            float curve = t < 0.5f ? t * 2f : (1f - t) * 2f;
            transform.localPosition = _basePosition + new Vector3(-HitRecoilDistance * curve * _direction, 0f, 0f);
            if (_sr != null)
            {
                float flash = t < 0.5f ? 1f : 0f;
                _sr.color = Color.Lerp(_baseColor, Color.white, flash * 0.6f);
            }
            if (_timer <= 0f) ReturnToIdle();
        }

        void UpdateDeath()
        {
            _timer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_timer / DeathFadeDuration);
            if (_sr != null)
            {
                var c = _baseColor;
                c.a = 1f - t;
                _sr.color = c;
            }
            transform.localPosition = _basePosition + new Vector3(0f, -t * 0.3f, 0f);
        }

        void ReturnToIdle()
        {
            _state = AnimState.Idle;
            transform.localPosition = _basePosition;
            if (_sr != null) _sr.color = _baseColor;
        }

        /// <summary>Play attack lunge animation. Direction: +1 for right-facing, -1 for left.</summary>
        public void PlayAttack(float direction = 1f)
        {
            _direction = direction;
            _state = AnimState.Attacking;
            _timer = AttackDuration;
        }

        /// <summary>Play hit recoil animation.</summary>
        public void PlayHit(float direction = 1f)
        {
            _direction = direction;
            _state = AnimState.Recoiling;
            _timer = HitDuration;
        }

        /// <summary>Play death fade animation.</summary>
        public void PlayDeath()
        {
            _state = AnimState.Dying;
            _timer = DeathFadeDuration;
        }

        void UpdateCast()
        {
            _timer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_timer / CastGlowDuration);
            if (_sr != null)
            {
                float glow = Mathf.Sin(t * Mathf.PI);
                _sr.color = Color.Lerp(_baseColor, CastGlowColor, glow * 0.5f);
            }
            float scaleWave = 1f + Mathf.Sin(t * Mathf.PI) * 0.1f;
            transform.localScale = Vector3.one * scaleWave;
            if (_timer <= 0f)
            {
                transform.localScale = Vector3.one;
                ReturnToIdle();
            }
        }

        void UpdateDodge()
        {
            _timer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_timer / DodgeDuration);
            float curve = t < 0.5f ? t * 2f : (1f - t) * 2f;
            transform.localPosition = _basePosition + new Vector3(-DodgeSlideDistance * curve * _direction, 0f, 0f);
            if (_sr != null)
            {
                var c = _baseColor;
                c.a = 1f - curve * 0.4f;
                _sr.color = c;
            }
            if (_timer <= 0f) ReturnToIdle();
        }

        /// <summary>Play cast/spell animation with glow.</summary>
        public void PlayCast()
        {
            _state = AnimState.Casting;
            _timer = CastGlowDuration;
        }

        /// <summary>Play dodge/evade animation.</summary>
        public void PlayDodge(float direction = 1f)
        {
            _direction = direction;
            _state = AnimState.Dodging;
            _timer = DodgeDuration;
        }

        /// <summary>Update base color (after fairy form or appearance change).</summary>
        public void SetBaseColor(Color c)
        {
            _baseColor = c;
            if (_state == AnimState.Idle && _sr != null) _sr.color = c;
        }

        /// <summary>Snap base position to current transform position.</summary>
        public void RecaptureBasePosition()
        {
            _basePosition = transform.localPosition;
        }

        /// <summary>Show a status effect particle on this character.</summary>
        public void ShowStatusParticles(StatusEffectType type)
        {
            ClearStatusParticles();
            _statusParticles = type switch
            {
                StatusEffectType.Burning => ManuscriptParticleFactory.CreateBurningEmbers(transform),
                StatusEffectType.Frost => ManuscriptParticleFactory.CreateFrostCrystals(transform),
                StatusEffectType.Poison => ManuscriptParticleFactory.CreatePoisonDrip(transform),
                StatusEffectType.Stun => ManuscriptParticleFactory.CreateStunStars(transform),
                StatusEffectType.Blessed => ManuscriptParticleFactory.CreateBlessedGlow(transform),
                _ => null
            };
        }

        public void ClearStatusParticles()
        {
            if (_statusParticles != null)
            {
                Destroy(_statusParticles);
                _statusParticles = null;
            }
        }
    }
}
