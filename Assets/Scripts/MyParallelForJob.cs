using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using System.Collections;

public struct MyParallerForJob : IJobParallelFor
{

    /*在作业中，需要定义所有用于执行作业和输出结果的数据
    Unity会创建内置数组，它们大体上和普通数组差不多，但是
    需要自己处理分配和释放设置*/

    public NativeArray<Vector3> waypoints;
    public float offsetToAdd;
    /*所有作业都需要Execute函数*/
    public void Execute(int i)
    {

        //该函数会保存行为。要执行的变量必须在该struct开头定义
        waypoints[i] = waypoints[i] * offsetToAdd;
    }
}
