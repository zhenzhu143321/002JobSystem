using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

struct JobResultAndHandle
{
    public NativeArray<Vector3> resultWaypoints;
    public JobHandle handle;
}

public class MyJobSchedulerMain : MonoBehaviour
{
    public Vector3[] srcWaypoints;//数据源
    public float offsetForWaypoints;

    //我们将保存结果和句柄的列表
    List<JobResultAndHandle> resultsAndHandlesList = new List<JobResultAndHandle>();

    void Start()
    {
        /*我们会在需要时创建新的JobResultANdHandle（该代码不必在Update方法中，因为
          它只是个示例）然后我们会给ScheduleJob方法提供引用。
        */
        JobResultAndHandle newResultAndHandle = new JobResultAndHandle();
        ScheduleJob(ref newResultAndHandle);


        /*如果ResultAndHAndles的列表非空，我们会在该列表进行循环，了解是否有需要调用的作业。*/
        if (resultsAndHandlesList.Count > 0)
        {
            for (int i = 0; i < resultsAndHandlesList.Count; i++)
            {
                CompleteJob(resultsAndHandlesList[i]);
            }
        }
    }
    // ScheduleJob会获取JobResultAndHandle的引用，初始化并调度作业。
    void ScheduleJob(ref JobResultAndHandle resultAndHandle)
    {
        //我们会填充内置数组，设置合适的分配器
        resultAndHandle.resultWaypoints = new NativeArray<Vector3>(srcWaypoints, Allocator.TempJob);

        //我们会初始化作业，提供需要的数据
        MyParallerForJob newJob = new MyParallerForJob
        {
            waypoints = resultAndHandle.resultWaypoints,
            offsetToAdd = offsetForWaypoints,
        };

        //设置作业句柄并调度作业
        resultAndHandle.handle = newJob.Schedule(resultAndHandle.resultWaypoints.Length, 1);
        resultsAndHandlesList.Add(resultAndHandle);
    }

    //完成后，我们会复制作业中处理的数据，然后弃用弃用内置数组
    //这一步很有必要，因为我们需要释放内存
    void CompleteJob(JobResultAndHandle resultAndHandle)
    {
        resultsAndHandlesList.Remove(resultAndHandle);
        resultAndHandle.handle.Complete();
        resultAndHandle.resultWaypoints.CopyTo(srcWaypoints);
        resultAndHandle.resultWaypoints.Dispose();
    }


}
