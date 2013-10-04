//*********************************************************************
// File:       		AuditTestCollection.cs
// Author:  	    Hector Sosa, Jr
// Date:			3/1/2005
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		3/1/2005	    Created
//*********************************************************************

using System.Collections;

namespace NDataAudit.Framework
{
	/// <summary>
	/// Summary description for AuditTestCollection.
	/// </summary>
	public class AuditTestCollection : CollectionBase
	{
		#region  Declarations 

		#endregion

		#region  Constructors 

		/// <summary>
		/// Empty constructor
		/// </summary>
		public AuditTestCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion

		#region  Properties 

		/// <summary>
		/// Gets the <see cref="Audit"/> object at Index position inside this AuditTestCollection.
		/// </summary>
		public AuditTest this[int Index]
		{
			get
			{
				return ((AuditTest)(List[Index]));
			}
		}

		#endregion

		#region  Public Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Items"></param>
		/// <returns></returns>
		public int[] Add(AuditTestCollection Items)
		{
			ArrayList indexes = new ArrayList();

			foreach (object Item in Items)
			{
				indexes.Add(this.List.Add(Item));
			}

			return ((int[])(indexes.ToArray(typeof(int))));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Item"></param>
		/// <returns></returns>
		public int Add(AuditTest Item)
		{
			return List.Add(Item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Index"></param>
		/// <param name="Item"></param>
		public void Insert(int Index, AuditTest Item)
		{
			List.Insert(Index, Item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Item"></param>
		public void Remove(AuditTest Item)
		{
			List.Remove(Item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Item"></param>
		/// <returns></returns>
		public bool Contains(AuditTest Item)
		{
			return List.Contains(Item);
		}

		#endregion
	}
}
