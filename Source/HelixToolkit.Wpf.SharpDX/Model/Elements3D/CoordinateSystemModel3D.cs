﻿// <copyright file="CoordinateSystemModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>

using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;
using System.Windows;
using Media = System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX
{
    public class CoordinateSystemModel3D : ScreenSpacedElement3D
    {
        /// <summary>
        /// <see cref="AxisXColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisXColorProperty = DependencyProperty.Register("AxisXColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.Red,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(0, ((Media.Color)e.NewValue).ToColor4());
                }));
        /// <summary>
        /// <see cref="AxisYColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisYColorProperty = DependencyProperty.Register("AxisYColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.Green,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(1, ((Media.Color)e.NewValue).ToColor4());
                }));
        /// <summary>
        /// <see cref="AxisZColor"/>
        /// </summary>
        public static readonly DependencyProperty AxisZColorProperty = DependencyProperty.Register("AxisZColor", typeof(Media.Color), typeof(CoordinateSystemModel3D),
            new AffectsRenderPropertyMetadata(Media.Colors.Blue,
                (d, e) =>
                {
                    (d as CoordinateSystemModel3D).UpdateAxisColor(2, ((Media.Color)e.NewValue).ToColor4());
                }));

        /// <summary>
        /// Axis X Color
        /// </summary>
        public Media.Color AxisXColor
        {
            set
            {
                SetValue(AxisXColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisXColorProperty);
            }
        }
        /// <summary>
        /// Axis Y Color
        /// </summary>
        public Media.Color AxisYColor
        {
            set
            {
                SetValue(AxisYColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisYColorProperty);
            }
        }
        /// <summary>
        /// Axis Z Color
        /// </summary>
        public Media.Color AxisZColor
        {
            set
            {
                SetValue(AxisZColorProperty, value);
            }
            get
            {
                return (Media.Color)GetValue(AxisZColorProperty);
            }
        }

        private readonly BillboardTextModel3D[] axisBillboards = new BillboardTextModel3D[3];
        private readonly MeshGeometryModel3D arrowMeshModel = new MeshGeometryModel3D();

        public CoordinateSystemModel3D()
        {
            var builder = new MeshBuilder(true, false, false);
            builder.AddArrow(Vector3.Zero, new Vector3(5, 0, 0), 0.5, 1.2, 10);
            builder.AddArrow(Vector3.Zero, new Vector3(0, 5, 0), 0.5, 1.2, 10);
            builder.AddArrow(Vector3.Zero, new Vector3(0, 0, 5), 0.5, 1.2, 10);
            var mesh = builder.ToMesh();
            arrowMeshModel.Material = PhongMaterials.White;
            arrowMeshModel.Geometry = mesh;
            arrowMeshModel.CullMode = CullMode.Back;
            arrowMeshModel.OnSetRenderTechnique += (host) => { return host.EffectsManager[DefaultRenderTechniqueNames.Colors]; };
            arrowMeshModel.IsHitTestVisible = false;

            axisBillboards[0] = new BillboardTextModel3D() { IsHitTestVisible = false };
            axisBillboards[1] = new BillboardTextModel3D() { IsHitTestVisible = false };
            axisBillboards[2] = new BillboardTextModel3D() { IsHitTestVisible = false };
            UpdateAxisColor(mesh, 0, AxisXColor.ToColor4());
            UpdateAxisColor(mesh, 1, AxisYColor.ToColor4());
            UpdateAxisColor(mesh, 2, AxisZColor.ToColor4());

            Children.Add(arrowMeshModel);
            Children.Add(axisBillboards[0]);
            Children.Add(axisBillboards[1]);
            Children.Add(axisBillboards[2]);
        }

        private void UpdateAxisColor(int which, Color4 color)
        {
            UpdateAxisColor(arrowMeshModel.Geometry, which, color);
        }

        private void UpdateAxisColor(Geometry3D mesh, int which, Color4 color)
        {
            switch (which)
            {
                case 0:
                    axisBillboards[which].Geometry = new BillboardSingleText3D()
                    { TextInfo = new TextInfo("X", new Vector3(7, 0, 0)), BackgroundColor = Color.Transparent, FontSize = 12, FontColor = color };
                    break;
                case 1:
                    axisBillboards[which].Geometry = new BillboardSingleText3D()
                    { TextInfo = new TextInfo("Y", new Vector3(0, 7, 0)), BackgroundColor = Color.Transparent, FontSize = 12, FontColor = color };
                    break;
                case 2:
                    axisBillboards[which].Geometry = new BillboardSingleText3D()
                    { TextInfo = new TextInfo("Z", new Vector3(0, 0, 7)), BackgroundColor = Color.Transparent, FontSize = 12, FontColor = color };
                    break;
            }
            int segment = mesh.Positions.Count / 3;
            var colors = new Core.Color4Collection(mesh.Colors == null ? Enumerable.Repeat<Color4>(Color.Black, mesh.Positions.Count) : mesh.Colors);
            for (int i = segment * which; i < segment * (which + 1); ++i)
            {
                colors[i] = color;
            }
            mesh.Colors = colors;
        }

        protected override bool CanHitTest()
        {
            return false;
        }
    }
}
