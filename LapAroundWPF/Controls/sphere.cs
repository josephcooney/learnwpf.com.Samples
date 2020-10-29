using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;

class sphere
{
	private double radius;

	private double horizontal_angle_DEG = 360;
	private double vertical_angle_DEG = 180;

	private int horizontal_segments;
	private int vertical_segments;

	private Material face_material;



	private Point3D[,] outer_curved_grid;
	//private Vector3D[,] outer_straightening_vectors;

	public Model3DGroup all_models;




	private MeshGeometry3D outer_face_mesh;
	private GeometryModel3D outer_face_geometry_model;

	private Point3D position;

	double epsilon;
	double hit_offset;


	private double deg_to_rad(double degrees) { return (degrees * Math.PI / 180.0); }



	public sphere(
		Point3D position,
		double radius,
		int horizontal_segments,
		int vertical_segments,
		double epsilon,
		double hit_offset,
		Material face_material
	)

	{
		this.position = position;

		this.radius = radius;

		this.horizontal_segments = horizontal_segments;
		this.vertical_segments = vertical_segments;

		this.epsilon = epsilon;
		this.hit_offset = hit_offset;

		this.face_material = face_material;

		make_model();
	}




	private void make_model()
	{
	

		int horizontal_vertex_count = horizontal_segments + 1;
		int vertical_vertex_count = vertical_segments + 1;


		outer_curved_grid = new Point3D[horizontal_vertex_count, vertical_vertex_count];


		all_models = new Model3DGroup();

		outer_face_mesh = new MeshGeometry3D();


		for (int y = 0; y < vertical_vertex_count; y++) {
			for (int x = 0; x < horizontal_vertex_count; x++) {
				outer_face_mesh.Positions.Add(new Point3D());
			}
		}

		double alpha_start_RAD = deg_to_rad(-vertical_angle_DEG / 2.0);
		double beta_start_RAD = deg_to_rad(-horizontal_angle_DEG / 2.0);

		double alpha_RAD = alpha_start_RAD;
		double beta_RAD = beta_start_RAD;

		double alpha_increment_RAD = deg_to_rad(vertical_angle_DEG / ((double)vertical_segments));
		double beta_increment_RAD = deg_to_rad(horizontal_angle_DEG / ((double)horizontal_segments));


		for (int y = 0; y < vertical_vertex_count; y++) {

			double cos_alpha = Math.Cos(alpha_RAD);
			double sin_alpha = Math.Sin(alpha_RAD);

			for (int x = 0; x < horizontal_vertex_count; x++) {

				double cos_beta = Math.Cos(beta_RAD);
				double sin_beta = Math.Sin(beta_RAD);

				outer_curved_grid[x, y] = new Point3D(
					(-(radius) * cos_alpha * sin_beta),
					((radius) * sin_alpha),
					((radius) * cos_alpha * cos_beta));

				outer_curved_grid[x, y] += ((Vector3D)(position));


				beta_RAD += beta_increment_RAD;

			}
			beta_RAD = beta_start_RAD;
			alpha_RAD += alpha_increment_RAD;
		}

		// set mesh positions
		for (int y = 0; y < vertical_vertex_count; y++) {
			for (int x = 0; x < horizontal_vertex_count; x++) {
				outer_face_mesh.Positions[(y * horizontal_vertex_count) + x] = outer_curved_grid[x, y];
			}
		}

		// create mesh texture coordinates
		for (int y = 0; y < vertical_vertex_count; y++) {
			for (int x = 0; x < horizontal_vertex_count; x++) {

				outer_face_mesh.TextureCoordinates.Add(
					new System.Windows.Point(
						1.0 - ((double)x) / ((double)(horizontal_vertex_count - 1.0)),
						1.0 - ((double)y) / ((double)(vertical_vertex_count - 1.0)))
					);
			}
		}

		// set mesh vertex indices
		for (int y = 0; y < vertical_segments; y++) {
			for (int x = 0; x < horizontal_segments; x++) {

				outer_face_mesh.TriangleIndices.Add(x + (y * horizontal_vertex_count)); // front face
				outer_face_mesh.TriangleIndices.Add(x + ((y + 1) * horizontal_vertex_count));
				outer_face_mesh.TriangleIndices.Add((x + 1) + (y * horizontal_vertex_count));

				outer_face_mesh.TriangleIndices.Add(x + ((y + 1) * horizontal_vertex_count)); // front face
				outer_face_mesh.TriangleIndices.Add((x + 1) + ((y + 1) * horizontal_vertex_count));
				outer_face_mesh.TriangleIndices.Add((x + 1) + (y * horizontal_vertex_count));

			}
		}

		outer_face_geometry_model = new GeometryModel3D(outer_face_mesh, face_material);

		all_models.Children.Add(outer_face_geometry_model);

	}







	public void node_hit_test(node node_a)
	{
		Vector3D v = node_a.next_position - position;
		double length = v.Length;

		if (length > (radius + epsilon)) {
			return;
		}

		double offset_mag = (radius + epsilon + hit_offset) - length;

		v.Normalize();

		node_a.next_position += v * offset_mag;

		double velocity_mag_delta = Vector3D.DotProduct(node_a.velocity, v);
		node_a.velocity -= v * velocity_mag_delta;
	}



}





