using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace lenovo.themes.generic.Gif;

public class ImageAnimationController : IDisposable
{
	private static readonly DependencyPropertyDescriptor _sourceDescriptor;

	private readonly Image _image;

	private readonly ObjectAnimationUsingKeyFrames _animation;

	private readonly AnimationClock _clock;

	private readonly ClockController _clockController;

	public int FrameCount => _animation.KeyFrames.Count;

	public TimeSpan Duration
	{
		get
		{
			if (!_animation.Duration.HasTimeSpan)
			{
				return TimeSpan.Zero;
			}
			return _animation.Duration.TimeSpan;
		}
	}

	public bool IsPaused => _clock.IsPaused;

	public bool IsComplete => _clock.CurrentState == ClockState.Filling;

	public int CurrentFrame
	{
		get
		{
			TimeSpan? time = _clock.CurrentTime;
			return _animation.KeyFrames.Cast<ObjectKeyFrame>().Select((ObjectKeyFrame f, int i) => new
			{
				Time = f.KeyTime.TimeSpan,
				Index = i
			}).FirstOrDefault(fi =>
			{
				TimeSpan time2 = fi.Time;
				TimeSpan? timeSpan = time;
				return time2 >= timeSpan;
			})?.Index ?? (-1);
		}
	}

	public event EventHandler CurrentFrameChanged;

	static ImageAnimationController()
	{
		_sourceDescriptor = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
	}

	internal ImageAnimationController(Image image, ObjectAnimationUsingKeyFrames animation, bool autoStart)
	{
		_image = image;
		_animation = animation;
		_animation.Completed += AnimationCompleted;
		_clock = _animation.CreateClock();
		_clockController = _clock.Controller;
		_sourceDescriptor.AddValueChanged(image, ImageSourceChanged);
		_clockController.Pause();
		_image.ApplyAnimationClock(Image.SourceProperty, _clock);
		if (autoStart)
		{
			_clockController.Resume();
		}
	}

	private void AnimationCompleted(object sender, EventArgs e)
	{
		_image.RaiseEvent(new RoutedEventArgs(ImageBehavior.AnimationCompletedEvent, _image));
	}

	private void ImageSourceChanged(object sender, EventArgs e)
	{
		OnCurrentFrameChanged();
	}

	public void GotoFrame(int index)
	{
		ObjectKeyFrame objectKeyFrame = _animation.KeyFrames[index];
		_clockController.Seek(objectKeyFrame.KeyTime.TimeSpan, TimeSeekOrigin.BeginTime);
	}

	public void Pause()
	{
		_clockController.Pause();
	}

	public void Play()
	{
		_clockController.Resume();
	}

	private void OnCurrentFrameChanged()
	{
		this.CurrentFrameChanged?.Invoke(this, EventArgs.Empty);
	}

	~ImageAnimationController()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_image.BeginAnimation(Image.SourceProperty, null);
			_animation.Completed -= AnimationCompleted;
			_sourceDescriptor.RemoveValueChanged(_image, ImageSourceChanged);
			_image.Source = null;
		}
	}
}
