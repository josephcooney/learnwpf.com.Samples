using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace VideoCarousel
{
	public class ListBox3DItem : DispatcherObject
	{

		#region Properties
		public const string BASE_DATA_DIR = @"media\";
        private MediaElement _FrontVideoDrawing = new MediaElement();
		private Model3DGroup _ItemGroup;
		private ScaleTransform3D _GroupScaleTransform;
		private TranslateTransform3D _GroupTranslateTransform;
		private RotateTransform3D _GroupRotateTransformY;
		private int _Status = 0;
		private string _VideoSrc;
		private double _angleY;

		public double AngleY
		{
			get { return _angleY; }
			set { _angleY = value; }
		}

		public Model3DGroup ItemGroup
		{
			get
			{
				return _ItemGroup;
			}
		}
		public string VideoSrc
		{
			get { return _VideoSrc; }
			set
			{
				_VideoSrc = value;
			}
		}
		public int Status
		{
			get { return _Status; }
			set { _Status = value; }
		}
		public double PlaybackVolume
		{
			get { return _FrontVideoDrawing.Volume; }
			set
			{
                if (_FrontVideoDrawing != null) _FrontVideoDrawing.Volume = value;
			
            }
		}

		#endregion

		#region Constructor

		public ListBox3DItem()
		{
			//here we get the geometry for the item
			_ItemGroup = GetMainGroup();
            
		}

		#endregion

		#region Public Methods


		public void Initialize()
		{
			if (this.VideoSrc != "")
			{
                if (_FrontVideoDrawing.Clock == null)
                {
                    //because our MediaElement is instantiated in code, we need to set its loaded and unloaded behavior to be manual
                    _FrontVideoDrawing.LoadedBehavior = MediaState.Manual;
                    _FrontVideoDrawing.UnloadedBehavior = MediaState.Manual;
                    MediaTimeline mt = new MediaTimeline(new Uri((String)this.VideoSrc, UriKind.Absolute));
                    //mt.RepeatBehavior = RepeatBehavior.Forever;
                    //there are issues w/ RepeatBehavior in the Feb CTP so instead we
                    //wire up the current state invalidated event to get repeat behavior
                    mt.CurrentStateInvalidated += new EventHandler(mt_CurrentStateInvalidated);
                    MediaClock mc = mt.CreateClock();
                    _FrontVideoDrawing.Clock = mc;
                    _FrontVideoDrawing.Width = 5;
                    _FrontVideoDrawing.Height = 10;
                    
                    VisualBrush db = new VisualBrush(_FrontVideoDrawing);
                    Brush br = db as Brush;
                    MaterialGroup mg = new MaterialGroup();
                    mg.Children.Add(new DiffuseMaterial(br));
                    GeometryModel3D gm3dFront = (GeometryModel3D)_ItemGroup.Children[0];
                    //only need to paint it one place to show up two places!
                    gm3dFront.Material = mg;

                }
			}
		}

        void mt_CurrentStateInvalidated(object sender, EventArgs e)
        {
            Clock c = (Clock)sender;
            if (c.CurrentState != ClockState.Active)
            {
    			_FrontVideoDrawing.Clock.Controller.Stop();
	    		_FrontVideoDrawing.Clock.Controller.Seek(
				TimeSpan.FromMilliseconds( 0 ),
				System.Windows.Media.Animation.TimeSeekOrigin.BeginTime );
                _FrontVideoDrawing.Clock.Controller.Begin();
            }

        }




		public void SetToDefaultPosition(Vector3D scale, Vector3D translate, Vector3D rotateY, double angleY )
		{
			AngleY = angleY;
			_GroupScaleTransform = new ScaleTransform3D(scale);
			_GroupTranslateTransform = new TranslateTransform3D(translate);
            _GroupRotateTransformY = new RotateTransform3D(new AxisAngleRotation3D(rotateY, angleY));
            Transform3DCollection tcollection = new Transform3DCollection();
			tcollection.Add(_GroupScaleTransform);
			tcollection.Add(_GroupRotateTransformY);
			tcollection.Add(_GroupTranslateTransform);
			// setup group transform
			Transform3DGroup tGroupDefault = new Transform3DGroup();
			tGroupDefault.Children = tcollection;
			_ItemGroup.Transform = tGroupDefault;
			return;
		}

		public void PlayVideo()
		{

            _FrontVideoDrawing.Clock.Controller.Begin();
		}
		
		
		#endregion

		#region Private Methods

		private Model3DGroup GetMainGroup()
		{
			Application _myApp = (Application)Application.Current;
			//note how it is copied here.
			return (_myApp.FindResource("ListItem3DModel3DGroup") as Model3DGroup).Clone();
		}
		
		#endregion

		
	}
}
