

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Collections.Generic;


class Thing3D
{
    public Model3D model;
    public string name;

    public Thing3D(string new_name, Model3D new_model)
    {
        model = new Model3DGroup();
        ((Model3DGroup)model).Children.Add(new_model);
        //model = new_model;
        name = new_name;
    }

    public void input()
    {
    }


    public void update()
    {
    }


    public void render(Viewport3D vp)
    {
        ModelVisual3D mv3d = new ModelVisual3D();
        mv3d.Content = model;


        ((ModelVisual3D)(vp.Children[0])).Children.Add(mv3d);

        //vp.Models.Children.Add(model);
    }
}






partial class ClothDemo
{
    private Viewport3D vp;

    static Matrix3D view_matrix;
    static Matrix3D perspective_matrix;



    static List<Thing3D> things3d;


    static node_grid my_node_grid;
    static box my_box;

    static box my_box2;

    static sphere my_sphere;


    public void Create3D(Viewport3D vp)
    {
        this.vp = vp;

        vp.IsHitTestVisible = false;
        vp.ClipToBounds = false;


        RenderOptions.SetEdgeMode(vp, EdgeMode.Aliased);


        things3d = new List<Thing3D>();






        things3d.Add(new Thing3D("directional light", new DirectionalLight(Colors.White, new Vector3D(1.1, -1.1, -1.1))));
        things3d.Add(new Thing3D("directional light", new DirectionalLight(Colors.White, new Vector3D(-1.1, 0.1, 1.1))));
        things3d.Add(new Thing3D("directional light", new DirectionalLight(Colors.White, new Vector3D(1.1, 0.1, -1.1))));
        things3d.Add(new Thing3D("directional light", new DirectionalLight(Colors.White, new Vector3D(-1, 0, -1))));



        things3d.Add(new Thing3D("directional light", new AmbientLight(Color.FromRgb(55, 55, 55))));





        /*   [ m11      m12      m13      m14 ]
             [ m21      m22      m23      m24 ]
             [ m31      m32      m33      m34 ]
             [ offsetX  offsetY  offsetZ  m44 ] */




        MatrixCamera matrix_camera = new MatrixCamera();





        view_matrix = Matrix3D.Parse("0.539750657951516,0.559641738748834,-0.62886433472657,0,7.37257477290143E-18,0.747025071240996,0.66479586561394,0,0.841824938595261,-0.358824005868569,0.403207273708604,0,264.62356046239,-435.674414360455,-1350.56665818377,1");
        vp.Camera = new MatrixCamera(view_matrix, perspective_matrix);
        update_perspective_matrix();

        vp.SizeChanged += new SizeChangedEventHandler(vp_SizeChanged);


        my_node_grid = new node_grid(new Point3D(0, 1500, 0), 500, 500, 20, 20, 1, 5000000, 1500, 2500000, 1000, 3000000, 1000);
        // MINIMUM AXIAL VERTEX DENSITY ~= 500 / 20 = 25 units / node_count



        my_box = new box(new Point3D(0, 300, 1000), 200, 100, 50, .3, 0.01);




        my_box2 = new box(new Point3D(0, 300, -400), 150, 20, 100, .3, 0.01);



        //	my_sphere = new sphere(new Point3D(300, 200, 0), 100, 40, 20, 5.0, 0.01, mg);

        my_sphere = new sphere(new Point3D(000, 500, 02), 200, 40, 20, 10.0, .01, new DiffuseMaterial(MakeSphereGrid()));


        ModelVisual3D mv3d2 = new ModelVisual3D();
        mv3d2.Content = my_sphere.all_models;
        vp.Children.Add(mv3d2);

        //things3d.Add();







        things3d.Add(new Thing3D("box", my_box.model));
        things3d.Add(new Thing3D("box", my_box2.model));

        things3d.Add(new Thing3D("mesh", my_node_grid.model));




        int x = -750;

        things3d.Add(new Thing3D("floor", GenerateFloorMesh(MakeGrid(), x)));




        for (int n = 0; n < things3d.Count; n++)
        {
            ModelVisual3D mv3d = new ModelVisual3D();
            mv3d.Content = things3d[n].model;

            vp.Children.Add(mv3d);

        }
        CompositionTarget.Rendering += new EventHandler(Update);
    }

    static bool update_cloth = true;
    static bool a = false, b = false, c = false, d = false;

