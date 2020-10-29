using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

// F = m a = -kx -bv

class node
{
	public Point3D position;
	public Point3D next_position;

	public Vector3D velocity;
	public Vector3D total_force;

	public node()
	{
		position = new Point3D();
		velocity = new Vector3D();
		total_force = new Vector3D();
	}



	public void determine_internode_force(node other_node, double rest_length, double elasticity_coefficient, double damping_coefficient)
	{

		// F = m a = -kx -bv

		Vector3D vector_to_node = other_node.position - position;
		Vector3D direction_to_node = vector_to_node / vector_to_node.Length;

		Vector3D elastic_force = -(elasticity_coefficient * (rest_length - vector_to_node.Length) * direction_to_node); // may need negation
		Vector3D damping_force = (damping_coefficient * (other_node.velocity - velocity)); // this line may need negation
		
		Vector3D force = (elastic_force + damping_force) / rest_length;

		total_force += force;
		other_node.total_force -= force;
	}




	public void update_velocity_and_position(double mass, double delta_t)	// this is done separately from acceleration because it'll affect parameters used by other nodes
	{
		// f = m * a
		Vector3D acceleration = total_force / mass;

		velocity += acceleration * delta_t;

		next_position = position + velocity * delta_t + (.5 * acceleration * delta_t * delta_t);

		total_force.X = 0;
		total_force.Y = 0;
		total_force.Z = 0;
	}
}





class node_grid
{
	public node[,] grid;

//	private int x_node_count;
//	private int y_node_count;

	//public double side_length;

	public double x_length;
	public double y_length;


	//public int side_node_count;

	public int x_node_count;
	public int y_node_count;


	private double mass;

	private double structural_elasticity_coefficient;  // -kx .. relative to other node
	private double structural_damping_coefficient; // -bv .. velocities are again relative between nodes


	private double structural_rest_length_x;  // will probably need horizontal and vertical rest lengths for non-
	private double structural_rest_length_y;  // will probably need horizontal and vertical rest lengths for non-


	private double shear_rest_length;
	private double shear_elasticity_coefficient;
	private double shear_damping_coefficient;


	private double flex_rest_length_x;
	private double flex_rest_length_y;


	private double flex_elasticity_coefficient;
	private double flex_damping_coefficient;

	private Point3DCollection[] point_arrays;
	private int point_arrays_index;


	public GeometryModel3D model;


	public node_grid(
							Point3D position,
							//double side_length,

							double x_length, 
							double y_length,
		
							int x_node_count,
							int y_node_count,

							double mass,

							double structural_elasticity_coefficient,
							double structural_damping_coefficient,

							double shear_elasticity_coefficient,
							double shear_damping_coefficient,

							double flex_elasticity_coefficient,
							double flex_damping_coefficient


						  )
	{ 	// create a grid of edge x edge size AND count

		this.x_node_count = x_node_count;
		this.y_node_count = y_node_count;

		this.mass = mass;

		this.x_length = x_length;
		this.y_length = y_length;

		this.structural_rest_length_x = x_length / (x_node_count - 1);
		this.structural_rest_length_y = y_length / (y_node_count - 1);


    this.shear_rest_length = System.Math.Sqrt(structural_rest_length_x * structural_rest_length_x + structural_rest_length_y * structural_rest_length_y );




		this.flex_rest_length_x = 2.0 * structural_rest_length_x;
		this.flex_rest_length_y = 2.0 * structural_rest_length_y;




		this.structural_elasticity_coefficient = structural_elasticity_coefficient;// / structural_rest_length;
		this.structural_damping_coefficient = structural_damping_coefficient;// / structural_rest_length;

		this.shear_elasticity_coefficient = shear_elasticity_coefficient;// / shear_rest_length;
		this.shear_damping_coefficient = shear_damping_coefficient;// / shear_rest_length;

		this.flex_elasticity_coefficient = flex_elasticity_coefficient;// / flex_rest_length;
		this.flex_damping_coefficient = flex_damping_coefficient;// / flex_rest_length;



		point_arrays = new Point3DCollection[2];
		point_arrays[0] = new Point3DCollection();
		point_arrays[1] = new Point3DCollection();
		point_arrays_index = 0;



		grid = new node[x_node_count, y_node_count];

		double x_step = x_length / ((double)(x_node_count - 1));
		double z_step = y_length / ((double)(y_node_count - 1));

		double x_pos = position.X - (x_length / 2.0);

		for (int x = 0; x < x_node_count; x++) {
			
			double z_pos = position.Z - (y_length / 2.0);

			for (int y = 0; y < y_node_count; y++) {
				grid[x, y] = new node();
				grid[x, y].position = new Point3D(x_pos, position.Y, z_pos);	// WORTH LOOKING INTO CHANGING THIS AS APPROPRIATE!!!
				z_pos += z_step;

				point_arrays[0].Add(new Point3D());
				point_arrays[1].Add(new Point3D());

			}
			x_pos += x_step;
		}



		generate_model();






	}


