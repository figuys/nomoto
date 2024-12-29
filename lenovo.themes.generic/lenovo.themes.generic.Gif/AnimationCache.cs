using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace lenovo.themes.generic.Gif;

internal static class AnimationCache
{
	private class CacheKey
	{
		private readonly ImageSource _source;

		private readonly RepeatBehavior _repeatBehavior;

		public CacheKey(ImageSource source, RepeatBehavior repeatBehavior)
		{
			_source = source;
			_repeatBehavior = repeatBehavior;
		}

		private bool Equals(CacheKey other)
		{
			if (ImageEquals(_source, other._source))
			{
				return object.Equals(_repeatBehavior, other._repeatBehavior);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((CacheKey)obj);
		}

		public override int GetHashCode()
		{
			return (ImageGetHashCode(_source) * 397) ^ _repeatBehavior.GetHashCode();
		}

		private static int ImageGetHashCode(ImageSource image)
		{
			if (image != null)
			{
				Uri uri = GetUri(image);
				if (uri != null)
				{
					return uri.GetHashCode();
				}
			}
			return 0;
		}

		private static bool ImageEquals(ImageSource x, ImageSource y)
		{
			if (object.Equals(x, y))
			{
				return true;
			}
			if (x == null != (y == null))
			{
				return false;
			}
			if (x.GetType() != y.GetType())
			{
				return false;
			}
			Uri uri = GetUri(x);
			Uri uri2 = GetUri(y);
			if (uri != null)
			{
				return uri == uri2;
			}
			return false;
		}

		private static Uri GetUri(ImageSource image)
		{
			if (image is BitmapImage bitmapImage && bitmapImage.UriSource != null)
			{
				if (bitmapImage.UriSource.IsAbsoluteUri)
				{
					return bitmapImage.UriSource;
				}
				if (bitmapImage.BaseUri != null)
				{
					return new Uri(bitmapImage.BaseUri, bitmapImage.UriSource);
				}
			}
			if (image is BitmapFrame bitmapFrame)
			{
				string text = bitmapFrame.ToString();
				if (text != bitmapFrame.GetType().FullName && Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var result))
				{
					if (result.IsAbsoluteUri)
					{
						return result;
					}
					if (bitmapFrame.BaseUri != null)
					{
						return new Uri(bitmapFrame.BaseUri, result);
					}
				}
			}
			return null;
		}
	}

	private static readonly Dictionary<CacheKey, ObjectAnimationUsingKeyFrames> _animationCache = new Dictionary<CacheKey, ObjectAnimationUsingKeyFrames>();

	private static readonly Dictionary<CacheKey, int> _referenceCount = new Dictionary<CacheKey, int>();

	public static void IncrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
	{
		CacheKey key = new CacheKey(source, repeatBehavior);
		_referenceCount.TryGetValue(key, out var value);
		value++;
		_referenceCount[key] = value;
	}

	public static void DecrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
	{
		CacheKey key = new CacheKey(source, repeatBehavior);
		_referenceCount.TryGetValue(key, out var value);
		if (value > 0)
		{
			value--;
			_referenceCount[key] = value;
		}
		if (value == 0)
		{
			_animationCache.Remove(key);
			_referenceCount.Remove(key);
		}
	}

	public static void AddAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames animation)
	{
		CacheKey key = new CacheKey(source, repeatBehavior);
		_animationCache[key] = animation;
	}

	public static void RemoveAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames animation)
	{
		CacheKey key = new CacheKey(source, repeatBehavior);
		_animationCache.Remove(key);
	}

	public static ObjectAnimationUsingKeyFrames GetAnimation(ImageSource source, RepeatBehavior repeatBehavior)
	{
		CacheKey key = new CacheKey(source, repeatBehavior);
		_animationCache.TryGetValue(key, out var value);
		return value;
	}
}