    static void Update(object sender, EventArgs e)
    {
        Vector3D sum_movement = new Vector3D();

        Vector3D forward_vec = new Vector3D(view_matrix.M13, view_matrix.M23, view_matrix.M33);
        forward_vec.Normalize();

        Vector3D right_vec = new Vector3D(view_matrix.M11, view_matrix.M21, view_matrix.M31);


        double linear_speed = 3000; // deltat was here...
         



        if (sum_movement.Length > 0)
        {
            sum_movement.Normalize();
            Matrix3D movement_mat = new TranslateTransform3D(sum_movement * linear_speed).Value;

            //Transform3D.CreateTranslation(sum_movement * linear_speed).Value; // LINEAR SPEED
            view_matrix = movement_mat * view_matrix;
        }
        foreach (Thing3D n in things3d)
        {
            n.input();
        }


        foreach (Thing3D n in things3d)
        {
            n.update();
        }



        my_node_grid.update_brush_contents();

        if (update_cloth)
        {


            for (int m = 0; m < my_node_grid.x_node_count; m++)
            {	// gravity
                for (int n = 0; n < my_node_grid.y_node_count; n++)
                {
                    my_node_grid.grid[m, n].total_force.Y += -200000;
                }
            }


            if (d)
            {
                my_node_grid.grid[19, 19].position = new Point3D(500, 600, 0);
                my_node_grid.grid[19, 19].velocity = new Vector3D(0, 0, 0);
                my_node_grid.grid[19, 19].total_force = new Vector3D(0, 0, 0);
            }

            if (c)
            {
                my_node_grid.grid[13, 19].position = new Point3D(333, 600, 0);
                my_node_grid.grid[13, 19].velocity = new Vector3D(0, 0, 0);
                my_node_grid.grid[13, 19].total_force = new Vector3D(0, 0, 0);
            }

            if (b)
            {
                my_node_grid.grid[6, 19].position = new Point3D(166, 600, 0);
                my_node_grid.grid[6, 19].velocity = new Vector3D(0, 0, 0);
                my_node_grid.grid[6, 19].total_force = new Vector3D(0, 0, 0);
            }


            if (a)
            {
                my_node_grid.grid[0, 19].position = new Point3D(0, 600, 0);
                my_node_grid.grid[0, 19].velocity = new Vector3D(0, 0, 0);
                my_node_grid.grid[0, 19].total_force = new Vector3D(0, 0, 0);
            }

            my_node_grid.update_physics_generate_next_positions();


            // COLLISION DETECTION

            my_node_grid.check_box_collision(my_box);
            my_node_grid.check_box_collision(my_box2);

            my_node_grid.check_sphere_collision(my_sphere);//  check_sphere_collision(sphere s)


            my_node_grid.commit_new_positions();

        }


    }


    static private ImageBrush MakeSphereGrid()
    {
        //DrawingGroup dg = new DrawingGroup();

        DrawingVisual dv = new DrawingVisual();

        using (DrawingContext ctx = dv.RenderOpen())
        {
            LinearGradientBrush lgb = new LinearGradientBrush();
            lgb.GradientStops.Add(new GradientStop(Colors.Black, 0));
            lgb.GradientStops.Add(new GradientStop(Colors.Purple, 1));
            lgb.StartPoint = new Point(0, 0);
            lgb.EndPoint = new Point(0, 1);
            lgb.Freeze();

            ctx.DrawRectangle(lgb, null, new Rect(0, 0, 1024, 1024));
            Pen whitePen = new Pen();
            whitePen.Brush = Brushes.Gray;
            whitePen.Thickness = 4;
            whitePen.Freeze();
            // Horizontal Lines


            for (int n = 0; n < 21; n++)
            {
                ctx.DrawLine(whitePen, new Point(0, (1 / 20.0) * 1024 * n), new Point(1024, (1 / 20.0) * 1024 * n));
            }

            // Vertical Lines
            for (int n = 0; n < 20; n++)
            {	// 21 -> 20
                ctx.DrawLine(whitePen, new Point((1 / 20.0) * 1024 * n, 0), new Point((1 / 20.0) * 1024 * n, 1024));
            }

        }

        RenderTargetBitmap rtb = new RenderTargetBitmap(1024, 1024, 96, 96, PixelFormats.Pbgra32);

        rtb.Render(dv);

        ImageBrush ib = new ImageBrush(rtb);

        return ib;




    }

