using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media.Imaging;


class box
{
	private Point3D position;

	private double[,] visual_extents;	//axis  {x,y,z},  {-,+}
	private double[,] hit_test_extents;	//axis  {x,y,z},  {-,+}

	public GeometryModel3D model;

	double epsilon;
	double hit_offset;





	public box(Point3D position, double x_length, double y_length, double z_length, double epsilon, double hit_offset)
	{
		this.position = position;
		this.epsilon = epsilon;
		this.hit_offset = hit_offset;

		visual_extents = new double[3, 2];	// axis  {x,y,z},  {-,+}

		visual_extents[0, 0] = position.X - (x_length / 2.0);
		visual_extents[0, 1] = position.X + (x_length / 2.0);

		visual_extents[1, 0] = position.Y - (y_length / 2.0);
		visual_extents[1, 1] = position.Y + (y_length / 2.0);

		visual_extents[2, 0] = position.Z - (z_length / 2.0);
		visual_extents[2, 1] = position.Z + (z_length / 2.0);

		generate_box_model();


		hit_test_extents = new double[3, 2];	// axis  {x,y,z},  {-,+}

		hit_test_extents[0, 0] = visual_extents[0, 0] - epsilon;
		hit_test_extents[0, 1] = visual_extents[0, 1] + epsilon;

		hit_test_extents[1, 0] = visual_extents[1, 0] - epsilon;
		hit_test_extents[1, 1] = visual_extents[1, 1] + epsilon;

		hit_test_extents[2, 0] = visual_extents[2, 0] - epsilon;
		hit_test_extents[2, 1] = visual_extents[2, 1] + epsilon;

	}





	private double[] point_to_array(Point3D point)
	{
		double[] array = new double[3];

		array[0] = point.X;
		array[1] = point.Y;
		array[2] = point.Z;

		return array;
	}




	public bool hit_test(Point3D point_a, Point3D point_b)
	{
		double[] a = point_to_array(point_a);
		double[] b = point_to_array(point_b);


		bool[,] plane_crossed = new bool[3, 2];
		bool plane_was_crossed = false;

		for (int axis = 0; axis < 3; axis++) {
			for (int sign = 0; sign < 2; sign++) {
				plane_crossed[axis, sign] = (a[axis] > visual_extents[axis, sign]) ^ (b[axis] > visual_extents[axis, sign]);
				plane_was_crossed |= plane_crossed[axis, sign];
			}
		}


		if (!plane_was_crossed) {
			return false;
		}

		double[] v = new double[3];

		for (int n = 0; n < 3; n++) {
			v[n] = a[n] - b[n];	// construct vector from a to b
		}

		bool[,] box_side_intersected = new bool[3, 2];

		for (int axis = 0; axis < 3; axis++) {

			if (v[axis] == 0) {	// the vector is 'this axis'-invariant
				continue;
			}

			for (int sign = 0; sign < 2; sign++) {
				if (plane_crossed[axis, sign]) {

					double s = (hit_test_extents[axis, sign] - a[axis]) / v[axis];	// vector scaler (scalar) to add to a to get i, the intersection

					double[] i = new double[3];

					for (int n = 0; n < 3; n++) {
						i[n] = a[n] + (s * v[n]);	// now i = the plane's point of intersection
					}

					box_side_intersected[axis, sign] =
						((i[(axis + 1) % 3] > hit_test_extents[(axis + 1) % 3, 0]) ^ (i[(axis + 1) % 3] > hit_test_extents[(axis + 1) % 3, 1])) &&
						((i[(axis + 2) % 3] > hit_test_extents[(axis + 2) % 3, 0]) ^ (i[(axis + 2) % 3] > hit_test_extents[(axis + 2) % 3, 1]));

				}
			}
		}

		for (int n = 0; n < 3; n++) {
			for (int m = 0; m < 2; m++) {
				if (box_side_intersected[n, m]) {
					return true;
					//	Console.WriteLine("a");
				}
			}
		}

		return false;


	}





