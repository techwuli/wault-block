using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaultBlock.Models
{
    [Table("WalletDatas")]
    public class WalletData
    {
        /// <summary>
        /// Gets or sets the duration of the freshness, in seconds.
        /// </summary>
        /// <value>
        /// The duration of the freshness.
        /// </value>
        public double? FreshnessDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets the last opened.
        /// </summary>
        /// <value>
        /// The last opened.
        /// </value>
        public DateTime? LastOpened { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key, Column(Order = 0)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>
        /// The records.
        /// </value>
        public virtual ICollection<WalletRecord> Records { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
    }
}
