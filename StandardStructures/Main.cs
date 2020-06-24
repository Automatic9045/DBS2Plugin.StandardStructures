using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DepartureBoardSimulator;

namespace DbsPlugin.Standard.StandardStructures
{
    public class StandardStructures : DbsPluginBase
    {
        protected override DbsPluginConnector PluginConnector { get; }

        public StandardStructures(DbsPluginConnector pluginConnector) : base(pluginConnector)
        {
            PluginConnector = pluginConnector;
        }

        public override void Add(XElement element, string xmlPath)
        {
            switch (element.Name.LocalName)
            {
                case "Image":
                    AddImage(element, xmlPath);
                    break;
                case "Line":
                    AddLine(element);
                    break;
                case "Rectangle":
                    AddRectangle(element);
                    break;
                case "Text":
                    AddText(element);
                    break;
                default:
                    PluginConnector.ThrowError("コントロール \"" + element.Name.LocalName + "\" は定義されていません。", this.GetType().Name, "", "");
                    break;
            }
        }

        private void AddImage(XElement element, string xmlPath)
        {
            Image image = new Image()
            {
                HorizontalAlignment = (HorizontalAlignment)TypeDescriptor.GetConverter(typeof(HorizontalAlignment)).ConvertFromString((string)element.Attribute("Horizontal") ?? "Left"),
                VerticalAlignment = (VerticalAlignment)TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertFromString((string)element.Attribute("Vertical") ?? "Top"),
                Margin = new Thickness((double?)element.Attribute("X") ?? 0.0, (double?)element.Attribute("Y") ?? 0.0, 0, 0),

                Stretch = (Stretch)TypeDescriptor.GetConverter(typeof(Stretch)).ConvertFromString((string)element.Attribute("Mode") ?? "None"),
                Width = (double?)element.Attribute("Width") ?? 100.0,
                Height = (double?)element.Attribute("Height") ?? 100.0,
            };
            string sourcePath = System.IO.Path.Combine(xmlPath, (string)element.Attribute("Source") ?? "");
            string pluginName = this.GetType().Name;
            string type = element.Name.LocalName;
            string name = (string)element.Attribute("Name");
            if (sourcePath == xmlPath)
            {
                PluginConnector.ThrowError("画像のパスが指定されていません。", pluginName, type, name ?? "(名前無し)");
            }
            else if (!File.Exists(sourcePath))
            {
                PluginConnector.ThrowError("画像 \"" + sourcePath + "\" が見つかりませんでした。", pluginName, type, name ?? "(名前無し)");
            }
            else
            {
                ImageSource source = (ImageSource)new ImageSourceConverter().ConvertFromString(sourcePath);
                source.Freeze();
                image.Source = source;
            }

            ControlInfo controlInfo = new ControlInfo()
            {
                PluginName = this.GetType().Name,
                ControlType = this,
                ControlTypeName = type,
                ControlName = name,
            };
            PluginConnector.AddControl(controlInfo, image);
        }

        private void AddLine(XElement element)
        {
            Brush stroke = (Brush)new BrushConverter().ConvertFromString((string)element.Attribute("Color") ?? "Black");
            stroke.Freeze();
            Line line = new Line()
            {
                HorizontalAlignment = (HorizontalAlignment)TypeDescriptor.GetConverter(typeof(HorizontalAlignment)).ConvertFromString((string)element.Attribute("Horizontal") ?? "Left"),
                VerticalAlignment = (VerticalAlignment)TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertFromString((string)element.Attribute("Vertical") ?? "Top"),

                X1 = (double?)element.Attribute("X") ?? 0.0,
                Y1 = (double?)element.Attribute("Y") ?? 0.0,
                Stroke = stroke,
                StrokeThickness = (double?)element.Attribute("Thickness") ?? 1.0,
            };
            line.X2 = ((double?)element.Attribute("Width") ?? 0.0) + line.X1;
            line.Y2 = ((double?)element.Attribute("Height") ?? 0.0) + line.Y1;
            
            ControlInfo controlInfo = new ControlInfo()
            {
                PluginName = this.GetType().Name,
                ControlType = this,
                ControlTypeName = element.Name.LocalName,
                ControlName = (string)element.Attribute("Name"),
            };
            PluginConnector.AddControl(controlInfo, line);
        }

        private void AddRectangle(XElement element)
        {
            Brush stroke = (Brush)new BrushConverter().ConvertFromString((string)element.Attribute("Stroke") ?? "Black");
            stroke.Freeze();
            Brush fill = (Brush)new BrushConverter().ConvertFromString((string)element.Attribute("Stroke") ?? "Transparent");
            fill.Freeze();
            Rectangle rectangle = new Rectangle()
            {
                HorizontalAlignment = (HorizontalAlignment)TypeDescriptor.GetConverter(typeof(HorizontalAlignment)).ConvertFromString((string)element.Attribute("Horizontal") ?? "Left"),
                VerticalAlignment = (VerticalAlignment)TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertFromString((string)element.Attribute("Vertical") ?? "Top"),
                Margin = new Thickness((double?)element.Attribute("X") ?? 0.0, (double?)element.Attribute("Y") ?? 0.0, 0, 0),

                Width = (double?)element.Attribute("Width") ?? 100.0,
                Height = (double?)element.Attribute("Height") ?? 100.0,
                Stroke = stroke,
                StrokeThickness = (double?)element.Attribute("Thickness") ?? 1.0,
                Fill = fill,
            };

            ControlInfo controlInfo = new ControlInfo()
            {
                PluginName = this.GetType().Name,
                ControlType = this,
                ControlTypeName = element.Name.LocalName,
                ControlName = (string)element.Attribute("Name"),
            };
            PluginConnector.AddControl(controlInfo, rectangle);
        }

        private void AddText(XElement element)
        {
            Brush foreGround = (Brush)new BrushConverter().ConvertFromString((string)element.Attribute("FontColor") ?? "Black");
            foreGround.Freeze();
            TextBlock textBlock = new TextBlock()
            {
                HorizontalAlignment = (HorizontalAlignment)TypeDescriptor.GetConverter(typeof(HorizontalAlignment)).ConvertFromString((string)element.Attribute("Horizontal") ?? "Left"),
                VerticalAlignment = (VerticalAlignment)TypeDescriptor.GetConverter(typeof(VerticalAlignment)).ConvertFromString((string)element.Attribute("Vertical") ?? "Top"),
                Margin = new Thickness((double?)element.Attribute("X") ?? 0.0, (double?)element.Attribute("Y") ?? 0.0, 0, 0),

                FontFamily = new FontFamily((string)element.Attribute("FontFamily") ?? "MS Gothic"),
                FontSize = (double?)element.Attribute("FontSize") ?? 20.0,
                FontWeight = (FontWeight)new FontWeightConverter().ConvertFromString((string)element.Attribute("FontWeight") ?? "Regular"),
                FontStyle = (FontStyle)new FontStyleConverter().ConvertFromString((string)element.Attribute("FontStyle") ?? "Normal"),
                Foreground = foreGround,
                Text = (string)element.Attribute("Content") ?? "",
            };

            ControlInfo controlInfo = new ControlInfo()
            {
                PluginName = this.GetType().Name,
                ControlType = this,
                ControlTypeName = element.Name.LocalName,
                ControlName = (string)element.Attribute("Name"),
            };
            PluginConnector.AddControl(controlInfo, textBlock);
        }

        public override void Tick()
        {

        }

        public override void EditingUserControlShown()
        {

        }
    }
}
