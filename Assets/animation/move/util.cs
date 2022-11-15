using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 共通して利用できるクラスはこちらに作成する
/// </summary>
namespace util
{
    public class ProgressBar: MonoBehaviourPunCallbacks
    {
        [SerializeField] public bool isActive = false;
        [SerializeField] protected GameObject progressPanel;
        protected RectTransform panelTransform;
        [SerializeField] protected float maxWidth;
        [Range(0f,1f)] public float progressRatio;
        [SerializeField] protected float needTime;
        protected float progressTime;
        protected bool istaskCompleted = false;

        private void Awake()
        {
            panelTransform = progressPanel.GetComponent<RectTransform>();
        }

        protected void changePanelSize(ref RectTransform rectTransform, float size)
        {
            Vector2 rectSize = rectTransform.sizeDelta;
            rectSize.x = size;
            rectTransform.sizeDelta = rectSize;
        }


        protected void Progress()
        {
            progressTime += Time.deltaTime;
            progressRatio = progressTime / needTime;
            float changedSize = progressRatio * maxWidth;
            if(0<progressRatio&&progressRatio<1.0)
            {
                changePanelSize(ref panelTransform, changedSize);
            }
            if(progressRatio>=1)
            {
                changePanelSize(ref panelTransform, maxWidth);
                CompleteTask();
            }

        }

        virtual public void CompleteTask()
        {
            istaskCompleted = true;
        }

        protected void TaskStop()
        {
            isActive = false;
            this.gameObject.SetActive(false);
        }
        protected void TaskStart()
        {
            isActive = true;
        }


    }




    //ダメージを受ける処理のインターフェース
    public interface iApplicableDamaged
    { 
        void damaged(int amount);
        void die();
    }

    //ダメージを与えることを保証するインターフェース
    public interface iCanDamage
    {
        void damage(int damageAmount, iApplicableDamaged target);

    }

    //爆発のためのインターフェース
    public interface canExplode
    {
        void playEffect();

    }

    //弾を発射するためのインターフェース
    public abstract class LaunchBulletController : MonoBehaviour
    {
        
        [SerializeField]
        GameObject positionTarget;
        [SerializeField]
        GameObject rotationTarget;
        [SerializeField]
        protected GameObject bulletPrefab;
        public Vector3 launchPosition;
        public Quaternion targetRotation;
        protected float span;
        [SerializeField]
        protected float defaultSpan;
        public bool canLaunch = true;

        private void Start()
        {
            span = defaultSpan;
            launchPosition = positionTarget.transform.position;
            targetRotation = rotationTarget.transform.rotation;
        }
        bool getCanLaunch()
        {
            return canLaunch;
        }
        public void launch()
        {
            if (canLaunch == true)
            {
                Instantiate(bulletPrefab, launchPosition, targetRotation);
                span = defaultSpan;
                canLaunch = false;
            }
        }
        

        public void updateSpan()
        {
            if (canLaunch == false)
            {
                span -= Time.deltaTime;
                if (span < 0)
                {
                    canLaunch = true;
                }
            }
        }
        private void Update()
        {
            updateSpan();
            launchPosition = positionTarget.transform.position;
            targetRotation = rotationTarget.transform.rotation;
        }
    }
    
}