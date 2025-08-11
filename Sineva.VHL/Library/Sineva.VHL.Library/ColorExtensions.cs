/****************************************************************
 * Copyright	: www.sineva.com.cn
 * Version		: V3.0
 * Programmer	: Software Team
 * Issue Date	: 23.01.13
 * Description	: 
 * 
 ****************************************************************/
using System.Windows;
using System.Windows.Media;

namespace Sineva.VHL.Library
{
    public class ColorExtensions
    {
        public static readonly DependencyProperty ColorFrontProperty = DependencyProperty.RegisterAttached(
    "ColorFront",
    typeof(Color),
    typeof(ColorExtensions),
    new UIPropertyMetadata(Colors.White));

        public static Color GetColorFront(DependencyObject target)
        {
            return (Color)target.GetValue(ColorFrontProperty);
        }

        public static void SetColorFront(DependencyObject target, Color value)
        {
            target.SetValue(ColorFrontProperty, value);
        }

        public static readonly DependencyProperty ColorMidProperty = DependencyProperty.RegisterAttached(
            "ColorMid",
            typeof(Color),
            typeof(ColorExtensions),
            new UIPropertyMetadata(Colors.Black));

        public static Color GetColorMid(DependencyObject target)
        {
            return (Color)target.GetValue(ColorMidProperty);
        }

        public static void SetColorMid(DependencyObject target, Color value)
        {
            target.SetValue(ColorMidProperty, value);
        }

        public static readonly DependencyProperty ColorBackProperty = DependencyProperty.RegisterAttached(
            "ColorBack",
            typeof(Color),
            typeof(ColorExtensions),
            new UIPropertyMetadata(Colors.Black));

        public static Color GetColorBack(DependencyObject target)
        {
            return (Color)target.GetValue(ColorBackProperty);
        }

        public static void SetColorBack(DependencyObject target, Color value)
        {
            target.SetValue(ColorBackProperty, value);
        }
    }
}
