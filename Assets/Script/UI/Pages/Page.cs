using UnityEngine;
using System.Collections;

public class Page : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private string id = "";
	[SerializeField] protected string type = "normal";
	#endregion

	#region Properties

	public string Id { get { return id; } }
	public string Type { get { return type; } }
	#endregion

	#region Public Methods
	

	public virtual void Initialize()
	{
	}

	public virtual void Show(bool back, bool immediate)
	{
		 
	}

	public virtual void Hide(bool back, bool immediate)
	{
		
	}

	public virtual void GoBack()
    {
		var navPage = transform.parent.GetComponent<NavPage>();
		if (navPage != null)
        {
			transform.parent.GetComponent<NavPage>().Back();
		}
	}
	#endregion
}
