using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Paint_
{
    // Enum khi di chuột vào 1 item
    public enum HitType
    {
        None, Body, UL, UR, LR, LL, L, R, T, B
    };
    public class Selector
    {
        #region Member
        public ItemLayer itemChoose;
        //Type chuột
        HitType MouseHitType = HitType.None;
        //Điểm cuối click chuột
        public Point LastPoint;
        //Đang drag
        public bool DragInProgress = false;
        Canvas MainCanvas;
        #endregion

        #region Contructor
        public Selector(Canvas cv)
        {
            MainCanvas = cv;
        }
        #endregion

        #region Function
        public void OnMouse_Move()
        {
            if (!DragInProgress)
            {
                if (itemChoose != null)
                {
                    MouseHitType = SetHitType(Mouse.GetPosition(MainCanvas));
                    SetMouseCursor();
                }
            }
            else
            {
                if (itemChoose == null) {
                    MouseHitType = HitType.None;
                    return; 
                }
                // Lấy vị trí chuột
                Point point = Mouse.GetPosition(MainCanvas);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;
               
                // Lấy vị trí
                double new_x = Canvas.GetLeft(itemChoose);
                double new_y = Canvas.GetTop(itemChoose);
                double new_width = itemChoose.Width;
                double new_height = itemChoose.Height;
             
                // Cập nhật loại mouse
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;
                    case HitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        if (new_x + new_width < new_x)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.UR;
                        }
                        else if (new_y + new_height < new_y)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.LL;
                        }

                        if (new_width < 0)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.UL;
                        }
                        else if (new_height < 0)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.LL;
                        }
                        break;
                    case HitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        if (new_x + new_width < new_x)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.UL;
                        }
                        else if (new_y + new_height < new_y)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.LR;
                        }
                        if (new_width < 0)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.UR;
                        }
                        else if (new_height < 0)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.LR;
                        }
                        break;
                    case HitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        if (new_width < 0)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.LL;
                        }
                        else if (new_height < 0)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.UR;
                        }
                        break;
                    case HitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        if (new_x + new_width < new_x)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.LR;
                        }
                        else if (new_y + new_height < new_y)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.UL;
                        }
                        if (new_width < 0)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.LL;
                        }
                        else if (new_height < 0)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.UL;
                        }
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        if (new_x + new_width < new_x)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.R;
                        }
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        if (new_width < 0)
                        {
                            new_width = Math.Abs(new_width);
                            new_x -= new_width;
                            MouseHitType = HitType.L;
                        }
                        break;
                    case HitType.B:
                        new_height += offset_y;
                        if (new_height < 0)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.T;
                        }
                        break;
                    case HitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        if (new_y + new_height < new_y)
                        {
                            new_height = Math.Abs(new_height);
                            new_y -= new_height;
                            MouseHitType = HitType.B;
                        }
                        break;
                }
                
                if (itemChoose.Type == TypeShape.Line && (MouseHitType == HitType.UL || MouseHitType == HitType.LR))
                {
                    itemChoose.ChangePart = false;
                }
                else if (itemChoose.Type == TypeShape.Line && (MouseHitType == HitType.UR || MouseHitType == HitType.LL))
                {
                    itemChoose.ChangePart = true;
                }
                else if (itemChoose.Type == TypeShape.Line && MouseHitType != HitType.Body)
                {
                    return;
                }
                if (itemChoose.Type == TypeShape.PolyLine)
                {
                    MouseHitType = HitType.None;
                }
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && (itemChoose.Type == TypeShape.Rectangle || itemChoose.Type == TypeShape.Ellipse))
                {
                    if (HitType.LR == MouseHitType || MouseHitType == HitType.UR)
                    {
                        new_width = new_height;
                    }
                    else
                    {
                        new_x += new_width - new_height;
                        new_width = new_height;
                    }
                }
                // Update 
                Canvas.SetLeft(itemChoose, new_x);
                Canvas.SetTop(itemChoose, new_y);
                itemChoose.Width = Math.Abs(new_width);
                itemChoose.Height = Math.Abs(new_height);

                // Save location.
                LastPoint = point;

            }
        }

        public void OnMouse_Up()
        {
            DragInProgress = false;
        }

        public void OnMouse_Down()
        {
            //MouseHitType = SetHitType(Mouse.GetPosition(MainCanvas));
            SetMouseCursor();
            if (MouseHitType == HitType.None) return;

            LastPoint = Mouse.GetPosition(MainCanvas);
            DragInProgress = true;
        }

        public void SetItemSelector(ItemLayer item)
        {
            if (item != null)
            {
                itemChoose = item;
                itemChoose.Selected = true;
            }
        }

        public void SetItemCreate(ItemLayer item)
        {
            MouseHitType = HitType.LR;
            if (item.Type == TypeShape.PolyLine) MouseHitType = HitType.None;
            DragInProgress = true;
            SetMouseCursor();
            LastPoint = Mouse.GetPosition(MainCanvas);
            item.Selected = true;
            itemChoose = item;
        }
        // Cài đặt chuột
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Mouse.OverrideCursor != desired_cursor) Mouse.OverrideCursor = desired_cursor;
        }
        // Lấy Type chuột
        private HitType SetHitType(Point point)
        {
            return itemChoose.GetPartOver();
          



            if (itemChoose == null) return HitType.None;
            double left = Canvas.GetLeft(itemChoose);
            double top = Canvas.GetTop(itemChoose);
            double right = left + itemChoose.Width;
            double bottom = top + itemChoose.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                if(itemChoose.Type != TypeShape.Line) return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                if (itemChoose.Type != TypeShape.Line) return HitType.R;
            }


            if (itemChoose.Type == TypeShape.Line)
            {
                if (itemChoose.GetMouseOver())
                    return HitType.Body;
                else
                    return HitType.None;
            }

            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }
        #endregion

    }
}
