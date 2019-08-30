using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class MessageDialog : BaseDialog
{
    TwoStageFuture _future = new TwoStageFuture();

    [SerializeField] Button _okButton;

    public IDelayedFuture Future { get { return _future; } }

	void Awake()
	{
		_okButton.onClick.AddListener( Close );
	}

    public void SetInfo( GameObject content, string title, string okTextCode )
    {
        SetContent( content, title );

        if( _okButton != null )
        {
            foreach( var text in _okButton.GetComponentsInChildren<Text>() ) text.SetLocalizedText( okTextCode );
            _okButton.onClick.AddListener( Close );
        }
        else Debug.LogError( "OkButton not setted" );
    }
    
    protected override bool Warning( float delay ) { _future.CompleteWarning( delay ); return true; }    
    protected override void Complete() { _future.ForceComplete(); }

    public void Close()
    {
        FadeAndDestroy();
    }
}