	// clear all forces
	// compute all forces
	// compute all positions and velocities from forces and 


	public void update_physics_generate_next_positions()
	{

		update_structural_forces();

		update_shear_forces();

		update_flex_forces();





		for (int x = 0; x < x_node_count; x++) {
			for (int y = 0; y < y_node_count; y++) {
				grid[x, y].update_velocity_and_position(mass, 0.001 /* HAX00R3D UP */ );
			}
		}


	}





	public void commit_new_positions()
	{

		int pos = 0;

		for (int x = 0; x < x_node_count; x++) {
			for (int y = 0; y < y_node_count; y++) {
				point_arrays[point_arrays_index][pos++] = grid[x, y].position = grid[x, y].next_position;
			}
		}

		((MeshGeometry3D)(model.Geometry)).Positions = point_arrays[point_arrays_index];

		point_arrays_index = (point_arrays_index + 1) % 2;

	}






	public void update_brush_contents()
	{



	}






	void update_flex_forces()
	{


		for (int x = 0; x < x_node_count - 2; x++) {	// do these in pairs
			for (int y = 0; y < y_node_count; y++) {	// pairs

				grid[x, y].determine_internode_force(
					grid[x + 2, y],
					flex_rest_length_x,
					flex_elasticity_coefficient,
					flex_damping_coefficient);
			}
		}


		for (int x = 0; x < x_node_count; x++) {	// do these in pairs
			for (int y = 0; y < y_node_count - 2; y++) {	// pairs

				grid[x, y].determine_internode_force(
					grid[x, y + 2],
					flex_rest_length_y,
					flex_elasticity_coefficient,
					flex_damping_coefficient);
			}
		}


	}








	void update_shear_forces()
	{
		
		for (int x = 0; x < x_node_count - 1; x++) {	// do these in pairs
			for (int y = 0; y < y_node_count - 1; y++) {	// pairs

				grid[x, y].determine_internode_force(
					grid[x + 1, y + 1],
					shear_rest_length,
					shear_elasticity_coefficient,
					shear_damping_coefficient);
			}
		}
		 

		for (int x = 1; x < x_node_count; x++) {	// do these in pairs
			for (int y = 0; y < y_node_count - 1; y++) {	// pairs

				grid[x, y].determine_internode_force(
					grid[x - 1, y + 1],
					shear_rest_length,
					shear_elasticity_coefficient,
					shear_damping_coefficient);
			}
		}
		

	}








	void update_structural_forces()
	{
		

		for (int x = 0; x < x_node_count - 1; x++) {	// do these in pairs
			for (int y = 0; y < y_node_count; y++) {	// pairs

				grid[x, y].determine_internode_force(
					grid[x + 1, y],
					structural_rest_length_x,
					structural_elasticity_coefficient,
					structural_damping_coefficient);
			}
		}


		for (int x = 0; x < x_node_count; x++) {	// do these in pairs
			for (int y = 0; y < y_node_count - 1; y++) {	// pairs

				grid[x, y].determine_internode_force(
					grid[x, y + 1],
					structural_rest_length_y,
					structural_elasticity_coefficient,
					structural_damping_coefficient);
			}
		}

		
	}





	public void check_sphere_collision(sphere s)
	{

		for (int x = 0; x < x_node_count; x++) {
			for (int y = 0; y < y_node_count; y++) {
				s.node_hit_test(grid[x, y]);

			}
		}
	}




	public void check_box_collision(box box)
	{

		for (int x = 0; x < x_node_count; x++) {
			for (int y = 0; y < y_node_count; y++) {
				box.node_hit_test( grid[x, y] );
			}
		}

		

		for (int x = 0; x < x_node_count - 1; x++) {
			for (int y = 0; y < y_node_count; y++) {
				if (box.hit_test(grid[x, y].next_position, grid[x+1, y].next_position)) {
					grid[x, y].next_position = grid[x, y].position;
					grid[x, y].velocity = new Vector3D();

					grid[x+1, y].next_position = grid[x+1, y].position;
					grid[x+1, y].velocity = new Vector3D();
				}
			}
		}


		for (int x = 0; x < x_node_count; x++) {
			for (int y = 0; y < y_node_count - 1; y++) {
				if (box.hit_test(grid[x, y].next_position, grid[x, y+1].next_position)) {
					grid[x, y].next_position = grid[x, y].position;
					grid[x, y].velocity = new Vector3D();

					grid[x, y+1].next_position = grid[x, y+1].position;
					grid[x, y+1].velocity = new Vector3D();

				}
			}
		}
 

	}












