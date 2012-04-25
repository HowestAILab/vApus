///*
// * Copyright 2012 (c) Sizing Servers Lab
// * University College of West-Flanders, Department GKG
// * 
// * Author(s):
// *    Dieter Vandroemme
// */
//using System;
//using System.Runtime.InteropServices;
//using SlimDX;
//using SlimDX.Direct3D9;

//namespace vApus.Util
//{
//    public abstract class D3D9BaseChart : BaseChart
//    {
//        protected Direct3D _d3d;
//        protected Device _device;

//        private const int GRIDLINECOUNT = 6;
//        protected int _lineVertexSize = Marshal.SizeOf(typeof(LineVertex));

//        //Basically a list of begin and end points
//        private LineVertex[] _gridLinesVertexes = new LineVertex[GRIDLINECOUNT * 2];


//        public D3D9BaseChart()
//        {
//            this.Disposed += new EventHandler(D3D9BaseChart_Disposed);
//            if (this.IsHandleCreated)
//                Init();
//            else
//                this.HandleCreated += new EventHandler(D3D9BaseChart_HandleCreated);
//        }

//        private void D3D9BaseChart_HandleCreated(object sender, EventArgs e)
//        {
//            this.HandleCreated -= D3D9BaseChart_HandleCreated;
//            Init();
//        }

//        private void Init()
//        {
//            _d3d = new Direct3D();
//            _device = new Device(_d3d, 0, DeviceType.Hardware, Handle,
//                                    CreateFlags.HardwareVertexProcessing, PresentParameters);
//            InitGridLines();
//        }
//        protected PresentParameters PresentParameters
//        {
//            get
//            {
//                var pp = new PresentParameters();
//                pp.SwapEffect = SwapEffect.Discard;
//                pp.DeviceWindowHandle = Handle;
//                pp.Windowed = true;
//                pp.BackBufferWidth = Width;
//                pp.BackBufferHeight = Height;
//                pp.BackBufferFormat = Format.X8R8G8B8;
//                return pp;
//            }
//        }
//        private void InitGridLines()
//        {
//            ////Fill the line vertexes array
//            float step = 2.0f / (GRIDLINECOUNT - 1);
//            int i = 0;
//            for (float y = -1; y <= 1.0f; y += step)
//            {
//                //Begin points
//                _gridLinesVertexes[i++] = new LineVertex()
//                {
//                    Position = new Vector3(-1, y, 0),
//                    Color = System.Drawing.Color.LightGray.ToArgb()
//                };
//                //End points
//                _gridLinesVertexes[i++] = new LineVertex()
//                {
//                    Position = new Vector3(1, y, 0),
//                    Color = System.Drawing.Color.LightGray.ToArgb()
//                };
//            }
//        }

//        private void D3D9BaseChart_Disposed(object sender, EventArgs e)
//        {
//            CleanUp();
//        }
//        private void CleanUp()
//        {
//            if (_device != null && !_device.Disposed)
//                try { _device.Dispose(); }
//                catch { }
//            _device = null;

//            if (_d3d != null && !_d3d.Disposed)
//                try { _d3d.Dispose(); }
//                catch { }
//            _d3d = null;
//        }

//        protected override void DrawGridLines()
//        {
//            //These are already known, theys just need to be shown.
//            _device.DrawUserPrimitives(PrimitiveType.LineList, 0, GRIDLINECOUNT, _gridLinesVertexes);
//        }
//    }

//    public struct LineVertex
//    {
//        public Vector3 Position { get; set; }
//        public int Color { get; set; }
//    }
//}
