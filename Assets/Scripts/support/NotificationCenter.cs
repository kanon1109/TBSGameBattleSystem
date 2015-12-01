using System;
using System.Collections.Generic;
namespace support
{
public class NotificationCenter
{
    //单例
    private static NotificationCenter instance;
    //方法回调类型
    public delegate void HandlerDelegate(Object param);
    //存放回调列表的字典
    private Dictionary<String, List<HandlerDelegate>> dict;
    public NotificationCenter()
    {
        this.dict = new Dictionary<string, List<HandlerDelegate>>();
    }

    /// <summary>
    /// 获取单例
    /// </summary>
    /// <returns></returns>
    public static NotificationCenter getInstance()
    {
        if(instance == null) instance = new NotificationCenter();
        return instance;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="name">消息名称</param>
    /// <param name="obj">消息所带参数</param>
    /// <returns></returns>
    public void postNotification(String name, Object obj)
    {
        if (!this.dict.ContainsKey(name)) return;
        List<HandlerDelegate> delegateList = this.dict[name];
        int count = delegateList.Count;
        for (int i = 0; i < count; ++i)
        {
            HandlerDelegate handler = delegateList[i];
            handler.Invoke(obj);
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="name">消息名称</param>
    /// <returns></returns>
    public void postNotification(String name)
    {
        this.postNotification(name, null);
    }

    /// <summary>
    /// 添加消息观察者
    /// </summary>
    /// <param name="name">消息名称</param>
    /// <param name="handler">消息的回调</param>
    /// <returns></returns>
    public void addObserver(String name, HandlerDelegate handler)
    {
        List<HandlerDelegate> delegateList;
        if (!this.dict.ContainsKey(name))
        {
            //根据消息名称创建回调列表
            delegateList = new List<HandlerDelegate>();
            delegateList.Add(handler);
            this.dict.Add(name, delegateList);
        }
        else
        {
            //已经创建了回调列表则直接使用
            delegateList = this.dict[name];
            delegateList.Add(handler);
        }
    }

    /// <summary>
    /// 删除此消息的所有观察者
    /// </summary>
    /// <param name="name">消息名称</param>
    /// <returns></returns>
    public void removeObserver(String name)
    {
        if (!this.dict.ContainsKey(name)) return;
        List<HandlerDelegate> delegateList = this.dict[name];
        delegateList.Clear();
    }
    
    /// <summary>
    /// 删除所有观察者
    /// </summary>
    /// <returns></returns>
    public void removeObservers()
    {
        this.dict.Clear();
    }
}
}
