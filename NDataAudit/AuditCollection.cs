//*********************************************************************
// File:       		AuditCollection.cs
// Author:  	    Hector Sosa, Jr
// Date:			2/16/2005
//**************************************************************************************
// Change Log
//**************************************************************************************
// USER					DATE        COMMENTS
// Hector Sosa, Jr		2/16/2005	Created
//**************************************************************************************

using System;
using System.Collections;
using System.ComponentModel;

namespace NDataAudit.Framework
{
	/// <summary>
	/// Summary description for AuditCollection.
	/// </summary>
	public class AuditCollection : CollectionBase
	{

		#region Constructors

		/// <summary>
		/// Empty constructor
		/// </summary>
		public AuditCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion
		
		#region  Properties 

		/// <summary>
		/// 
		/// </summary>
		public Audit this[int Index]
		{
			get
			{
				return ((Audit)(List[Index]));
			}
		}

		#endregion

		#region  Public Members 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Items"></param>
		/// <returns></returns>
		public int[] Add(AuditCollection Items)
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
		public int Add(Audit Item)
		{
			return List.Add(Item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Index"></param>
		/// <param name="Item"></param>
		public void Insert(int Index, Audit Item)
		{
			List.Insert(Index, Item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Item"></param>
		public void Remove(Audit Item)
		{
			List.Remove(Item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Item"></param>
		/// <returns></returns>
		public bool Contains(Audit Item)
		{
			return List.Contains(Item);
		}

		#endregion
	}
}
