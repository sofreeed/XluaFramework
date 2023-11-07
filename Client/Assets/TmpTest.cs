using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TmpTest : MonoBehaviour
{

    public string abc = "";
    
    // Start is called before the first frame update
    void Start()
    {
        //ListNode node5 = new ListNode(5);
        //ListNode node4 = new ListNode(4, node5);
        //ListNode node3 = new ListNode(3, node4);
        //ListNode node2 = new ListNode(2, node3);
        //ListNode node1 = new ListNode(1, node2);
//
        //ListNode res = ReverseList(node1);

        char[] chars = new char[2];
        chars[0] = '胡';
        chars[1] = 'b';

        Debug.LogError(Encoding.ASCII.GetBytes(chars));
        //Debug.LogError(b.GetHashCode());
    }

    public ListNode ReverseList(ListNode head)
    {
        if (head == null)
            return null;

        ListNode pre = head;
        ListNode curr = pre.next;
        ListNode tmp = null;
        while (curr != null)
        {
            tmp = curr.next;

            curr.next = pre;

            pre = curr;
            curr = tmp;
        }

        return pre;
    }

    public class MyLinkedList
    {
        private MyLinkedListNode head = null;
        private int N = 0; //个数

        public MyLinkedList()
        {
        }

        public int Get(int index)
        {
            if (index >= N || index < 0)
            {
                return -1;
            }

            MyLinkedListNode curr = head;
            for (int i = 0; i < index; i++)
            {
                curr = curr.next;
            }

            return curr.val;
        }

        public void AddAtHead(int val)
        {
            AddAtIndex(0, val);
        }

        public void AddAtTail(int val)
        {
            AddAtIndex(N, val);
        }

        public void AddAtIndex(int index, int val)
        {
            if (index > N || index < 0)
            {
                return;
            }

            if (index == 0)
            {
                MyLinkedListNode node1 = new MyLinkedListNode(val, head);
                head = node1;
                N++;
                return;
            }

            MyLinkedListNode curr = head;
            MyLinkedListNode pre = null;
            for (int i = 0; i < index; i++)
            {
                pre = curr;
                curr = curr.next;
            }

            MyLinkedListNode node = new MyLinkedListNode(val, curr);
            pre.next = node;
            N++;
        }

        public void DeleteAtIndex(int index)
        {
            if (index >= N || index < 0)
            {
                return;
            }

            if (index == 0)
            {
                head = head.next;
                N--;
                return;
            }

            MyLinkedListNode curr = head;
            MyLinkedListNode pre = null;
            for (int i = 0; i < index; i++)
            {
                pre = curr;
                curr = curr.next;
            }

            pre.next = curr.next;
            N--;
        }
    }

    public class MyLinkedListNode
    {
        public int val;
        public MyLinkedListNode prev;
        public MyLinkedListNode next;

        public MyLinkedListNode(int val, MyLinkedListNode next)
        {
            this.val = val;
            this.next = next;
        }
    }

    public class ListNode
    {
        public int val;
        public ListNode next;

        public ListNode(int val = 0, ListNode next = null)
        {
            this.val = val;
            this.next = next;
        }
    }

    public ListNode RemoveElements(ListNode head, int val)
    {
        ListNode curr = head;
        ListNode pre = null;
        while (curr != null)
        {
            if (curr.val == val)
            {
                if (curr == head)
                {
                    head = curr.next;
                    curr = head;
                }
                else
                {
                    pre.next = curr.next;
                    curr = curr.next;
                }
            }
            else
            {
                pre = curr;
                curr = curr.next;
            }
        }

        return head;
    }

    //螺旋数组
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

    //长度最小子数组
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