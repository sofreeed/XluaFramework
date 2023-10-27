using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //int[] array = { 2, 3, 1, 2, 4, 3 };
        //MinSubArrayLen(7, array);

        GenerateMatrix(3);
    }

    public int[][] GenerateMatrix(int n)
    {
        int[][] array = new int[n][];
        for (int i = 0; i < n; i++)
        {
            array[i] = new int[n];
        }

        int xMin = 0;
        int yMin = 0;
        int xMax = n - 1;
        int yMax = n - 1;

        int val = 0;

        int currX = 0;
        int currY = 0;

        bool xRise = true;
        bool yRise = true;

        while (true)
        {
            val++;
            array[currY][currX] = val;
            if (val == n * n)
                break;

            if (xRise && yRise)
            {
                currX++;
                if (currX > xMax)
                {
                    currX--;
                    yMin++;
                    currY = yMin;
                    xRise = false;
                }
            }
            else if (!xRise && yRise)
            {
                currY++;
                if (currY > yMax)
                {
                    currY--;
                    xMax--;
                    currX = xMax;
                    yRise = false;
                }
            }
            else if (!xRise && !yRise)
            {
                currX--;
                if (currX < xMin)
                {
                    currX++;
                    yMax--;
                    currY = yMax;
                    xRise = true;
                }
            }
            else if (xRise && !yRise)
            {
                currY--;
                if (currY < yMin)
                {
                    currY++;
                    xMin++;
                    currX = xMin;
                    yRise = true;
                }
            }
        }

        return array;
    }

    public int MinSubArrayLen(int target, int[] nums)
    {
        int minCount = int.MaxValue;
        int sliderNum = nums[0];
        int j = 0;

        for (int i = 0; i < nums.Length; i++)
        {
            if (i > 0)
            {
                sliderNum -= nums[i - 1];
            }

            while (sliderNum < target)
            {
                j++;
                if (j >= nums.Length)
                {
                    break;
                }

                sliderNum += nums[j];
            }

            if (sliderNum >= target)
            {
                int tmpMinCount = j - i + 1;
                minCount = minCount < tmpMinCount ? minCount : tmpMinCount;
            }
        }

        if (minCount == int.MaxValue)
        {
            return 0;
        }
        else
        {
            return minCount;
        }
    }
}