using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Input;
using System.Windows.Threading;
using HitTestResult2D = System.Windows.Media.HitTestResult;

namespace VideoCarousel
{
	public class ListBox3D : ListBox
	{
		#region Properties

		private double _Radius = 2.4;
		private double _PerpendicularAngle = 90;
		private double _ItemOffsetAngle = 0;
		private int _FrontmostItemIndex = 0;
		private DispatcherTimer _VolumeAdjustTimer;
		private RotateTransform3D _GroupRotateTransformY;
		private TranslateTransform3D _GroupTranslateTransform;
		private ScaleTransform3D _GroupScaleTransform;
		private bool _IsAutoRotating = false;
		private int _AutoRotationState = 0;
		private const int AUTOROTATION_ADVANCE_DURATION = 1000;
		private const int CAROUSEL_ACTIVATION_ANIMATION_DURATION_MS = 2000;
		private const int AUTOROTATION_SHOWCASE_DURATION = 9000;
		private int ADJUST_VOLUME_INTERVAL = 200;
		private const double SHOWCASE_ANGLE = 14;
		private Model3DGroup _ModelLights;
		private Viewport3D _MainViewport3D;
		private ListBox3DItem _ciHitTest;
        private double _ciHitTestDistance;
		private Model3DGroup _MainGroup;
		private Model3DGroup _ModelItems;
		public int AutoRotationState
		{
			get { return _AutoRotationState; }
		}
		public bool IsAutoRotating
		{
			get { return _IsAutoRotating; }
		}
		public double CurrentRotationAngle
		{
            get { return ((AxisAngleRotation3D)(_GroupRotateTransformY.Rotation)).Angle; }
        }
		#endregion

		#region Private Methods

		private void OnInitialized(object sender, EventArgs e)
		{
			InitializeModels();
		}

