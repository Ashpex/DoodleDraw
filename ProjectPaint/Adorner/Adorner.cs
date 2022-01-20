using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ProjectPaint
{
    public class RectangleAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        public Thumb topEdge, bottomEdge, leftEdge, rightEdge;
        public Thumb topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner;
        public Thumb moveThumb;
        public Thumb rotateThumb;

        // To store and manage the adorner's visual children.
        VisualCollection visualChildren;

        //=================================================================================================
        // Initialize the ResizingAdorner.
        private void InitializeComponents(FrameworkElement adornedElement)
        {
            visualChildren = new VisualCollection(this);

            adornedElement.RenderTransformOrigin = new Point(0.5, 0.5);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerThumb(ref topEdge, Cursors.SizeNS);
            BuildAdornerThumb(ref bottomEdge, Cursors.SizeNS);
            BuildAdornerThumb(ref leftEdge, Cursors.SizeWE);
            BuildAdornerThumb(ref rightEdge, Cursors.SizeWE);

            BuildAdornerThumb(ref topLeftCorner, Cursors.SizeNWSE);
            BuildAdornerThumb(ref topRightCorner, Cursors.SizeNESW);
            BuildAdornerThumb(ref bottomLeftCorner, Cursors.SizeNESW);
            BuildAdornerThumb(ref bottomRightCorner, Cursors.SizeNWSE);

            BuildAdornerThumb(ref rotateThumb, Cursors.Hand);

            //Build move thumb
            moveThumb = new Thumb()
            {
                Cursor = Cursors.SizeAll,
                SnapsToDevicePixels = true,
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                Opacity = 0
            };
            moveThumb.Width = adornedElement.Width;
            moveThumb.Height = adornedElement.Height;
            visualChildren.Add(moveThumb);


            // Add handlers for resizing.
            topEdge.DragDelta += new DragDeltaEventHandler(HandleResize);
            bottomEdge.DragDelta += new DragDeltaEventHandler(HandleResize);
            leftEdge.DragDelta += new DragDeltaEventHandler(HandleResize);
            rightEdge.DragDelta += new DragDeltaEventHandler(HandleResize);

            bottomLeftCorner.DragDelta += new DragDeltaEventHandler(HandleResize);
            bottomRightCorner.DragDelta += new DragDeltaEventHandler(HandleResize);
            topLeftCorner.DragDelta += new DragDeltaEventHandler(HandleResize);
            topRightCorner.DragDelta += new DragDeltaEventHandler(HandleResize);

            //Add handler for dragging
            moveThumb.DragDelta += new DragDeltaEventHandler(HandleMove);

            //Add handler for rotating
            rotateThumb.DragStarted += new DragStartedEventHandler(HandleBeginRotate);
            rotateThumb.DragDelta += new DragDeltaEventHandler(HandleRotating);
        }

        //--------------------------------------------
        //Ctors
        public RectangleAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            InitializeComponents(adornedElement);
        }

        //=================================================================================================
        // Handler for resizing.
        private void HandleResize(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

        

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.

            double Left = Canvas.GetLeft(adornedElement);
            double Top = Canvas.GetTop(adornedElement);

            if (hitThumb == topEdge || hitThumb == topLeftCorner || hitThumb == topRightCorner)
            {
                double ActualChange = adornedElement.Height - Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
                Canvas.SetTop(adornedElement, Top + ActualChange);
                adornedElement.Height = adornedElement.Height - ActualChange;
            }
            else if (hitThumb != leftEdge && hitThumb != rightEdge)
                adornedElement.Height = Math.Max(adornedElement.Height + args.VerticalChange, hitThumb.DesiredSize.Height);


            if (hitThumb == leftEdge || hitThumb == topLeftCorner || hitThumb == bottomLeftCorner)
            {
                double ActualChange = adornedElement.Width - Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
                Canvas.SetLeft(adornedElement, Left + ActualChange);
                adornedElement.Width = adornedElement.Width - ActualChange;
            }
            else if (hitThumb != topEdge && hitThumb != bottomEdge)
                adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
        }

        // Handler for moving.
        private void HandleMove(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            Point dragDelta = new Point(args.HorizontalChange, args.VerticalChange);
            RotateTransform rotateTransform = adornedElement.RenderTransform as RotateTransform;
            if (rotateTransform != null)
                dragDelta = rotateTransform.Transform(dragDelta);

            Canvas.SetLeft(adornedElement, Canvas.GetLeft(adornedElement) + dragDelta.X);
            Canvas.SetTop(adornedElement, Canvas.GetTop(adornedElement) + dragDelta.Y);
        }

        //Handler for rotating
        Point centerPoint;
        Vector startVector;
        double initialAngle;
        private void HandleBeginRotate(object sender, DragStartedEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            Canvas cv = adornedElement.Parent as Canvas;
            this.centerPoint = adornedElement.TranslatePoint(
                        new Point(adornedElement.Width * adornedElement.RenderTransformOrigin.X,
                                  adornedElement.Height * adornedElement.RenderTransformOrigin.Y),
                                  cv);

            Point startPoint = Mouse.GetPosition(cv);
            this.startVector = Point.Subtract(startPoint, this.centerPoint);

            if (adornedElement.RenderTransform as RotateTransform == null)
            {
                adornedElement.RenderTransform = new RotateTransform(0);
                this.initialAngle = 0;
            }
            else
            {
                this.initialAngle = (adornedElement.RenderTransform as RotateTransform).Angle;
            }

        }
        private void HandleRotating(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            Canvas cv = adornedElement.Parent as Canvas;
            Point currentPoint = Mouse.GetPosition(cv);
            Vector deltaVector = Point.Subtract(currentPoint, this.centerPoint);

            double angle = Vector.AngleBetween(this.startVector, deltaVector);

            RotateTransform rotateTransform = adornedElement.RenderTransform as RotateTransform;
            rotateTransform.Angle = this.initialAngle + Math.Round(angle, 0);
            adornedElement.InvalidateMeasure();
        }

        //=================================================================================================
        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well.
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            if (topEdge != null)
                topEdge.Arrange(new Rect(desiredWidth / 2 - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            if (bottomEdge != null)
                bottomEdge.Arrange(new Rect(desiredWidth / 2 - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            if (leftEdge != null)
                leftEdge.Arrange(new Rect(-adornerWidth / 2, desiredHeight / 2 - adornerHeight / 2, adornerWidth, adornerHeight));
            if (rightEdge != null)
                rightEdge.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight / 2 - adornerHeight / 2, adornerWidth, adornerHeight));

            if (topLeftCorner != null)
                topLeftCorner.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            if (topRightCorner != null)
                topRightCorner.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            if (bottomLeftCorner != null)
                bottomLeftCorner.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            if (bottomRightCorner != null)
                bottomRightCorner.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            if (moveThumb != null)
            {
                moveThumb.Arrange(new Rect(0, 0, adornerWidth, adornerHeight));
                moveThumb.Width = adornerWidth;
                moveThumb.Height = adornerHeight;
            }

            //Arrange Rotate Thumb
            rotateThumb.Arrange(new Rect(desiredWidth / 2 - adornerWidth / 2, -adornerHeight / 2 - 20, adornerWidth, adornerHeight));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerThumb(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            //cornerThumb.RenderTransform = rotateTransform;
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 6;
            cornerThumb.SnapsToDevicePixels = true;
            cornerThumb.BorderThickness = new Thickness(1);
            cornerThumb.BorderBrush = new SolidColorBrush(Colors.Black);
            cornerThumb.Background = new SolidColorBrush(Colors.White);

            visualChildren.Add(cornerThumb);
        }
       
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override void OnRender(DrawingContext drawingContext)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            if (adornedElement == null) return;

            //adornedElement.Parent.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Unspecified);
            Pen BorderPen = new Pen(Brushes.Blue, 1) { DashStyle = DashStyles.Dash };
            drawingContext.DrawRectangle(Brushes.Transparent, BorderPen, new Rect(0, 0, adornedElement.Width, adornedElement.Height));
        }
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }

    public class LineAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        public Thumb p1, p2;

        // To store and manage the adorner's visual children.
        VisualCollection visualChildren;

        //=================================================================================================
        //Ctors
        public LineAdorner(Line adornedElement) 
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);

            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerThumb(ref p1, Cursors.SizeNS);
            BuildAdornerThumb(ref p2, Cursors.SizeNS);


            // Add handlers for resizing.
            p1.DragDelta += new DragDeltaEventHandler(HandleResize);
            p2.DragDelta += new DragDeltaEventHandler(HandleResize); 
        }


        //=================================================================================================
        // Handler for resizing.
        private void HandleResize(object sender, DragDeltaEventArgs args)
        {
            Line adornedElement = this.AdornedElement as Line;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            //FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            if (hitThumb == p1)
            {
                adornedElement.X1 += args.HorizontalChange;
                adornedElement.Y1 += args.VerticalChange;
            }
            else //if (hitThumb == p1)
            {
                adornedElement.X2 += args.HorizontalChange;
                adornedElement.Y2 += args.VerticalChange;
            }
        }

        //=================================================================================================
        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            Line adornedElement = this.AdornedElement as Line;

            if (p1 != null)
                p1.Arrange(new Rect(adornedElement.X1 - p1.Width/2, adornedElement.Y1 - p1.Height/2, p1.Width, p1.Height));

            if (p1 != null)
                p2.Arrange(new Rect(adornedElement.X2 - p2.Width/2, adornedElement.Y2 - p2.Height/2, p2.Width, p2.Height));
            
            // Return the final size.
            return finalSize;
        }


        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerThumb(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 6;
            cornerThumb.SnapsToDevicePixels = true;
            cornerThumb.BorderThickness = new Thickness(1);
            cornerThumb.BorderBrush = new SolidColorBrush(Colors.Black);
            cornerThumb.Background = new SolidColorBrush(Colors.White);

            visualChildren.Add(cornerThumb);
        }


        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
    //======================================

    public class SurroundingSizeChangedEventArgs : RoutedEventArgs
    {
        public Size NewSize;
    }
    public delegate void SurroundingSizeChangedEventHandler(object sender, SurroundingSizeChangedEventArgs e);

    public class CanvasAdorner : Adorner
    {
        // Resizing adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        public SurroundingSizeChangedEventHandler surroundingSizeChanged;
        public Thumb bottomEdge, rightEdge, bottomRightCorner;
        Rect surrounding;
        bool surroundingVisible = false;
        Border borderElement;

        // To store and manage the adorner's visual children.
        VisualCollection visualChildren;


        //=================================================================================================
        //Ctors
        public CanvasAdorner(Canvas adornedElement, Border borderElement)
            : base(adornedElement)
        {
            this.borderElement = borderElement;
            surrounding = new Rect(0, 0, adornedElement.Width, adornedElement.Height);

            visualChildren = new VisualCollection(this);
            // Call a helper method to initialize the Thumbs
            // with a customized cursors.
            BuildAdornerThumb(ref bottomEdge, Cursors.SizeNS);
            BuildAdornerThumb(ref rightEdge, Cursors.SizeWE);
            BuildAdornerThumb(ref bottomRightCorner, Cursors.SizeNWSE);


            // Add handlers for resizing.
            bottomEdge.DragDelta += new DragDeltaEventHandler(HandlePreviewResize);
            bottomEdge.DragCompleted += new DragCompletedEventHandler(HandleResize);
            rightEdge.DragDelta += new DragDeltaEventHandler(HandlePreviewResize);
            rightEdge.DragCompleted += new DragCompletedEventHandler(HandleResize);
            bottomRightCorner.DragDelta += new DragDeltaEventHandler(HandlePreviewResize);
            bottomRightCorner.DragCompleted += new DragCompletedEventHandler(HandleResize);
        }

        //=================================================================================================
        // Handler for resizing.
        private void HandlePreviewResize(object sender, DragDeltaEventArgs args)
        {
            surroundingVisible = true;
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;
            if (adornedElement == null || hitThumb == null) return;

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            if (hitThumb != rightEdge)
                surrounding.Height = (int)Math.Max(surrounding.Height + args.VerticalChange, hitThumb.DesiredSize.Height);

            if (hitThumb != bottomEdge)
                surrounding.Width = (int)Math.Max(surrounding.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);

            if (surroundingSizeChanged != null)
                surroundingSizeChanged(this, new SurroundingSizeChangedEventArgs() { NewSize = surrounding.Size });
            this.InvalidateVisual();
        }
        private void HandleResize(object sender, DragCompletedEventArgs args)
        {
            surroundingVisible = false;
            FrameworkElement borderElement = this.borderElement as FrameworkElement;
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            //FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize.
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            if (hitThumb != rightEdge)
            {
                adornedElement.Height = surrounding.Height; //Math.Max(adornedElement.Height + args.VerticalChange, hitThumb.DesiredSize.Height);
                borderElement.Height = adornedElement.Height;
            }

            if (hitThumb != bottomEdge)
            {
                adornedElement.Width = surrounding.Width; //Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
                borderElement.Width = adornedElement.Width;
            }
        }


        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (bottomEdge != null)
                bottomEdge.Arrange(new Rect(surrounding.Width/2 - bottomEdge.Width / 2 , surrounding.Height, bottomEdge.Width, bottomEdge.Height));
            if (rightEdge != null)
                rightEdge.Arrange(new Rect(surrounding.Width, surrounding.Height / 2 - rightEdge.Height / 2, rightEdge.Width, rightEdge.Height));
            if (bottomRightCorner != null)
                bottomRightCorner.Arrange(new Rect(surrounding.Width, surrounding.Height, bottomRightCorner.Width, bottomRightCorner.Height));

            // Return the final size.
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        void BuildAdornerThumb(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 6;
            cornerThumb.SnapsToDevicePixels = true;
            cornerThumb.BorderThickness = new Thickness(1);
            cornerThumb.BorderBrush = new SolidColorBrush(Colors.Black);
            cornerThumb.Background = new SolidColorBrush(Colors.White);

            visualChildren.Add(cornerThumb);
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (surroundingVisible)
            {
                Pen BorderPen = new Pen(Brushes.Blue, 1);

                BorderPen.DashStyle = DashStyles.Dash;
                drawingContext.DrawRectangle(Brushes.Transparent, BorderPen, surrounding);
            }
        }
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

    }
}
