using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ConfirmationDialog : BaseDialog
{
    bool _result;
    TwoStageFutureWithPermission<bool> _future = new TwoStageFutureWithPermission<bool>();
    
    [SerializeField] Button _confirmationButton;
    [SerializeField] Button _cancelButton;
    
    public IDelayedFuture<bool> Future { get { return _future; } }
    public TwoStageFutureWithPermission<bool> FutureWithPermission { get { return _future; } }

	void Awake()
	{
		_confirmationButton.onClick.AddListener( Confirm );
		_cancelButton.onClick.AddListener( Cancel );
	}

    public void SetInfo( GameObject content, string titleTextCode, string confirmationTextCode, bool canConfirm, string cancelTextCode )
    {
        SetContent( content, titleTextCode );

        _confirmationButton.interactable = canConfirm;

        if( _confirmationButton != null )
        {
            foreach( var text in _confirmationButton.GetComponentsInChildren<Text>() ) text.SetLocalizedText( confirmationTextCode );
            _confirmationButton.onClick.AddListener( Confirm );
        }
        else Debug.LogError( "ConfirmationButton not setted" );

        if( _cancelButton != null )
        {
            foreach( var text in _cancelButton.GetComponentsInChildren<Text>() ) text.SetLocalizedText( cancelTextCode );
            _cancelButton.onClick.AddListener( Cancel );
        }
        else Debug.LogError( "CancelButton not setted" );
    }
    
    protected override bool Warning( float delay ) { _future.CompleteWarning( _result, delay ); return _future.CanClose( _result ); }    
    protected override void Complete() { _future.ForceComplete( _result ); }

    public void Confirm()
    {
        _result = true;
        FadeAndDestroy();
    }

    public void Cancel()
    {
        _result = false;
        FadeAndDestroy();
    }
}