		private void InitializeModels()
		{
			// create Main Model group - light and transforms for all sub model groups go here
			_MainGroup = new Model3DGroup();

			// Create default Transform collection
			// Add transform collection to the _MainGroup
			_GroupScaleTransform = new ScaleTransform3D(new Vector3D(1, 1, 1));
            _GroupRotateTransformY = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0,1,0), 0),
                new Point3D(0, 0, 0));
            _GroupTranslateTransform = new TranslateTransform3D(new Vector3D(0, 1, 0));
			Transform3DCollection tcollection = new Transform3DCollection();
			tcollection.Add(_GroupScaleTransform);
			tcollection.Add(_GroupRotateTransformY);
			tcollection.Add(_GroupTranslateTransform);

			// setup group transform
			Transform3DGroup tGroupDefault = new Transform3DGroup();
			tGroupDefault.Children = tcollection;
			_MainGroup.Transform = tGroupDefault;

			// Create sub model group [0] for the light
			//
			_ModelLights = new Model3DGroup();
			AmbientLight light1 = new AmbientLight(Colors.White);
			_ModelLights.Transform = tGroupDefault.Clone();
			_ModelLights.Children.Add(light1);
			_MainGroup.Children.Add(_ModelLights);
			_ModelItems = new Model3DGroup();
			_MainGroup.Children.Add(_ModelItems);

		}

		// called after properties are set
		private void OnLoaded(object sender, EventArgs e)
		{
			InitializeViewport3D();

		}
		private void InitializeViewport3D()
		{
			FrameworkElement f = this as FrameworkElement;

			//Because the viewport is nested inside a control template, we need to 
			//extract it
			FrameworkElement viewport3DElement = FindViewport3D(f); // since the Viewport3D is in the style, we need to find it.
			_MainViewport3D = viewport3DElement as Viewport3D;

            if (_MainViewport3D != null)
            {
                ModelVisual3D v3d = new ModelVisual3D();
                v3d.Content = _MainGroup;
                _MainViewport3D.Children.Add(v3d);
            }
        }

        private FrameworkElement FindViewport3D(Visual parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                Visual visual = (Visual) VisualTreeHelper.GetChild(parent, i);

                if ((visual is FrameworkElement) && (visual is Viewport3D))
                {
                    return (visual as FrameworkElement);
                }
                else
                {
                    FrameworkElement result = FindViewport3D(visual);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

		public void Add(string VideoSrc)
		{
			ListBox3DItem expListItem = new ListBox3DItem();
			expListItem.VideoSrc = VideoSrc;
			this.AddChild(expListItem);
		}

		public void Build()
		{

			Vector3D axis = new Vector3D(0, 1, 0);
			Vector3D scale = new Vector3D(2.5, 1.5, .2);
			
			//this allows us to dynamically add items to the carousel
			_ItemOffsetAngle = 360 / this.Items.Count;
			//these are private variable we will use to layout each plane
			double angle = 360.0 / this.Items.Count;
			double totalAngle = 0;
			// Layout the meshes
			for (int i = 0; i < this.Items.Count; i++)
			{
				//create a translation vector relative to the angle
				Vector3D tVector = GetTranslationOffsetForCarouselAngle(totalAngle);
				// fetch the list3DItem 
				ListBox3DItem li = this.Items[i] as ListBox3DItem;
				//set some properties
				li.Status = (int)Carousel3DItemStates.Unselected;
				li.SetToDefaultPosition(scale, tVector, axis, _PerpendicularAngle - totalAngle);
                

				//add it to the _ModelItem Model3DGroup
				_ModelItems.Children.Add(li.ItemGroup);
				//here we set up the video on the mesh
				li.Initialize();
				//increment angle
				totalAngle += angle;
			}
		}
		public void Activate()
		{
			foreach (ListBox3DItem item in this.Items)
			{
				item.Status = (int)Carousel3DItemStates.AnimatingUnselected;
				item.PlayVideo();
			}

			_AutoRotationState = (int)Carousel3DRotationStates.ActivationRotate;
			StartAutoRotation();
		}

		private Vector3D GetTranslationOffsetForCarouselAngle(double angle)
		{
			double radian = Math.PI * angle / 180.0;
			double x = _Radius * Math.Cos(radian);
			double z = _Radius * Math.Sin(radian);
			return new Vector3D(x, 0, z);
		}

		private double GetAnglePositionForItem(ListBox3DItem item)
		{
			return -(item.AngleY - _PerpendicularAngle);
		}

		/// Given that the carousel just keeps winding, return the new 0 degree angle factoring
		/// in the number of full rotations that have been made
		private double GetZeroAngle()
		{
			double numFullRotations = Math.Floor(this.CurrentRotationAngle / 360);
			return numFullRotations * 360;
		}

		/// Given an item, returns the angle of rotation required to locate that item
		/// directly in front of the user
		private double GetFrontmostAngleForItem(ListBox3DItem item)
		{
			// with no rotation, 90 degrees is the frontmost position
			// we need to account for the number of rotations past 360 the carousel has made and factor
			// that in to our result.
			// find the shortest distance from the current position to the front, which will determine
			// the direction of rotation

			double baseAngle = GetZeroAngle() + GetAnglePositionForItem(item);
			double frontAngle1 = baseAngle - 90;
			double frontAngle2 = baseAngle + 270;
			double distAngle1 = Math.Abs(frontAngle1 - CurrentRotationAngle);
			double distAngle2 = Math.Abs(frontAngle2 - CurrentRotationAngle);
			double shortestDist = Math.Min(distAngle1, distAngle2);

			return shortestDist == distAngle1 ? frontAngle1 : frontAngle2;
		}


		/// Returns the relative position of the item on a scale of -180 to 180, where 0 is directly in front of the viewer
		private double GetRelativeAngleForItem(ListBox3DItem item)
		{
			double angle = -((GetAnglePositionForItem(item) - (CurrentRotationAngle - GetZeroAngle())) - 90);
			if (angle % 360 == 0) angle = 0;

			if (angle > 180)
			{
				angle -= 360;
			}


			return angle;
		}
		private ListBox3DItem GetFrontmostItem()
		{
			return this.Items[_FrontmostItemIndex] as ListBox3DItem;
		}
		#endregion

		#region Public Methods

		public ListBox3D()
		{
			this.Initialized += new EventHandler(OnInitialized);
			this.Loaded += new RoutedEventHandler(OnLoaded);
		}


		public void StartAutoRotation()
		{
			if (_IsAutoRotating) return;

			_IsAutoRotating = true;

			if (_VolumeAdjustTimer == null)
			{
				_VolumeAdjustTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(ADJUST_VOLUME_INTERVAL),  //Time to wait
									 DispatcherPriority.Background, // Priority
									 new EventHandler(this.OnVolumeAdjustTimer),  //Handler
									 this.Dispatcher);  // Current dispatcher.
			}

			if ( _AutoRotationState != (int)Carousel3DRotationStates.ActivationRotate)
			{
				ShowcaseItem();
			}
			else
			{
				RotateToNextItem();
			}
			_VolumeAdjustTimer.Start();
		}

        public bool DoHitTest(Visual target, Point p)
        {
            _ciHitTest = null; // _HitMesh will be update by HTResult delegate when intersection occurs.
            _ciHitTestDistance = 9999999.0;

            VisualTreeHelper.HitTest(target, null, new HitTestResultCallback(HTResult),
                new PointHitTestParameters(p));

            return (_ciHitTest != null);
        }
        public HitTestResultBehavior HTResult(HitTestResult2D result2d)
        {
            RayMeshGeometry3DHitTestResult rayht = (result2d as RayMeshGeometry3DHitTestResult);
            Model3D model = rayht.ModelHit;

            foreach (ListBox3DItem ci in this.Items)
            {
                if (ci.ItemGroup.Children.Contains(model))
                {
                    if (rayht.DistanceToRayOrigin < _ciHitTestDistance)
                    {
                        _ciHitTest = ci;
                        _ciHitTestDistance = rayht.DistanceToRayOrigin;
                    }
                }
            }

            return HitTestResultBehavior.Continue;
        }

        #endregion

		#region Animation

		private void RotateModel(double start, double end, int duration)
		{
            RotateTransform3D rt3D = _GroupRotateTransformY;
            Rotation3D r3d = rt3D.Rotation;
			DoubleAnimation anim = new DoubleAnimation();
			anim.From = start;
			anim.To = end;
			anim.BeginTime = null;
			anim.AccelerationRatio = 0.1;
			anim.DecelerationRatio = 0.6;
			anim.Duration = new TimeSpan(0, 0, 0, 0, duration);
			AnimationClock ac = anim.CreateClock();
			ac.CurrentStateInvalidated += new EventHandler(OnRotateEnded);
			ac.Controller.Begin();
            r3d.ApplyAnimationClock(AxisAngleRotation3D.AngleProperty, ac);
        }
		public void OnRotateEnded(object sender, EventArgs args)
		{
			if (sender == null)
				return;
			Clock clock = sender as Clock;
			if (clock == null)
				return;
			if (clock.CurrentState == ClockState.Filling)
			{
				if (this.IsAutoRotating)
				{
					if (this.AutoRotationState == (int)Carousel3DRotationStates.ShowcaseRotate)
					{
						this.RotateToNextItem();
					}
					else
					{
						this.ShowcaseItem();
					}
				}
				clock.CurrentStateInvalidated -= new EventHandler(this.OnRotateEnded);
				clock = null;
			}
		}

		public void RotateToNextItem()
		{

			_FrontmostItemIndex++;
			if (_FrontmostItemIndex > this.Items.Count - 1) _FrontmostItemIndex = 0;
			ListBox3DItem nextItem = GetFrontmostItem();
			double start = CurrentRotationAngle;
			double end = GetFrontmostAngleForItem(nextItem) - (SHOWCASE_ANGLE / 2);
			if (!_IsAutoRotating) return;
			RotateModel(start, end, AUTOROTATION_ADVANCE_DURATION);
			_AutoRotationState = (int)Carousel3DRotationStates.AdvanceRotate;
		}

		public void ShowcaseItem()
		{
			double start = CurrentRotationAngle;
			double end = start + SHOWCASE_ANGLE;
			this.RotateModel(start, end, AUTOROTATION_SHOWCASE_DURATION);
			_AutoRotationState = (int)Carousel3DRotationStates.ShowcaseRotate;

		}

		#endregion


		public void OnPreviewLeftClick(object sender, MouseButtonEventArgs e)
		{
			Point p = e.GetPosition(this);

			DoHitTest(_MainViewport3D, p);
			//here we get back the selected listitem and can do with it what we will
			if ((_ciHitTest != null))
			{
				ListBox3DItem[] ListBox3DItemList = { _ciHitTest };
				ListBox3DItem[] ListBox3DItemListRemoved = { };
				//we also raise the OnSelectionChanged event for anyone listening	
				OnSelectionChanged(new SelectionChangedEventArgs(ListBox3D.SelectionChangedEvent, ListBox3DItemListRemoved, ListBox3DItemList));
			}
		}

		/// <summary>
		/// Adjusts the volume of all videos in the carousel to ensure that only the frontmost video can be heard
		/// </summary>
		private void OnVolumeAdjustTimer(object sender, EventArgs args)
		{
			double itemSpacing = 360 / this.Items.Count;
			double dist = 0;
			double overlap = itemSpacing / this.Items.Count;
			double activeZone = (itemSpacing + overlap) / 2;

			foreach (ListBox3DItem item in this.Items)
			{
				// Calculate distance from 0 degrees
				dist = Math.Abs(0 - this.GetRelativeAngleForItem(item));
				if (dist <= (itemSpacing + overlap) / 2)
				{
					item.PlaybackVolume = (activeZone - dist) / activeZone;
				}
				else
				{
					item.PlaybackVolume = 0;
				}
			}
		}
	}

	#region Public Enums

	public enum Carousel3DRotationStates
	{
		ActivationRotate,
		AdvanceRotate,
		ShowcaseRotate
	}

	public enum Carousel3DItemStates
	{
		Unselected,
		Showcased,
		AnimatingUnselected,
		AnimatingShowcased,
		AnimatingUnshowcased
	}

	#endregion




}
