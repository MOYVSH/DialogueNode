using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// Timer
/// ��ǰ��ʱ������Ϊ��Ϸ������ʵʱ�� ����Time.timeScale��Ӱ�� �м�
/// 
/// ����1: 1������һ��
/// Timer.SetTimeout(1.0f, () =>
/// {
///     Debug.Log("after 1 second.");
/// });
/// 
/// 
/// ����2: ÿ1�����һ�� �����ƴ��� ֱ�������˳���ֹͣ
/// int i = 0;
/// Timer.SetInterval(1.0f, () =>
/// {
///     Debug.Log(i++);
/// });
/// 
/// ����3: ÿ1�����һ�� ����10�κ�ֹͣ
/// int i = 0;
/// Timer.SetInterval(1.0f, () =>
/// {
///     Debug.Log(i++);
/// }, 10);
/// 
/// 
/// ����4: ��ǰ֡��ε��ý��ڽ���ִ֡��һ�� 
/// for ( int i = 0; i < 1000000; i++ )
/// {
///     Timer.CallerLate( () =>
///     {
///         Debug.Log( "######### hello world" );
///     } );
/// }
///  
/// Anchor: ChenJC
/// Time: 2022/10/09
/// ԭ��: https://blog.csdn.net/qq_39162566/article/details/113105351
/// </summary>
public class Timer : MonoBehaviour
{

    //��ʱ��������
    public class TimerTask
    {
        public int tag;
        public float tm;
        public float life;
        public long count;
        public Action func;
        public TimerTask Clone()
        {
            var timerTask = new TimerTask();
            timerTask.tag = tag;
            timerTask.tm = tm;
            timerTask.life = life;
            timerTask.count = count;
            timerTask.func = func;
            return timerTask;
        }
        public void Destory()
        {
            m_freeTaskCls.Enqueue(this);
        }
    }


    #region Member property

    protected static List<TimerTask> m_activeTaskCls = new List<TimerTask>();//�����е�TimerTask����
    protected static Queue<TimerTask> m_freeTaskCls = new Queue<TimerTask>();//����TimerTask����
    protected static HashSet<Action> lateChannel = new HashSet<Action>();//ȷ��callLate���õ�Ψһ��
    protected static int m_tagCount = 1000; //timer��Ψһ��ʶ
    protected static bool m_inited = false; //��ʼ��
    protected bool m_isBackground = false;//�Ƿ���Ժ�̨���� false���˵���̨ʱ��ʱ��ֹͣ���� 

    #endregion


    #region public methods

    //ÿ֡����ʱִ�лص� : ��ǰ֡�ڵĶ�ε��ý��ڵ�ǰ֡������ʱ��ִ��һ��
    public static void CallerLate(Action func)
    {
        if (!lateChannel.Contains(func))
        {
            lateChannel.Add(func);
            SetTimeout(0f, func);
        }
    }


    //delay��� ִ��һ�λص�
    public static int SetTimeout(float delay, Action func)
    {
        return SetInterval(delay, func, false, 1);
    }

    /// <summary>
    /// �����Զ�ʱ�� ���һ��ʱ�����һ��
    /// </summary>
    /// <param name="interval"> ���ʱ��: ��</param>
    /// <param name="func"> ���õķ����ص� </param>
    /// <param name="immediate"> �Ƿ�����ִ��һ�� </param>
    /// <param name="times"> ���õĴ���: Ĭ������ѭ�� ��ֵ<=0ʱ��һֱ���µ��� ��ֵ>0ʱ ѭ��ָ�������� ֹͣ���� </param>
    /// <returns></returns>
    public static int SetInterval(float interval, Action func, bool immediate = false, int times = 0)
    {
        //��free���� ��ȡһ�����õ�TimerTask����
        var timer = GetFreeTimerTask();
        timer.tm = 0;
        timer.life = interval;
        timer.func = func;
        timer.count = times;
        timer.tag = ++m_tagCount;

        //���Գ�ʼ��
        Init();

        //����ִ��һ��
        if (immediate)
        {
            --timer.count;
            func?.Invoke();
            if (timer.count == 0)
            {

                timer.Destory();
            }
            else
            {
                //��ӵ��������
                m_activeTaskCls.Add(timer);
            }
        }
        else
        {
            //��ӵ��������
            m_activeTaskCls.Add(timer);
        }

        return m_tagCount;
    }

