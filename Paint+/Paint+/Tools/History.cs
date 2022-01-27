using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Paint_
{
    public class History
    {
        //public Image MainImage;
        List<string> ListData = new List<string>();
        List<string> ListTemp = new List<string>();

        public History()
        {
            PushBack("");
        }

        public void PushBack(string item)
        {
            ListData.Add(item);
        }

        public string Undo()
        {
            if (ListData.Count > 1)
            {
                string ms = ListData[ListData.Count - 1];
                string result = ListData[ListData.Count - 2];
                ListTemp.Add(ms);
                ListData.Remove(ms);
                return result;
            }
            return null;
        }

        public void ClearTemp()
        {
            if (ListTemp.Count > 0)
            {
                ListTemp.Clear();
            }
        }
        public void ClearMain()
        {
            if (ListData.Count > 0)
            {
                ListData.Clear();
            }
            PushBack("");
        }
        public string Redo()
        {
            if (ListTemp.Count > 0)
            {
                string ms = ListTemp[ListTemp.Count - 1];
                ListData.Add(ms);
                ListTemp.Remove(ms);

                return ms;
            }
            return null;
        }
    }
}