	public void node_hit_test(node node_a)
	{

		double[] a = point_to_array(node_a.position);
		double[] b = point_to_array(node_a.next_position);

		bool[,] plane_crossed = new bool[3, 2];
		bool plane_was_crossed = false;

		for (int axis = 0; axis < 3; axis++) {
			for (int sign = 0; sign < 2; sign++) {
				plane_crossed[axis, sign] = (a[axis] > hit_test_extents[axis, sign]) ^ (b[axis] > hit_test_extents[axis, sign]);
				plane_was_crossed |= plane_crossed[axis, sign];
			}
		}


		if (!plane_was_crossed) {
			return;
		}

		double[] v = new double[3];

		for (int n = 0; n < 3; n++) {
			v[n] = b[n] - a[n];	// construct vector from a to b
		}

		double smallest_s = double.MaxValue;	// smallest vector length of a valid intersection
		bool side_was_intersected = false;
		int smallest_axis = 0;
		int smallest_sign = 0;



		for (int axis = 0; axis < 3; axis++) {

			if (v[axis] == 0) {	// the vector is 'this axis'-invariant
				continue;
			}

			for (int sign = 0; sign < 2; sign++) {
				if (plane_crossed[axis, sign]) {

					double s = (hit_test_extents[axis, sign] - a[axis]) / v[axis];	// vector scaler (scalar) to add to a to get i, the intersection

					double[] i = new double[3];

					for (int n = 0; n < 3; n++) {
						i[n] = a[n] + (s * v[n]);	// now i = the plane's point of intersection
					}

					if (((i[(axis + 1) % 3] > hit_test_extents[(axis + 1) % 3, 0]) ^
						(i[(axis + 1) % 3] > hit_test_extents[(axis + 1) % 3, 1]))
						&&
						((i[(axis + 2) % 3] > hit_test_extents[(axis + 2) % 3, 0]) ^
						(i[(axis + 2) % 3] > hit_test_extents[(axis + 2) % 3, 1]))) {	// intersection on a box side

						side_was_intersected = true;

						if (s < smallest_s) {	// smallest s corresponds to first collision
							smallest_s = s;
							smallest_axis = axis;
							smallest_sign = sign;
						}

					}

				}
			}
		}

		if (!side_was_intersected) {
			return;
		}

		double offset;

		if (smallest_sign == 0) {
			offset = -hit_offset;
		}
		else {
			offset = hit_offset;
		}

		switch (smallest_axis) {
			case 0:
				node_a.velocity.X = 0;
				node_a.next_position.X = hit_test_extents[smallest_axis, smallest_sign] + offset;
				break;
			case 1:
				node_a.velocity.Y = 0;
				node_a.next_position.Y = hit_test_extents[smallest_axis, smallest_sign] + offset;
				break;
			case 2:
				node_a.velocity.Z = 0;
				node_a.next_position.Z = hit_test_extents[smallest_axis, smallest_sign] + offset;
				break;
		}

	}





	private double determinant(Vector3D a, Vector3D b, Vector3D c)
	{
		return a.X * (b.Y * c.Z - b.Z * c.Y) - a.Y * (b.X * c.Z - b.Z * c.X) + a.Z * (b.X * c.Y - b.Y * c.X);
	}




	public void segment_hit_test(node node_a, node node_b)
	{

		double[] a = point_to_array(node_a.next_position);
		double[] b = point_to_array(node_b.next_position);

		bool[,] plane_crossed = new bool[3, 2];
		bool plane_was_crossed = false;

		for (int axis = 0; axis < 3; axis++) {
			for (int sign = 0; sign < 2; sign++) {
				plane_crossed[axis, sign] = (a[axis] > hit_test_extents[axis, sign]) ^ (b[axis] > hit_test_extents[axis, sign]);
				plane_was_crossed |= plane_crossed[axis, sign];
			}
		}


		if (!plane_was_crossed) {
			return;
		}

		double[] v = new double[3];

		for (int n = 0; n < 3; n++) {
			v[n] = b[n] - a[n];	// construct vector from a to b
		}

		double smallest_s = double.MaxValue;	// smallest vector length of a valid intersection
		bool side_was_intersected = false;
		int smallest_axis = 0;
		int smallest_sign = 0;



		for (int axis = 0; axis < 3; axis++) {

			if (v[axis] == 0) {	// the vector is 'this axis'-invariant
				continue;	// use epsilon here
			}

			for (int sign = 0; sign < 2; sign++) {
				if (plane_crossed[axis, sign]) {

					double s = (hit_test_extents[axis, sign] - a[axis]) / v[axis];	// vector scaler (scalar) to add to a to get i, the intersection

					double[] i = new double[3];

					for (int n = 0; n < 3; n++) {
						i[n] = a[n] + (s * v[n]);	// now i = the plane's point of intersection
					}

					if (((i[(axis + 1) % 3] > hit_test_extents[(axis + 1) % 3, 0]) ^
						(i[(axis + 1) % 3] > hit_test_extents[(axis + 1) % 3, 1]))
						&&
						((i[(axis + 2) % 3] > hit_test_extents[(axis + 2) % 3, 0]) ^
						(i[(axis + 2) % 3] > hit_test_extents[(axis + 2) % 3, 1]))) {	// intersection on a box side

						side_was_intersected = true;

						if (s < smallest_s) {	// smallest s corresponds to first collision
							smallest_s = s;
							smallest_axis = axis;
							smallest_sign = sign;
						}

					}

				}
			}
		}

		if (!side_was_intersected) {
			return;
		}

		double offset;

		if (smallest_sign == 0) {
			offset = -hit_offset;
		}
		else {
			offset = hit_offset;
		}

		switch (smallest_axis) {
			case 0:
				node_a.velocity.X = 0;
				node_a.next_position.X = hit_test_extents[smallest_axis, smallest_sign] + offset;
				break;
			case 1:
				node_a.velocity.Y = 0;
				node_a.next_position.Y = hit_test_extents[smallest_axis, smallest_sign] + offset;
				break;
			case 2:
				node_a.velocity.Z = 0;
				node_a.next_position.Z = hit_test_extents[smallest_axis, smallest_sign] + offset;
				break;
		}

	}















