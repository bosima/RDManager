using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RDManager
{
    /// <summary>
    /// 表头排序类
    /// From http://www.cnblogs.com/zhangqs008/p/3773633.html
    /// </summary>
    public class ListViewSort : IComparer
    {
        private readonly int _col;
        private readonly bool _descK;

        public ListViewSort()
        {
            _col = 0;
        }

        public ListViewSort(int column, object desc)
        {
            _descK = (bool)desc;
            _col = column; //当前列,0,1,2...,参数由ListView控件的ColumnClick事件传递    
        }

        public int Compare(object x, object y)
        {
            var lvi_x = (ListViewItem)x;
            var lvi_y = (ListViewItem)y;

            if (lvi_x.Text == "..")
            {
                return -1000;
            }

            int tempInt = String.CompareOrdinal(lvi_x.SubItems[_col].Text, lvi_y.SubItems[_col].Text);

            if (_descK)
            {
                return -tempInt;
            }

            return tempInt;
        }
    }
}
