using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using AxJasAxControl;
using JasAxControl;
using NPCommonHelp.MVVM;
using NPCommonHelp.UI;

namespace NPTool.NPCamera
{
    /// <summary>
    ///     定位到经纬度工具
    ///     支持 DD|DDH DMS|DMSH 两种模式
    /// </summary>
    public class LocateToLatLangControler : BindableBase, IDataErrorInfo
    {
        /// <summary>
        ///     double 格式 DD|DDH
        /// </summary>
        private static readonly Regex DDModeRex = new Regex(
                @"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?)\s*,\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)(\s*,\s*\d+(\.\d+)?)?$")
            ;

        /// <summary>
        ///     度分秒格式 DMS|DMSH
        /// </summary>
        private static readonly Regex DMSModeRex =
                new Regex(
                    "^[1-8]?\\d(°|\\s+)[1-5]?\\d(\'|\\s+)[1-5]?\\d(\\.\\d+)?(\"|\\s+)(N|S)\\s+((1[0-7]\\d)|([1-9]?\\d))(°|\\s+)[1-5]?\\d(\'|\\s+)[1-5]?\\d(\\.\\d+)?(\"|\\s+)(W|E)\\s*((,|\\s+)\\d+(\\.\\d+)?M)?$")
            ;

        private readonly BackgroundWorker _bgWorker;
        private readonly double _flyHeight;

        private readonly JasColour _foreground = new JasColour {Blue = 1f, Green = 1f, Red = 1f, Alpha = 1f};
        private readonly JasColour _frameColor = new JasColour {Blue = 1f, Green = 1f, Red = 1f, Alpha = 1f};
        private readonly AxJasSceneControl _sceneControl;
        private double _altitude;
        private double _latitude;
        private double _longitude;
        private string _sceneLabelName;
        private string _searctString = string.Empty;

        /// <summary>
        ///     定位到经纬度工具
        /// </summary>
        /// <param name="control">三维控件</param>
        /// <param name="flyheight">飞行高度</param>
        public LocateToLatLangControler(AxJasSceneControl control, double flyheight = 500)
        {
            _sceneControl = control;
            _flyHeight = flyheight;

            _bgWorker = new BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.DoWork += DoWork_Handler;
            _bgWorker.ProgressChanged += ProgressChanged_Handler;
        }

        /// <summary>
        ///     定位命令
        /// </summary>
        public ICommand LocationCommand => new DelegateCommand<string>(LocationTo);

        /// <summary>
        ///     搜索字符串
        /// </summary>
        public string SearctString
        {
            get { return _searctString; }

            set
            {
                SetProperty(ref _searctString, value);
                if (_sceneLabelName != null)
                {
                    RemoveLabel();
                }
            }
        }

        /// <summary>
        ///     经度
        /// </summary>
        public double Longitude
        {
            get { return _longitude; }

            set { SetProperty(ref _longitude, value); }
        }

        /// <summary>
        ///     纬度
        /// </summary>
        public double Latitude
        {
            get { return _latitude; }

            set { SetProperty(ref _latitude, value); }
        }

        /// <summary>
        ///     高度
        /// </summary>
        public double Altitude
        {
            get { return _altitude; }

            set { SetProperty(ref _altitude, value); }
        }

        /// <summary>
        ///     IDataErrorInfo
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                if (columnName == "SearctString" && !string.IsNullOrEmpty(SearctString) &&
                    !DMSModeRex.IsMatch(SearctString) && !DDModeRex.IsMatch(SearctString))
                {
                    return
                        "示例格式:\r\n 31.403, 137.174\r\n 31.403, 137.174\r\n 41°24'12.2\"N 2°10'26.5\"E\r\n 41°24'12.2\"N 2°10'26.5\"E, 500M";
                }