	private void generate_plane(MeshGeometry3D mesh, Point3D a, Point3D b, Point3D c, Point3D d)
	{
		mesh.Positions.Add(a);
		mesh.TextureCoordinates.Add(new Point(0, 1));

		mesh.Positions.Add(b);
		mesh.TextureCoordinates.Add(new Point(0, 0));

		mesh.Positions.Add(c);
		mesh.TextureCoordinates.Add(new Point(1, 0));

		mesh.Positions.Add(d);
		mesh.TextureCoordinates.Add(new Point(1, 1));


		mesh.TriangleIndices.Add(mesh.Positions.Count - 4);
		mesh.TriangleIndices.Add(mesh.Positions.Count - 3);
		mesh.TriangleIndices.Add(mesh.Positions.Count - 2);

		mesh.TriangleIndices.Add(mesh.Positions.Count - 2);
		mesh.TriangleIndices.Add(mesh.Positions.Count - 1);
		mesh.TriangleIndices.Add(mesh.Positions.Count - 4);
	}














	private void generate_box_model()
	{
		MeshGeometry3D mesh = new MeshGeometry3D();

		Point3D[] points = new Point3D[8];

		for (int z = 0; z < 2; z++) {	// corresponds to sign of z axis
			for (int y = 0; y < 2; y++) {
				for (int x = 0; x < 2; x++) {
					points[(z * 4) + (y * 2) + x] = new Point3D(visual_extents[0, x], visual_extents[1, y], visual_extents[2, z]);
				}
			}
		}

		generate_plane(mesh, points[6], points[4], points[5], points[7]);
		generate_plane(mesh, points[7], points[5], points[1], points[3]);

		generate_plane(mesh, points[3], points[1], points[0], points[2]);
		generate_plane(mesh, points[2], points[0], points[4], points[6]);

		generate_plane(mesh, points[2], points[6], points[7], points[3]);
		generate_plane(mesh, points[4], points[0], points[1], points[5]);


		MaterialGroup material_group = new MaterialGroup();

		material_group.Children.Add(new DiffuseMaterial(MakeGrid()));
		material_group.Children.Add(new SpecularMaterial(Brushes.White, 20));

		model = new GeometryModel3D(mesh, material_group);
	}










	static private ImageBrush MakeGrid()
	{

		DrawingVisual dv = new DrawingVisual();

		using (DrawingContext ctx = dv.RenderOpen()) {

			LinearGradientBrush lgb = new LinearGradientBrush();
			lgb.GradientStops.Add(new GradientStop(Colors.Black, 0));
			lgb.GradientStops.Add(new GradientStop(Colors.LightGray, 1));
			lgb.StartPoint = new Point(0, 0);
			lgb.EndPoint = new Point(1, 1);
			lgb.Freeze();

			ctx.DrawRectangle(lgb, null, new Rect(0, 0, 1, 1));

			Pen white_pen = new Pen();
			white_pen.Brush = Brushes.White;
			white_pen.Thickness = .05;
			white_pen.Freeze();

			ctx.DrawLine(white_pen, new Point(0.5, 0.3), new Point(0.5, 0.7));

			ctx.DrawLine(white_pen, new Point(0.3, 0.5), new Point(0.7, 0.5));


		}


		RenderTargetBitmap rtb = new RenderTargetBitmap(1024, 1024, 98304, 98304, PixelFormats.Pbgra32);



		rtb.Render(dv);

		ImageBrush ib = new ImageBrush(rtb);

		return ib;


	}




}