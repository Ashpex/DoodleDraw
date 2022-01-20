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

namespace ProjectPaint.Adorners
{
        public class SelectAdorner : Adorner
        {
            // Resizing adorner uses Thumbs for visual elements.  
            // The Thumbs have built-in mouse input handling.
            public Thumb topEdge, bottomEdge, leftEdge, rightEdge;
            public Thumb topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner;
            public Thumb moveThumb;

            Rect surrounding;
            // To store and manage the adorner's visual children.
            VisualCollection visualChildren;

            //=================================================================================================
            // Initialize the ResizingAdorner.
            private void InitializeComponents(FrameworkElement adornedElement)
            {
                visualChildren = new VisualCollection(this);

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

            }

            //--------------------------------------------
            //Ctors
            public SelectAdorner(FrameworkElement adornedElement)
                : base(adornedElement)
            {
                surrounding = new Rect(Canvas.GetLeft(adornedElement), Canvas.GetTop(adornedElement), adornedElement.Width, adornedElement.Height);
                InitializeComponents(adornedElement);
            }

            //=================================================================================================
            // Handler for resizing.
            private void HandleResize(object sender, DragDeltaEventArgs args)
            {
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

            //=================================================================================================
            // Handler for moving.
            private void HandleMove(object sender, DragDeltaEventArgs args)
            {
                FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
                Thumb hitThumb = sender as Thumb;

                if (adornedElement == null || hitThumb == null) return;
                //FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

                double Left = Canvas.GetLeft(adornedElement);
                double Top = Canvas.GetTop(adornedElement);

                Canvas.SetTop(adornedElement, Top + args.VerticalChange);
                Canvas.SetLeft(adornedElement, Left + args.HorizontalChange);
            }

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
                FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
                Pen BorderPen = new Pen(Brushes.Blue, 1);

                BorderPen.DashStyle = DashStyles.Dash;
                drawingContext.DrawRectangle(Brushes.Transparent, BorderPen, new Rect(0, 0, adornedElement.ActualWidth, adornedElement.ActualHeight));
            }
            protected override int VisualChildrenCount { get { return visualChildren.Count; } }
            protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
        }
}
