//-----------------------------------------------------------------------
// <copyright file="GridViewRowPresenterWithGridLines.cs" company="Lifeprojects.de">
//     Class: GridViewRowPresenterWithGridLines
//     Copyright © Lifeprojects.de 2019
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>14.05.2019</date>
//
// <summary>
//      UI Control Class with GridViewRowPresenterWithGridLines for ListView-Control
// </summary>
//-----------------------------------------------------------------------

namespace PasswortNET.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class GridViewRowPresenterWithGridLines : GridViewRowPresenter
    {
        public static readonly DependencyProperty SeparatorStyleProperty;
        private static readonly Style DefaultSeparatorStyle;
        private readonly List<FrameworkElement> lines = new List<FrameworkElement>();

        static GridViewRowPresenterWithGridLines()
        {
            DefaultSeparatorStyle = new Style(typeof(Rectangle));
            DefaultSeparatorStyle.Setters.Add(new Setter(Shape.FillProperty, SystemColors.ControlLightBrush));
            SeparatorStyleProperty = DependencyProperty.Register("SeparatorStyle", typeof(Style), typeof(GridViewRowPresenterWithGridLines),
                                                                    new UIPropertyMetadata(DefaultSeparatorStyle, SeparatorStyleChanged));
        }

        public Style SeparatorStyle
        {
            get { return (Style)GetValue(SeparatorStyleProperty); }
            set { this.SetValue(SeparatorStyleProperty, value); }
        }

        protected override int VisualChildrenCount
        {
            get { return base.VisualChildrenCount + this.lines.Count; }
        }

        private IEnumerable<FrameworkElement> Children
        {
            get { return LogicalTreeHelper.GetChildren(this).OfType<FrameworkElement>(); }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var size = base.ArrangeOverride(arrangeSize);
            var children = this.Children.ToList();
            this.EnsureLines(children.Count);
            for (var i = 0; i < this.lines.Count; i++)
            {
                var child = children[i];
                var x = child.TransformToAncestor(this).Transform(new Point(child.ActualWidth, 0)).X + child.Margin.Right;
                var rect = new Rect(x, -Margin.Top, 1, size.Height + Margin.Top + Margin.Bottom);
                var line = this.lines[i];
                line.Measure(rect.Size);
                line.Arrange(rect);
            }

            return size;
        }

        protected override Visual GetVisualChild(int index)
        {
            var count = base.VisualChildrenCount;
            return index < count ? base.GetVisualChild(index) : this.lines[index - count];
        }

        private static void SeparatorStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var presenter = (GridViewRowPresenterWithGridLines)d;
            var style = (Style)e.NewValue;
            foreach (FrameworkElement line in presenter.lines)
            {
                line.Style = style;
            }
        }

        private void EnsureLines(int count)
        {
            count = count - this.lines.Count;
            for (var i = 0; i < count; i++)
            {
                var line = (FrameworkElement)Activator.CreateInstance(this.SeparatorStyle.TargetType);
                line = new Rectangle { Fill = Brushes.LightGray };
                line.Style = this.SeparatorStyle;
                this.AddVisualChild(line);
                this.lines.Add(line);
            }
        }
    }
}