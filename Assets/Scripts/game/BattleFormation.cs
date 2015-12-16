using System.Collections.Generic;
using UnityEngine;
public class BattleFormation
{
    //横向间隔（x坐标间隔）
    public static int gapH = 5;
    //纵向间隔（z坐标间隔）
    public static int gapV = 5;
    //左边阵型的起始位置
    public static Vector3 leftStartPos = new Vector3(-5, 0, 45);
    //右边阵型的起始位置
    public static Vector3 rightStartPos = new Vector3(10, 0, 45);
    //左边阵型的位置矩阵
    public static List<int[]> leftMatrix;
    //右边阵型的位置矩阵
    public static List<int[]> rightMatrix;
    //左边位置索引（用于计算距离）
    public static List<int[]> leftPosIndex;
    //右边位置索引（用于计算距离）
    public static List<int[]> rightPosIndex;
    //阵位最大人数
    public static int MAX_ROLE_COUNT = 6;
    /// <summary>
    /// 初始化阵型
    /// </summary>
    public static void init()
    {
        leftMatrix = new List<int[]>();
        rightMatrix = new List<int[]>();
        leftPosIndex = new List<int[]>();
        rightPosIndex = new List<int[]>();

        //左边位置
        leftMatrix.Add(new int[] { 1, 1 });
        leftMatrix.Add(new int[] { 1, 2 });
        leftMatrix.Add(new int[] { 1, 3 });

        leftMatrix.Add(new int[] { 0, 1 });
        leftMatrix.Add(new int[] { 0, 2 });
        leftMatrix.Add(new int[] { 0, 3 });

        //右边位置
        rightMatrix.Add(new int[] { 1, 1 });
        rightMatrix.Add(new int[] { 1, 2 });
        rightMatrix.Add(new int[] { 1, 3 });

        rightMatrix.Add(new int[] { 2, 1 });
        rightMatrix.Add(new int[] { 2, 2 });
        rightMatrix.Add(new int[] { 2, 3 });

        //左边位置索引
        leftPosIndex.Add(new int[] { 2, 1 });
        leftPosIndex.Add(new int[] { 2, 2 });
        leftPosIndex.Add(new int[] { 2, 3 });

        leftPosIndex.Add(new int[] { 1, 1 });
        leftPosIndex.Add(new int[] { 1, 2 });
        leftPosIndex.Add(new int[] { 1, 3 });

        //右边位置索引
        rightPosIndex.Add(new int[] { 10, 1 });
        rightPosIndex.Add(new int[] { 10, 2 });
        rightPosIndex.Add(new int[] { 10, 3 });

        rightPosIndex.Add(new int[] { 11, 1 });
        rightPosIndex.Add(new int[] { 11, 2 });
        rightPosIndex.Add(new int[] { 11, 3 });
    }

    
}