                return null;
            }
        }

        /// <summary>
        ///     IDataErrorInfo
        /// </summary>
        [Description("Test-Property")]
        public string Error => string.Empty;

        private void LocationTo(string obj)
        {
            try
            {
                if (DDModeRex.IsMatch(SearctString))
                {
                    var langlat = SearctString.Split(',');
                    Latitude = double.Parse(langlat[0]);
                    Longitude = double.Parse(langlat[1]);
                    if (langlat.Length == 3)
                        Altitude = double.Parse(langlat[2]);
                    else
                        Altitude = double.NaN;

                    FlayTo();
                }
                if (DMSModeRex.IsMatch(SearctString))
                {
                    if (Match())
                    {
                        FlayTo();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool Match()
        {
            var find = new Regex("((\\d+\\.)?\\d+)[°'\"M\\s+]");

            var m = find.Match(SearctString);
            var result = new List<double>();
            while (m.Success)
            {
                for (var i = 1; i <= 1; i++)
                {
                    var g = m.Groups[i];
                    Debug.WriteLine("Group" + i + "='" + g + "'");
                    var cc = g.Captures;
                    for (var j = 0; j < cc.Count; j++)
                    {
                        var c = cc[j];
                        Debug.WriteLine("Capture" + j + "='" + c + "', Position=" + c.Index);

                        double convert;
                        if (double.TryParse(c.ToString(), out convert))
                        {
                            result.Add(convert);
                        }
                    }
                }
                m = m.NextMatch();
            }

            if (result.Count == 6)
            {
                Latitude = result[0] + (result[1] + result[2] / 60) / 60;
                Longitude = result[3] + (result[4] + result[5] / 60) / 60;
                Latitude = SearctString.Contains("N") ? Latitude : -Latitude;
                Longitude = SearctString.Contains("E") ? _longitude : -Longitude;
                Altitude = double.NaN;
                return true;
            }

            if (result.Count == 7)
            {
                Latitude = result[0] + (result[1] + result[2] / 60) / 60;
                Longitude = result[3] + (result[4] + result[5] / 60) / 60;
                Latitude = SearctString.Contains("N") ? Latitude : -Latitude;
                Longitude = SearctString.Contains("E") ? _longitude : -Longitude;
                Altitude = result[6];
                return true;
            }

            return false;
        }

        private void FlayTo()
        {
            var hei = _flyHeight;
            if (!double.IsNaN(Altitude))
                hei = Altitude;
            _sceneControl?.SceneView.FlyTo(Longitude, Latitude, 0, 0, hei, 300);

            if (!_bgWorker.IsBusy)
            {
                _bgWorker.RunWorkerAsync();
            }
        }

        private void ProgressChanged_Handler(object sender, ProgressChangedEventArgs e)
        {
            if (_sceneControl == null) return;
            var center = _sceneControl?.SceneView.Screen2Geo(_sceneControl.Width / 2, _sceneControl.Height / 2, false);
            var lonlatalt = _sceneControl?.SceneView.Screen2Geo(_sceneControl.Width / 2, _sceneControl.Height, false);

            double hei = 0;
            if (!double.IsNaN(Altitude))
                hei = Altitude;

            var carCenter = _sceneControl.MathUtility.Spherical2Cartesian(center.X, center.Y, hei);
            var carBotemcenter = _sceneControl.MathUtility.Spherical2Cartesian(lonlatalt.X, lonlatalt.Y, hei);

            var distance = carCenter.Minus(carBotemcenter).Length;
            var height = distance * Math.Tan((double) 30 / 180 * Math.PI);

            if (double.IsNaN(Altitude))
            {
                center = _sceneControl?.SceneView.Screen2Geo(_sceneControl.Width / 2, _sceneControl.Height / 2, true);
                Altitude = center.Z;
            }

            _sceneControl?.SceneView.FlyTo(lonlatalt.X, lonlatalt.Y, 0, 60, Altitude + height, 400);


            CreateLabel();
        }

        private void DoWork_Handler(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            Thread.Sleep(500);
            worker?.ReportProgress(1);
        }

        private void CreateLabel()
        {
            RemoveLabel();

            var lonlat = new JasVector3 {X = Longitude, Y = Latitude, Z = Altitude};
            var offset = new JasVector3 {X = 10, Y = 10, Z = 0};
            var text = "纬度: " + StringFormat.ConvertDigitalToDegrees(lonlat.Y) +
                       (lonlat.Y < 0 ? " S\n" : " N\n") +
                       "经度: " + StringFormat.ConvertDigitalToDegrees(lonlat.X) +
                       (lonlat.X < 0 ? " W\n" : " E\n") + 
                       "高程: " + lonlat.Z.ToString(".00") + " M";

            CreateLabel(_sceneControl, lonlat, offset, text, _foreground, _frameColor);
        }

        private void RemoveLabel()
        {
            if (_sceneLabelName != null)
            {
                var text = _sceneControl.SceneManager.GetSceneLabel(_sceneLabelName);
                if (text != null)
                {
                    _sceneControl.SceneManager.DestorySceneLabel(text.GetName());
                    Marshal.ReleaseComObject(text);
                }
                _sceneLabelName = null;
            }
        }

        private void CreateLabel(AxJasSceneControl control, JasVector3 lonlat, JasVector3 offset, string text,
            JasColour textColour, JasColour frameColour)
        {
            var pos = control.MathUtility.Spherical2Cartesian(lonlat.X, lonlat.Y, lonlat.Z);
            var rootNode = control.SceneManager.CreateSceneNode(jasSceneMemoryType.SMT_Dynamic);
            var quat = new JasQuaternion();
            quat.Set(1, 0, 0, 0);
            var sceneNode = rootNode.CreateChild(jasSceneMemoryType.SMT_Dynamic, pos, quat);

            if (_sceneLabelName == null)
            {
                //SceneLabel
                _sceneLabelName = DateTime.Now.Ticks.ToString();
                var sceneLabel = control.SceneManager.CreateSceneLabel(_sceneLabelName);
                sceneLabel.AttachToNode(sceneNode, control.SceneView);
                sceneLabel.AppendText(text, "微软雅黑", 12, textColour);

                //背景图片
                sceneLabel.CreateLabelContext("BlackWall.png");
                //边框
                sceneLabel.CreateLabelFrame(frameColour);
                //创建引线
                sceneLabel.CreateTextLine(frameColour, offset);
                sceneLabel.SetLineType(0, jasLineType.JLT_TRIANGLE);

                sceneLabel.SetSelectable(false);
                sceneLabel.SetDepthCheckEnabled(false);
            }
        }

        /// <summary>
        ///     清除所有占用资源
        /// </summary>
        public void Clear()
        {
            RemoveLabel();
        }
    }
}