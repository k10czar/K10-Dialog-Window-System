using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using K10.Localization;

public class DialogServiceDebug : MonoBehaviour
{
	[TextArea(10, 50), SerializeField] string _s;

	void Update() { _s = DialogService.DebugString; }
}

public static class DialogService
{
	class DialogOrder
	{
		int _order;
		Func<bool> _isValid;

		public int Order { get { return _order; } }
		public bool IsValid { get { return _isValid(); } }

		public DialogOrder(int order, Func<bool> isValid)
		{
			_order = order;
			_isValid = isValid;
		}

		public override string ToString()
		{
			return string.Format("[DialogOrder: Order={0}, IsValid={1}]", Order, IsValid);
		}
	}

	static List<DialogOrder> _dialogs = new List<DialogOrder>();

	public static string DebugString
	{
		get
		{
			string s = "";
			for (int i = 0; i < _dialogs.Count; i++) s += _dialogs[i];
			return s;
		}
	}

	public static bool HasDialog
	{
		get
		{
			var debug = Guaranteed<DialogServiceDebug>.Instance;
			for (int i = _dialogs.Count - 1; i >= 0; i--)
			{
				if (!_dialogs[i].IsValid)
					_dialogs.RemoveAt(i);
			}
			return _dialogs.Count > 0;
		}
	}

	public static void SetTopOrder(GameObject go, IFutureBase future)
	{
		int order = 3;

		for (int i = _dialogs.Count - 1; i >= 0; i--)
		{
			if (!_dialogs[i].IsValid) _dialogs.RemoveAt(i);
			else order = Mathf.Max(_dialogs[i].Order + 1, order);
		}

		var canvas = go.GetComponent<Canvas>();
		canvas.sortingOrder = order;

		go.transform.position += new Vector3(0, 0, order * .1f);

		_dialogs.Add(new DialogOrder(order, () => !future.IsComplete && go != null));
	}

	public static IDelayedFuture Message(string message) { return Message(message, ""); }
	public static IDelayedFuture Message(string message, string title) { return Message(message, title, "DefaultMessageBoxButton"); }
	public static IDelayedFuture Message(string message, string title, string okButton)
	{
		var res = Resources.Load("Dialog/Content/GenericMessage");

		if (res != null)
		{
			var go = (GameObject)GameObject.Instantiate(res);
			var text = go.GetComponentInChildren<Text>();
			text.SetLocalizedText(message);
			return Message(go, title, okButton);
		}

		return Message(default(GameObject), title, okButton);
	}



	public static IDelayedFuture Message(GameObject popup)
	{
		if (popup != null)
		{
			var go = (GameObject)GameObject.Instantiate(popup);
			var dialog = go.GetComponentInChildren<MessageDialog>();

			dialog.GetComponentInChildren<Animator>().SetBool("IsActive", true);
			SetTopOrder(go, dialog.Future);

			return dialog.Future;
		}

		return null;
	}



	public static IDelayedFuture Message(GameObject content, string title, string okButton)
	{
		var res = Resources.Load("Dialog/MessageBox");

		if (res != null)
		{
			var go = (GameObject)GameObject.Instantiate(res);
			var dialog = go.GetComponentInChildren<MessageDialog>();

			dialog.GetComponentInChildren<Animator>().SetBool("IsActive", true);
			SetTopOrder(go, dialog.Future);

			dialog.SetInfo(content, title, okButton);

			return dialog.Future;
		}

		return null;
	}

	public static TwoStageFutureWithPermission<bool> Confirmation(string message, string title) { return Confirmation(message, title, "DefaultConfirmationBoxYesButton", "DefaultConfirmationBoxNoButton"); }
	public static TwoStageFutureWithPermission<bool> Confirmation(string message, string title, string okButton, string cancelButton) { return Confirmation(message, title, okButton, true, cancelButton); }
	public static TwoStageFutureWithPermission<bool> Confirmation(string message, string title, string okButton, bool canConfirm, string cancelButton)
	{
		var res = Resources.Load("Dialog/Content/GenericMessage");

		if (res != null)
		{
			var go = (GameObject)GameObject.Instantiate(res);
			var text = go.GetComponentInChildren<Text>();
			text.SetLocalizedText(message);
			return Confirmation(go, title, okButton, canConfirm, cancelButton);
		}

		return Confirmation(default(GameObject), title, okButton, canConfirm, cancelButton);
	}

	public static TwoStageFutureWithPermission<bool> Confirmation(GameObject content, string title) { return Confirmation(content, title, "DefaultConfirmationBoxYesButton", "DefaultConfirmationBoxNoButton"); }
	public static TwoStageFutureWithPermission<bool> Confirmation(GameObject content, string title, string okButton, string cancelButton) { return Confirmation(content, title, okButton, true, cancelButton); }
	public static TwoStageFutureWithPermission<bool> Confirmation(GameObject content, string title, string okButton, bool canConfirm, string cancelButton)
	{
		var res = Resources.Load("Dialog/ConfirmationBox");

		if (res != null)
		{
			var go = (GameObject)GameObject.Instantiate(res);
			var dialog = go.GetComponentInChildren<ConfirmationDialog>();

			dialog.GetComponentInChildren<Animator>().SetBool("IsActive", true);
			SetTopOrder(go, dialog.Future);

			dialog.SetInfo(content, title, okButton, canConfirm, cancelButton);

			return dialog.FutureWithPermission;
		}

		return null;
	}

	public static TwoStageFutureWithPermission<bool> Confirmation(GameObject dialogGO)
	{
		if (dialogGO != null)
		{
			var go = (GameObject)GameObject.Instantiate(dialogGO);
			var dialog = go.GetComponentInChildren<ConfirmationDialog>();

			dialog.GetComponentInChildren<Animator>().SetBool("IsActive", true);
			SetTopOrder(go, dialog.Future);

			return dialog.FutureWithPermission;
		}

		return null;
	}
}
