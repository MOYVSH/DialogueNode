/**
 * 
 * class: EasyObjectPool
 * 
 * A lightweight object pool.
 * 
 * You need to create an object in the scene and then hang it.
 *
 * Support automatic capacity expansion.
 *  
 * Support recycling detection.
 * ��������������������������������
 * ��Ȩ����������ΪCSDN�����������⡹��ԭ�����£���ѭCC 4.0 BY-SA��ȨЭ�飬ת���븽��ԭ�ĳ������Ӽ���������
 * ԭ�����ӣ�https://blog.csdn.net/qq_39162566/article/details/128290119
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR || ENABLE_LOG
using UnityEditor;
#endif

public class EasyObjectPool : MonoBehaviour
{

    public class TransformPool
    {
        /// <summary>
        /// ģ��Ԥ��
        /// </summary>
        /// <param name="prefab"> Ԥ�� </param>
        /// <param name="parent"> ָ��һ������ </param>
        public TransformPool(GameObject prefab, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        //��ȡԤ��name(KEY)
        //public string name
        //{
        //    get
        //    {
        //        if ( this.prefab )
        //            return this.prefab.name;
        //        return string.Empty;
        //    }
        //    set
        //    {
        //        if ( this.prefab )
        //            this.prefab.name = value;
        //    }
        //}

        private Queue<Transform> free = new Queue<Transform>();//��������
        private List<Transform> active = new List<Transform>();//��������
        private GameObject prefab;//Ԥ��ģ��
        private Transform parent;//���ڵ�
        private float expandTimeSinceStartup = 0f; //����ʱ��
        private int expandCount = 10;//�������
        private int tryExpandCount = 0;//�������ݵĴ���
        //��̬����
        private void AutoExpandImmediately()
        {
            //0.01�����ڷ����������
            if (Time.realtimeSinceStartup - expandTimeSinceStartup < 1e-2)
            {
                expandCount = expandCount * 10;
            }
            else
            {
                expandCount = 10;
            }
            //����
            Reserve(expandCount);
        }

        /// <summary>
        /// �������
        /// 
        /// �������������Ƴ�һ�����ö��� ��������뼤���б����
        /// ͨ������� ��ֻ��Ҫ�������б��еĶ��󼴿�
        /// 
        /// </summary>
        /// <returns> ����ҪΪ�����ø��� ��Ϊ������ ActiveΪtrue </returns>
        public Transform Pop()
        {
            if (free.Count > 0)
            {
                //����ɹ�
                tryExpandCount = 0;
                var freeObj = free.Dequeue();
                active.Add(freeObj);
                return freeObj;
            }

            //���������ڴ�ʧ��
            if (tryExpandCount > 5)
            {
                //#if UNITY_EDITOR || ENABLE_LOG
                //                //��ʼ����GC����
                //                System.GC.Collect( 0, System.GCCollectionMode.Forced, true, true );
                //                return Pop();
                //#endif
                return null;
            }

            //����
            ++tryExpandCount;
            AutoExpandImmediately();
            return Pop();
        }

        /// <summary>
        /// ���ƶ���
        /// </summary>
        /// <param name="obj"></param>
        public void Push_Back(Transform obj)
        {
            if (null != obj)
            {
                obj.transform.SetParent(parent);
                obj.gameObject.SetActive(false);
                if (active.Contains(obj))
                {
                    active.Remove(obj);
                }
                if (!free.Contains(obj))
                {
                    free.Enqueue(obj);
                }
            }
        }


        public enum PoolElementState
        {
            Unknown = 0,//δ֪ �����ڵ�ǰ�������
            Active, //����״̬
            Free, //����״̬
        }

        /// <summary>
        /// ��ȡ�����ڳ��е�״̬
        /// </summary>
        /// <param name="dest"> enum: PoolElementState </param>
        /// <returns></returns>
        public PoolElementState GetElementState(Transform dest)
        {
            if (free.Contains(dest))
            {
                return PoolElementState.Free;
            }

            if (active.Contains(dest))
            {
                return PoolElementState.Active;
            }

            return PoolElementState.Unknown;
        }

        /// <summary>
        /// �Ƿ��ڶ����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool InSidePool(Transform obj)
        {
            return PoolElementState.Unknown != GetElementState(obj);
        }

        /// <summary>
        /// �������м���Ķ���
        /// </summary>
        public void Recycle()
        {
            var objs = active.ToArray();
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] != null)
                {
                    Push_Back(objs[i]);
                }
            }

            objs = free.ToArray();
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] != null)
                {
                    Push_Back(objs[i]);
                }
            }
        }

        /// <summary>
        /// Ԥ��һ��������Ԥ��
        /// </summary>
        /// <param name="count"></param>
        public void Reserve(int count)
        {
            string key = prefab.name;
            for (int i = 0; i < count; i++)
            {
                var inst = GameObject.Instantiate(prefab, parent);
                inst.SetActive(false);
                inst.name = $"{key} <Clone>";
                free.Enqueue(inst.transform);
                nameofDict.Add(inst.transform, key);
            }
            expandTimeSinceStartup = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// �˲�������ȫ�ͷŶ�����ڴ�ռ�� ��deleteŶ~
        /// </summary>
        public void Release()
        {
            foreach (var obj in free)
            {
                Destroy(obj.gameObject, 0.0016f);
            }
            foreach (var obj in active)
            {
                Destroy(obj.gameObject, 0.0016f);
            }
            free.Clear();
            active.Clear();
        }
    }

    /// <summary>
    /// Ԥ������
    /// </summary>
    [System.Serializable]
    public class PreloadConfigs
    {
        [Header("��ʼԤ������")]
        public GameObject prefab;
        public int preloadCount = 100;
        [Header("Ԥ����·��( �Զ����� )")]
        [ReadOnly]
        public string url = string.Empty;
    }

    [SerializeField]
    private List<PreloadConfigs> preloadConfigs = new List<PreloadConfigs>();
    private Dictionary<string, TransformPool> poolDict = new Dictionary<string, TransformPool>();
    private static Dictionary<Transform, string> nameofDict = new Dictionary<Transform, string>();
    private static EasyObjectPool instance = null;
    public static EasyObjectPool GetInstance() { return instance; }
    /// <summary>
    /// �״μ����Ƿ����
    /// </summary>
    public static bool firstPreloadFinish
    {
        get
        {
            return instance != null && instance.preloadConfigs.Count == 0;
        }
    }

#if UNITY_EDITOR || ENABLE_LOG
    private void OnValidate()
    {
        foreach (var config in preloadConfigs)
        {
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(config.prefab))
            {
                string url = UnityEditor.AssetDatabase.GetAssetPath(config.prefab);
                config.url = url;
            }
        }
    }
#endif

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad( gameObject );

        //��һ������
        foreach (var config in preloadConfigs)
        {
            Add(config.prefab, config.preloadCount);
        }
        preloadConfigs.Clear();
    }

    /// <summary>
    /// �Ӷ��������һ�����õĶ���
    /// </summary>
    /// <param name="key"></param>
    /// <returns>������nullʱ ˵�����������Ԥ��ĳ��� �����ʹ�� GeneratePool �����һ���µĳ��� </returns>
    public Transform Spawn(string key)
    {

        TransformPool res = null;
        if (poolDict.TryGetValue(key, out res))
        {
            Transform trans = res.Pop();
            trans.gameObject.SetActive(true);
            return trans;
        }
        return null;
    }
    /// <summary>
    /// ����һ�����󵽶������
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Despawn(Transform obj)
    {
        if (null == obj)
        {
#if UNITY_EDITOR || ENABLE_LOG
            throw new System.Exception("Despawn obj is null");
#else
            return false;
#endif
        }

        if (nameofDict.TryGetValue(obj, out string name) && poolDict.TryGetValue(name, out TransformPool res))
        {
            res.Push_Back(obj);
            return true;
        }
        else
        {
            //�ݴ���
#if UNITY_EDITOR || ENABLE_LOG
            obj.gameObject.SetActive(false);
            Debug.LogError($"current object is not objectPool element: {obj.name}", obj.gameObject);
#else
            Destroy( obj.gameObject );
#endif
        }
        return false;
    }


    /// <summary>
    /// ��ָ��key�Ļ���������еĶ���ȫ������
    /// </summary>
    /// <param name="pool"></param>
    public void Despawn(string pool)
    {
        if (poolDict.TryGetValue(pool, out TransformPool res))
        {
            res.Recycle();
        }
#if UNITY_EDITOR || ENABLE_LOG
        else
        {
            Debug.LogError($"exclusive pool: {pool}");
        }
#endif
    }

    /// <summary>
    /// �ӳٻ���
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="delay"></param>
    public void Despawn(Transform obj, float delay)
    {
        Timer.SetTimeout(delay, () => Despawn(obj));
    }

    /// <summary>
    /// �Ƿ��Ƕ����Ԫ��
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool Contains(Transform element)
    {
        TransformPool pool;
        if (null != element && nameofDict.TryGetValue(element, out string name) && poolDict.TryGetValue(name, out pool) && pool.InSidePool(element))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// �����������еĶ����Ԫ�� ( �������� )
    /// </summary>
    /// <param name="root"></param>
    public void DespawnSelfAny<T>(Transform root) where T : Component
    {
        T[] suspectObjects = root.GetComponentsInChildren<T>();
        foreach (var obj in suspectObjects)
        {
            if (Contains(obj.transform))
            {
                Despawn(obj.transform);
            }
        }
    }


    /// <summary>
    /// �����Լ����ӽڵ� ����ӽڵ��Ƕ����Ԫ�صĻ�
    /// </summary>
    /// <param name="root"> ���ڵ� </param>
    /// <param name="includeSelf"> ���λ����Ƿ�������ڵ� </param>
    /// <param name="force"> true: �������еĺ��ӽڵ�  false: ������һ�� </param>
    public void DespawnChildren(Transform root, bool includeSelf = false, bool force = false)
    {
        List<Transform> children = null;

        if (force)
        {
            Transform[] suspectObjects = root.GetComponentsInChildren<Transform>();
            children = new List<Transform>(suspectObjects);
            if (!includeSelf) children.Remove(root);

        }
        else
        {
            children = new List<Transform>();
            if (includeSelf)
            {
                children.Add(root);
            }
            foreach (Transform child in root)
            {
                children.Add(child);
            }
        }

        foreach (var child in children)
        {
            Despawn(child);
        }
    }

    /// <summary>
    /// ����Ҫ�������
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="firstExpandCount"></param>
    public void Add(GameObject prefab, int firstExpandCount = 100)
    {
        if (prefab != null)
        {
            var key = prefab.name;
            if (poolDict.ContainsKey(key))
            {
                Debug.LogError($"Add Pool Error: pool name <{key}> already exist!");
#if UNITY_EDITOR || ENABLE_LOG
                Selection.activeGameObject = prefab;
#endif
                return;
            }
            var pool = new TransformPool(prefab, transform);
            poolDict.Add(key, pool);
            pool.Reserve(firstExpandCount);
#if UNITY_EDITOR || ENABLE_LOG
            Debug.Log($"<color=#00ff44>[EasyObjectPool]\t����ش����ɹ�: {key}\t��ǰ��������: {firstExpandCount}</color>");
#endif
        }
        else
        {
            Debug.LogError($"Add Pool Error: prefab is null");
        }
    }


    /// <summary>
    /// �������м������
    /// </summary>
    public void Recycle()
    {
        foreach (var pool in poolDict)
        {
            pool.Value.Recycle();
        }
    }

}


public class ReadOnlyAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR || ENABLE_LOG
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif
