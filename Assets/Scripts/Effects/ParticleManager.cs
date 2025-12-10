using UnityEngine;
using System.Collections.Generic;

namespace PawzyPop.Effects
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager Instance { get; private set; }

        [Header("Particle Prefabs")]
        [SerializeField] private ParticleSystem matchParticlePrefab;
        [SerializeField] private ParticleSystem comboParticlePrefab;
        [SerializeField] private ParticleSystem starParticlePrefab;

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 10;

        private Queue<ParticleSystem> matchParticlePool;
        private Queue<ParticleSystem> comboParticlePool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            matchParticlePool = new Queue<ParticleSystem>();
            comboParticlePool = new Queue<ParticleSystem>();

            if (matchParticlePrefab != null)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    var particle = Instantiate(matchParticlePrefab, transform);
                    particle.gameObject.SetActive(false);
                    matchParticlePool.Enqueue(particle);
                }
            }

            if (comboParticlePrefab != null)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    var particle = Instantiate(comboParticlePrefab, transform);
                    particle.gameObject.SetActive(false);
                    comboParticlePool.Enqueue(particle);
                }
            }
        }

        public void PlayMatchEffect(Vector3 position, Color color)
        {
            if (matchParticlePool == null || matchParticlePool.Count == 0)
            {
                // 没有粒子系统，创建简单的缩放效果
                CreateSimpleEffect(position, color);
                return;
            }

            var particle = matchParticlePool.Dequeue();
            particle.transform.position = position;
            
            // 设置粒子颜色
            var main = particle.main;
            main.startColor = color;

            particle.gameObject.SetActive(true);
            particle.Play();

            // 播放完成后回收
            StartCoroutine(ReturnToPool(particle, matchParticlePool, main.duration));
        }

        public void PlayComboEffect(Vector3 position, int comboCount)
        {
            if (comboParticlePool == null || comboParticlePool.Count == 0)
                return;

            var particle = comboParticlePool.Dequeue();
            particle.transform.position = position;

            // 根据连击数调整粒子数量
            var emission = particle.emission;
            emission.rateOverTime = 10 + comboCount * 5;

            particle.gameObject.SetActive(true);
            particle.Play();

            var main = particle.main;
            StartCoroutine(ReturnToPool(particle, comboParticlePool, main.duration));
        }

        public void PlayStarEffect(Vector3 position)
        {
            if (starParticlePrefab == null) return;

            var particle = Instantiate(starParticlePrefab, position, Quaternion.identity);
            Destroy(particle.gameObject, particle.main.duration + 0.5f);
        }

        private System.Collections.IEnumerator ReturnToPool(ParticleSystem particle, Queue<ParticleSystem> pool, float delay)
        {
            yield return new WaitForSeconds(delay + 0.1f);
            
            particle.Stop();
            particle.gameObject.SetActive(false);
            pool.Enqueue(particle);
        }

        private void CreateSimpleEffect(Vector3 position, Color color)
        {
            // 创建简单的视觉反馈（当没有粒子系统时）
            GameObject effect = new GameObject("MatchEffect");
            effect.transform.position = position;

            SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
            sr.color = color;
            sr.sortingOrder = 100;

            // 简单的缩放消失动画
            StartCoroutine(SimpleEffectAnimation(effect));
        }

        private System.Collections.IEnumerator SimpleEffectAnimation(GameObject effect)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = Vector3.one * 0.5f;
            Vector3 endScale = Vector3.one * 1.5f;

            SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
            Color startColor = sr.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                effect.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                
                Color c = startColor;
                c.a = 1f - t;
                sr.color = c;

                yield return null;
            }

            Destroy(effect);
        }
    }
}
