using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class BaseDialog : MonoBehaviour
{
    [SerializeField] RectTransform _contentParent;
	//[SerializeField] GameObject _root;
	[SerializeField] Text _title;
    //[SerializeField] RectTransform _panel;

    //GameObject Root { get { return _root ?? gameObject; } }

	protected void SetContent( GameObject content, string title )
    {
        bool hasTitle = ( title != null && title != "" );
		_title.transform.parent.gameObject.SetActive( hasTitle );
        if( hasTitle ) _title.SetLocalizedText( title );


        if( content != null )
        {
			var t = content.transform;
			t.parent = _contentParent;
			t.localScale = Vector3.one;
			t.localPosition = Vector3.zero;

			var rectTransform = t as RectTransform;
			if( rectTransform != null ) 
			{
				rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, _contentParent.sizeDelta.y - 110 );
				rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, _contentParent.sizeDelta.x - 80 );
				rectTransform.anchoredPosition = rectTransform.anchoredPosition + Vector2.up * 10;
			}
//			t.localPosition = ( hasTitle ) ? Vector3.zero : new Vector3( 0, 20, 0 );
            //Vector2 min = Vector2.one * float.MaxValue;
            //Vector2 max = Vector2.one * float.MinValue;
            
            //foreach( var rend in content.GetComponentsInChildren<Renderer>() )
            //{
            //    min.x = Mathf.Min( min.x, rend.bounds.min.x );
            //    min.y = Mathf.Min( min.y, rend.bounds.min.y );
            //    max.x = Mathf.Max( max.x, rend.bounds.max.x );
            //    max.y = Mathf.Max( max.y, rend.bounds.max.y );
            //}
            
            //SetSize( max - min, Vector3.one * .4f );
        }
    }
    
//    protected void SetSize( Vector2 size, Vector2 margin )
//    {
////        var textureSize = new Vector2( 175, 59 );
//        var textureSize = new Vector2( 32, 32 );

//        size = Vector2.Max( ( size + margin ), new Vector2( 1.75f, .75f ) );

//        //m_bkg.dimensions = new Vector2( size.x * textureSize.x / ( m_bkg.scale.x * m_bkg.transform.lossyScale.x ),
//        //                                size.y * textureSize.y / ( m_bkg.scale.y * m_bkg.transform.lossyScale.x ) );
//    }

    protected abstract bool Warning( float delay );
    protected abstract void Complete();
    
    protected void FadeAndDestroy()
    {
        float delay = .25f;

		var animator = GetComponentInChildren<Animator>();
		if( animator != null )
		{
			animator.SetBool( "IsActive", false );
			StartCoroutine( DelayDestroy( delay ) );
		}
		else
		{
        	DestroyGameObject();
		}
    }

	IEnumerator DelayDestroy( float seconds )
	{
		while( seconds > 0 )
		{
			seconds -= Time.unscaledDeltaTime;
			yield return null;
		}
		DestroyGameObject();
	}

    void DestroyGameObject()
    {
        Complete();
        //GameObject.Destroy( Root );
        GameObject.Destroy( gameObject );
    }
}