	private void generate_model()
	{

		MeshGeometry3D mesh = new MeshGeometry3D();


		for (int x = 0; x < x_node_count; x++) { // n -> x
			for (int y = 0; y < y_node_count; y++) { // m -> y
				mesh.Positions.Add(  grid[x,y].position );
			}
		}



		for (int x = 0; x < x_node_count; x++) { // n -> x
			for (int y = 0; y < y_node_count; y++) { // m -> y
				mesh.TextureCoordinates.Add(new Point(1.0 - (((double)x) / ((double)(x_node_count - 1))), 1.0 -  (((double)y) / ((double)(y_node_count - 1)))));
			}
		}







		for (int x = 0; x < x_node_count - 1; x++) {
			for (int y = 0; y < y_node_count - 1; y++) {  

				if ((x + y) % 2 == 1) {

					mesh.TriangleIndices.Add(y + (x * y_node_count)); 
					mesh.TriangleIndices.Add((y + 1) + x * y_node_count);
					mesh.TriangleIndices.Add((y) + ((x + 1) * y_node_count));

					mesh.TriangleIndices.Add(y + ((x + 1) * y_node_count)); 
					mesh.TriangleIndices.Add((y + 1) + ((x) * y_node_count));
					mesh.TriangleIndices.Add((y + 1) + ((x + 1) * y_node_count));

				}

				else {

					mesh.TriangleIndices.Add(y + (x * y_node_count)); 
					mesh.TriangleIndices.Add((y + 1) + x * y_node_count);
					mesh.TriangleIndices.Add((y + 1) + ((x + 1) * y_node_count));

					mesh.TriangleIndices.Add(y + (x * y_node_count)); 
					mesh.TriangleIndices.Add((y + 1) + ((x + 1) * y_node_count));
					mesh.TriangleIndices.Add(y + ((x + 1) * y_node_count)); 
				}
			}
		}


		MaterialGroup material_group = new MaterialGroup();

		material_group.Children.Add(new EmissiveMaterial(MakeText(Brushes.DarkOrange)));
		material_group.Children.Add(new SpecularMaterial(MakeText(Brushes.White), 20));
		material_group.Children.Add(new EmissiveMaterial(MakeGrid()));
		material_group.Children.Add(new SpecularMaterial((Brushes.Gray), 10));

		model = new GeometryModel3D(mesh, material_group);
    model.BackMaterial = material_group;
    material_group.Freeze();

	}



	






	private DrawingBrush MakeVideoDrawingBrush()
	{
		MediaTimeline mt = new MediaTimeline(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, "C:\\2134.mpeg"), UriKind.Absolute));
		//MediaClock mc = mt.CreateClock();
		DrawingGroup dg = new DrawingGroup();

		//MediaPlayer mp = new MediaPlayer();
		MediaPlayer mp = new MediaPlayer();
		mp.Open(new Uri("C:\\2134.mpeg"));
		mp.Play();
	


		using (DrawingContext ctx = dg.Open()) {
			ctx.DrawVideo(mp, new Rect(0, 0, 30, 30));
		}

	//	dg.Freeze();

		return new DrawingBrush(dg);
	}












	static private ImageBrush MakeGrid()
	{

		DrawingVisual dv = new DrawingVisual();

		using (DrawingContext ctx = dv.RenderOpen()) {

			Pen yellow_pen = new Pen();
			yellow_pen.Brush = Brushes.Yellow;
			yellow_pen.Thickness = 2;
			yellow_pen.Freeze();
			// Horizontal Lines
			for (int n = 0; n < 100; n++) {
				ctx.DrawLine(yellow_pen, new Point(0, (1 / 99.0) * 10000 * n), new Point(10000, (1 / 99.0) * 10000 * n));
			}

			// Vertical Lines
			for (int n = 0; n < 100; n++) {
				ctx.DrawLine(yellow_pen, new Point((1 / 99.0) * 10000 * n, 0), new Point((1 / 99.0) * 10000 * n, 10000));
			}


		}


		RenderTargetBitmap rtb = new RenderTargetBitmap(1024, 1024, 96, 96, PixelFormats.Pbgra32);

		rtb.Render(dv);

		ImageBrush ib = new ImageBrush(rtb);

		return ib;
	}





	static private ImageBrush MakeText(Brush b)
	{

		DrawingVisual dv = new DrawingVisual();
		using (DrawingContext ctx = dv.RenderOpen()) {

            ctx.DrawText(new FormattedText("We <3 WPF\n We <3 WPF\n We <3 WPF\n We <3 WPF\n We <3 WPF\n We <3 WPF\n We <3 WPF\n We <3 WPF\n ", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 100, b), new Point(0, 0));

		}


		RenderTargetBitmap rtb = new RenderTargetBitmap(1024, 1024, 96, 96, PixelFormats.Pbgra32);

		rtb.Render(dv);

		ImageBrush ib = new ImageBrush(rtb);

		return ib;



	}










}



