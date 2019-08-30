

public interface IDelayedFutureBase : IFuture { bool IsWarned { get; } }
public interface IDelayedFutureBase<Result> : IFuture<Result> { bool IsWarned { get; } }

public interface IDelayedFutureObserver : IDelayedFutureBase
{
	IEventRegister<float> OnCompletionWarning { get; }
}

public interface IDelayedFutureObserver<Result> : IDelayedFutureBase<Result>
{
	IEventRegister<Result, float> OnCompletionWarning { get; }
}

public interface IDelayedFuture : IDelayedFutureObserver
{
	void CompleteWarning( float time );
}

public interface IDelayedFuture<Result> : IDelayedFutureObserver<Result>
{
	void CompleteWarning( Result t, float time );
}

class TwoStageFuture : Future, IDelayedFuture
{
	bool _isWarned = false;

	EventSlot<float> _warning = new EventSlot<float>();

	public IEventRegister<float> OnCompletionWarning { get { return _warning; } }

	public void CompleteWarning( float seconds )
	{
		if( _isWarned )
			return;

		_isWarned = true;
		_warning.Trigger( seconds );
	}

	public bool IsWarned { get { return _isWarned; } }
}

public class TwoStageFuture<Result> : Future<Result>, IDelayedFuture<Result>
{
	bool _isWarned = false;

	EventSlot<Result, float> _warning = new EventSlot<Result, float>();

	public IEventRegister<Result, float> OnCompletionWarning { get { return _warning; } }

	public void CompleteWarning( Result result, float seconds )
	{
		if( _isWarned )
			return;

		_isWarned = true;
		_warning.Trigger( result, seconds );
	}

	public bool IsWarned { get { return _isWarned; } }
}

public class TwoStageFutureWithPermission<Result> : TwoStageFuture<Result>
{
	System.Func<Result, bool> _warningCanClose = null;
	public System.Func<Result, bool> OnWarningCanClose { set { _warningCanClose = value; } }

	public bool CanClose( Result t )
	{
		if( _warningCanClose == null ) return true;
		var result = _warningCanClose( t );
		return result;
	}
}
