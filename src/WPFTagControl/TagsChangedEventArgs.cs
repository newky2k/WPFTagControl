using System;
using System.Collections.Generic;

namespace WPFTagControl
{
    /// <summary>
    /// Tags Changed events args
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TagsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<TagItem> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsChangedEventArgs"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public TagsChangedEventArgs(IList<TagItem> items)
        {
            this.Items = items;
        }
    }
}