    private DrawingBrush MakeVideoDrawingBrush()
    {

        MediaTimeline mt = new MediaTimeline(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, "C:\\2134.mpeg"), UriKind.Absolute));
        //MediaClock mc = mt.CreateClock();
        DrawingGroup dg = new DrawingGroup();

        MediaPlayer mp = new MediaPlayer();

        using (DrawingContext ctx = dg.Open())
        {
            ctx.DrawVideo(mp, new Rect(0, 0, 30, 30));
        }

        dg.Freeze();

        return new DrawingBrush(dg);
    }


  private static void create_grid_of_doubles(double[,] grid, int edge, double height, double x_offset, double y_offset)
  {

	 for (int n = 0; n < edge; n++) {
		for (int m = 0; m < edge; m++) {
		  grid[n, m] = height * System.Math.Sin((n + x_offset) * 2 * System.Math.PI / (double)edge) * System.Math.Sin((m + y_offset) * 2 * System.Math.PI / (double)edge);
		}
	 }

  }




  private static void set_mesh_positions(int edge, double height, MeshGeometry3D mesh, double[,] grid, int x_offset, int y_offset)
  {
	 mesh.Positions.Clear();

	 for (int n = 0; n < edge; n++) { // n -> x
		for (int m = 0; m < edge; m++) { // m -> y

		  mesh.Positions.Add(new Point3D(((n / (double)edge) - 0.5) * 500.0, ((m / (double)edge) - 0.5) * 500.0,
			 grid[(n + x_offset) % edge, (m + y_offset) % edge] - (height / 2.0)));

		}
	 }
  }



  private static void set_mesh_texture_coordinates(int edge, MeshGeometry3D mesh)
  {
	 mesh.TextureCoordinates.Clear();

	 for (int n = 0; n < edge; n++) { // n -> x
		for (int m = 0; m < edge; m++) { // m -> y

		  mesh.TextureCoordinates.Add(new Point(n / (double)edge, m / (double)edge));
		}
	 }
  }




  private static void set_mesh_vertex_indices(int edge, MeshGeometry3D mesh)
  {
	 for (int n = 0; n < edge - 1; n++) {
		for (int m = 0; m < edge - 1; m++) {  // front and back faces of this quad

		  mesh.TriangleIndices.Add(n + (m * edge)); // front face
		  mesh.TriangleIndices.Add(n + ((m + 1) * edge));
		  mesh.TriangleIndices.Add((n + 1) + (m * edge));

		  mesh.TriangleIndices.Add((n + 1) + (m * edge)); // back face
		  mesh.TriangleIndices.Add(n + ((m + 1) * edge));
		  mesh.TriangleIndices.Add(n + (m * edge));


		  mesh.TriangleIndices.Add(n + ((m + 1) * edge)); // front face
		  mesh.TriangleIndices.Add((n + 1) + ((m + 1) * edge));
		  mesh.TriangleIndices.Add((n + 1) + (m * edge));

		  mesh.TriangleIndices.Add((n + 1) + (m * edge)); // back face
		  mesh.TriangleIndices.Add((n + 1) + ((m + 1) * edge));
		  mesh.TriangleIndices.Add(n + ((m + 1) * edge));

		}
	 }
  }



  private const int edge = 20;
  private static double height = 200;

  private static double[,] grid;

  private static MeshGeometry3D mesh;




  private static GeometryModel3D generate_video_mesh(Brush brush)
  {
	 grid = new double[edge, edge];
	 mesh = new MeshGeometry3D();

	 create_grid_of_doubles(grid, edge, height, 0, 0);

	 set_mesh_positions(edge, height, mesh, grid, 0, 0);

	 set_mesh_texture_coordinates(edge, mesh);


	 set_mesh_vertex_indices(edge, mesh);





	 GeometryModel3D model = new GeometryModel3D(mesh, new DiffuseMaterial((brush)));

	 return model;
  }









  static private ImageBrush MakeGrid()
  {
	// DrawingGroup dg = new DrawingGroup();

	 DrawingVisual dv = new DrawingVisual();

	 using (DrawingContext ctx = dv.RenderOpen() ) {
		LinearGradientBrush lgb = new LinearGradientBrush();
		lgb.GradientStops.Add(new GradientStop(Colors.DarkBlue, 0));
		lgb.GradientStops.Add(new GradientStop(Colors.Black, 1));
		lgb.StartPoint = new Point(0, 0);
		lgb.EndPoint = new Point(1, 1);
		lgb.Freeze();

		ctx.DrawRectangle(lgb, null, new Rect(0, 0, 1024, 1024));
		Pen whitePen = new Pen();
		whitePen.Brush = Brushes.Gray;
		whitePen.Thickness = 4;
		whitePen.Freeze();
		// Horizontal Lines


		for (int n = 0; n < 21; n++) {
			ctx.DrawLine(whitePen, new Point(0, (1 / 20.0) * 1024 * n), new Point(1024, (1 / 20.0) * 1024 * n));
		}

		// Vertical Lines
		for (int n = 0; n < 21; n++) {
			ctx.DrawLine(whitePen, new Point((1 / 20.0) * 1024 * n, 0), new Point((1 / 20.0) * 1024 * n, 1024));
		}



	 }


	 RenderTargetBitmap rtb = new RenderTargetBitmap(1024, 1024, 96, 96, PixelFormats.Pbgra32);

	 rtb.Render(dv);

	 ImageBrush ib = new ImageBrush(rtb);

	 return ib;

  }

  private static GeometryModel3D GenerateFloorMesh(Brush brush, double height)
  {

	 const int edge = 2;

	 MeshGeometry3D mesh = new MeshGeometry3D();

	 for (int n = 0; n < edge; n++) { // n -> x
		for (int m = 0; m < edge; m++) { // m -> y
		  mesh.Positions.Add(new Point3D(((n / ((double)edge - 1)) - 0.5) * 1000.0, height, ((m / ((double)edge - 1)) - 0.5) * 1000.0));


		  mesh.TextureCoordinates.Add(new Point(n / (double)edge, m / (double)edge));
		}
	 }


	 for (int n = 0; n < edge - 1; n++) {
		for (int m = 0; m < edge - 1; m++) {
		  mesh.TriangleIndices.Add(n + (m * edge));
		  mesh.TriangleIndices.Add(n + ((m + 1) * edge));
		  mesh.TriangleIndices.Add((n + 1) + (m * edge));

		  mesh.TriangleIndices.Add((n + 1) + (m * edge));
		  mesh.TriangleIndices.Add(n + ((m + 1) * edge));
		  mesh.TriangleIndices.Add(n + (m * edge));


		  mesh.TriangleIndices.Add(n + ((m + 1) * edge));
		  mesh.TriangleIndices.Add((n + 1) + ((m + 1) * edge));
		  mesh.TriangleIndices.Add((n + 1) + (m * edge));

		  mesh.TriangleIndices.Add((n + 1) + (m * edge));
		  mesh.TriangleIndices.Add((n + 1) + ((m + 1) * edge));
		  mesh.TriangleIndices.Add(n + ((m + 1) * edge));

		}
	 }

	 GeometryModel3D model = new GeometryModel3D(mesh, new EmissiveMaterial((brush)));
	 return model;
  }

  static double fov_degrees = 60;
  static double near_clip_plane = 10.0;
  static double far_clip_plane = 50000;


  private void update_perspective_matrix()
  {


      double fov_rad = (fov_degrees / 180.0) * Math.PI;

      double aspect_ratio = (double)vp.ActualWidth / (double)vp.ActualHeight;

      double denom = Math.Tan(fov_rad / 2.0);

      double m11 = 1.0 / denom;
      double m22 = aspect_ratio / denom;
      double m33 = far_clip_plane / (near_clip_plane - far_clip_plane);
      double m43 = near_clip_plane * far_clip_plane / (near_clip_plane - far_clip_plane);


      perspective_matrix = new Matrix3D(m11, 0, 0, 0,
                                                   0, m22, 0, 0,
                                                   0, 0, m33, -1,
                                                   0, 0, m43, 0);

      ((MatrixCamera)(vp.Camera)).ProjectionMatrix = perspective_matrix;

  }










  void vp_SizeChanged(object sender, SizeChangedEventArgs e)
  {

	 Console.WriteLine("vp_SizeChanged " + vp.ActualWidth.ToString() + " x " + vp.ActualHeight.ToString());
	 update_perspective_matrix();


  }


  void w_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {


//	 vp.Dispatcher.BeginInvoke(update_priority, new update_delegate(Update));
  }









  static void w_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
  {

  }





}
