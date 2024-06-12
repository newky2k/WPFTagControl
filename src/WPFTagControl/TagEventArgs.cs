using System;

namespace WPFTagControl
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TagEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public TagItem Item { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public TagEventArgs(TagItem item)
        {
            this.Item = item;
        }
    }
}