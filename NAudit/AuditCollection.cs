//*********************************************************************
// File:       		AuditCollection.cs
// Author:  	    Hector Sosa, Jr
// Date:			2/16/2005
//**************************************************************************************
// Change Log
//**************************************************************************************
// USER					DATE        COMMENTS
// Hector Sosa, Jr		2/16/2005	Created
// Hector Sosa, Jr      3/12/2017   Renamed NAudit back to NDataAudit.
//**************************************************************************************

using System.Collections;

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
        /// Gets the <see cref="Audit"/> with the specified index.
        /// </summary>
        /// <param name="Index">The index.</param>
        /// <returns>Audit.</returns>
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
        /// Adds the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>System.Int32[].</returns>
        public int[] Add(AuditCollection items)
		{
			ArrayList indexes = new ArrayList();

			foreach (object Item in items)
			{
				indexes.Add(this.List.Add(Item));
			}

			return ((int[])(indexes.ToArray(typeof(int))));
		}

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public int Add(Audit item)
		{
			return List.Add(item);
		}

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void Insert(int index, Audit item)
		{
			List.Insert(index, item);
		}

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(Audit item)
		{
			List.Remove(item);
		}

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(Audit item)
		{
			return List.Contains(item);
		}

		#endregion
	}
}
