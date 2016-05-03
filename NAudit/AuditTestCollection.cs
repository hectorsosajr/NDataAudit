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

namespace NAudit.Framework
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
        /// Gets the <see cref="Audit" /> object at Index position inside this AuditTestCollection.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>AuditTest.</returns>
        public AuditTest this[int index]
		{
			get
			{
				return ((AuditTest)(List[index]));
			}
		}

        #endregion

        #region  Public Members

        /// <summary>
        /// Adds the specified items.
        /// </summary>
        /// <param name="Items">The items.</param>
        /// <returns>System.Int32[].</returns>
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
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public int Add(AuditTest item)
		{
			return List.Add(item);
		}

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void Insert(int index, AuditTest item)
		{
			List.Insert(index, item);
		}

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Remove(AuditTest item)
		{
			List.Remove(item);
		}

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(AuditTest item)
		{
			return List.Contains(item);
		}

		#endregion
	}
}
