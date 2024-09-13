using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ActionPart.MemoryPool
{
    public class ObjectPoolManager : MonoBehaviour
    {
        [System.Serializable]
        private class ObjectInfo
        {
            // 오브젝트 이름
            public string objectName;
            // 오브젝트 풀에서 관리할 오브젝트
            public GameObject perfab;
            // 몇개를 미리 생성 해놓을건지
            public int count;
        }

        public static ObjectPoolManager Instance;

        // 오브젝트풀 매니저 준비 완료표시
        public bool IsReady { get; private set; }

        [SerializeField]
        private ObjectInfo[] objectInfos = null;

        // 생성할 오브젝트의 key값지정을 위한 변수
        private string objectName;

        // 오브젝트풀들을 관리할 딕셔너리
        private Dictionary<string, IObjectPool<GameObject>> objectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();

        // 오브젝트풀에서 오브젝트를 새로 생성할때 사용할 딕셔너리
        private Dictionary<string, GameObject> objectDic = new Dictionary<string, GameObject>();

        private List<PoolAble> poolAbles = new List<PoolAble>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

            DontDestroyOnLoad(gameObject);
            Init();
        }

        private void Init()
        {
            IsReady = false;

            for (int idx = 0; idx < objectInfos.Length; idx++)
            {
                IObjectPool<GameObject> pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, true, objectInfos[idx].count, objectInfos[idx].count);

                if (objectDic.ContainsKey(objectInfos[idx].objectName))
                {
                    Debug.LogFormat("{0} 이미 등록된 오브젝트입니다.", objectInfos[idx].objectName);
                    return;
                }

                objectDic.Add(objectInfos[idx].objectName, objectInfos[idx].perfab);
                objectPoolDic.Add(objectInfos[idx].objectName, pool);

                // 미리 오브젝트 생성 해놓기
                for (int i = 0; i < objectInfos[idx].count; i++)
                {
                    objectName = objectInfos[idx].objectName;
                    PoolAble poolAble = CreatePooledItem().GetComponent<PoolAble>();
                    if (poolAble == null)
                    {
                        Debug.LogError(objectName + " Doesn't have PoolAble Script");
                        break;
                    }
                    poolAbles.Add(poolAble);
                    poolAble.pool.Release(poolAble.gameObject);
                }
            }

            Debug.Log("오브젝트풀링 준비 완료");
            IsReady = true;
        }

        public void RecallAll()
        {
            foreach (var poolAble in poolAbles)
            {
                if (poolAble.gameObject.activeSelf)
                    poolAble.ReleaseObject();
            }
        }

        // 생성
        private GameObject CreatePooledItem()
        {
            GameObject pooledObject = Instantiate(objectDic[objectName]);
            pooledObject.GetComponent<PoolAble>().pool = objectPoolDic[objectName];
            return pooledObject;
        }

        // 대여
        private void OnTakeFromPool(GameObject pooledObject)
        {
            if (pooledObject != null)
                pooledObject.SetActive(true);
            else
                Debug.Log($"Pool Get {pooledObject.name} null 오류");
        }

        // 반환
        private void OnReturnedToPool(GameObject pooledObject)
        {
            if (pooledObject != null)
                pooledObject.SetActive(false);
            else
                Debug.Log($"Pool Return {pooledObject.name} null 오류");
        }

        // 삭제
        private void OnDestroyPoolObject(GameObject pooledObject)
        {
            if (pooledObject != null)
                Destroy(pooledObject);
            else
                Debug.Log($"Pool Destroy {pooledObject.name} null 오류");
        }

        public GameObject GetObject(string objectName)
        {
            this.objectName = objectName;

            if (objectDic.ContainsKey(objectName) == false)
            {
                Debug.LogFormat("{0} 오브젝트풀에 등록되지 않은 오브젝트입니다.", objectName);
                return null;
            }

            return objectPoolDic[objectName].Get();
        }
    }
}