    #endregion


    #region Get Timer methods

    /// <summary>
    /// ͨ��Tag��ȡ��ʱ������
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static TimerTask GetTimer(int tag)
    {
        return m_activeTaskCls.Find((TimerTask t) =>
        {
            return t.tag == tag;
        })?.Clone();
    }

    /// <summary>
    /// ͨ��Tag��ȡ��ʱ������
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static TimerTask GetTimer(Action func)
    {
        return m_activeTaskCls.Find((TimerTask t) =>
        {
            return t.func == func;
        })?.Clone();
    }
    #endregion


    #region Clean Timer methods

    /// <summary>
    /// ͨ��ID ����ʱ��
    /// </summary>
    /// <param name="tag">��ʱ����ǩ</param>
    /// <returns></returns>
    public static void ClearTimer(int tag)
    {
        int index = m_activeTaskCls.FindIndex((TimerTask t) =>
        {
            return t.tag == tag;
        });

        if (index != -1)
        {
            var t = m_activeTaskCls[index];
            if (lateChannel.Count != 0 && lateChannel.Contains(t.func))
            {
                lateChannel.Remove(t.func);
            }
            m_activeTaskCls.RemoveAt(index);
            m_freeTaskCls.Enqueue(t);
        }
    }

    /// <summary>
    /// ͨ������ ����ʱ��
    /// </summary>
    /// <param name="func">������</param>
    /// <returns></returns>
    public static void ClearTimer(Action func)
    {
        int index = m_activeTaskCls.FindIndex((TimerTask t) =>
        {
            return t.func == func;
        });

        if (index != -1)
        {
            var t = m_activeTaskCls[index];
            if (lateChannel.Count != 0 && lateChannel.Contains(t.func))
            {
                lateChannel.Remove(t.func);
            }
            m_activeTaskCls.RemoveAt(index);
            m_freeTaskCls.Enqueue(t);
        }
    }

    /// <summary>
    /// �������ж�ʱ��
    /// </summary>
    public static void ClearTimers()
    {
        lateChannel.Clear();
        m_activeTaskCls.ForEach(timer => m_freeTaskCls.Enqueue(timer));
        m_activeTaskCls.Clear();
    }

    #endregion


    #region System methods

    //Update����֮ǰ
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StopAllCoroutines();
        StartCoroutine(TimerElapse());
    }

    //�����л�����̨
    private void OnApplicationPause(bool pause)
    {
        if (!m_isBackground)
        {
            if (pause)
            {
                StopAllCoroutines();
            }
            else
            {

                StopAllCoroutines();
                StartCoroutine(TimerElapse());
            }
        }
    }

    //��ʱ������
    private IEnumerator TimerElapse()
    {

        TimerTask t = null;

        while (true)
        {
            if (m_activeTaskCls.Count > 0)
            {
                float dt = Time.unscaledDeltaTime;
                for (int i = 0; i < m_activeTaskCls.Count; ++i)
                {
                    t = m_activeTaskCls[i];
                    t.tm += Time.unscaledDeltaTime;
                    if (t.tm >= t.life)
                    {
                        t.tm -= t.life;
                        if (t.count == 1)
                        {
                            m_activeTaskCls.RemoveAt(i--);
                            if (lateChannel.Count != 0 && lateChannel.Contains(t.func))
                            {
                                lateChannel.Remove(t.func);
                            }
                            t.Destory();
                        }
                        --t.count;
                        t.func();
                    }
                }
            }
            yield return 0;
        }
    }

    //��ʼ��
    protected static void Init()
    {
        if (!m_inited)
        {
            m_inited = true;
            var inst = new GameObject("TimerNode");
            inst.AddComponent<Timer>();
        }
    }

    //��ȡ���ö�ʱ��
    protected static TimerTask GetFreeTimerTask()
    {
        if (m_freeTaskCls.Count > 0)
        {
            return m_freeTaskCls.Dequeue();
        }
        return new TimerTask();
    }

    #endregion

}