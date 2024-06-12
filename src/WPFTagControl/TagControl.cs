using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WPFTagControl
{
    /// <remarks>
    ///     Based on work of adabyron
    ///     http://stackoverflow.com/questions/15167809/how-can-i-create-a-tagging-control-similar-to-evernote-in-wpf
    /// </remarks>
    [TemplatePart(Name = "PART_CreateTagButton", Type = typeof (Button))]
    public class TagControl : ListBox
    {
        /// <summary>
        /// The tags property
        /// </summary>
        public static readonly DependencyProperty TagsProperty = DependencyProperty.Register("Tags",typeof(IEnumerable), typeof(TagControl), new FrameworkPropertyMetadata(null, OnTagsChanged));

        /// <summary>
        /// The add new tag text property
        /// </summary>
        public static readonly DependencyProperty AddNewTagTextProperty = DependencyProperty.Register("AddNewTagText",
            typeof (string), typeof (TagControl), new PropertyMetadata(null));

        /// <summary>
        /// The is editing property key
        /// </summary>
        private static readonly DependencyPropertyKey IsEditingPropertyKey =
            DependencyProperty.RegisterReadOnly("IsEditing", typeof (bool), typeof (TagControl),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// The is editing property
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

        static TagControl()
        {
            // lookless control, get default style from generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof (TagControl),
                new FrameworkPropertyMetadata(typeof (TagControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TagControl"/> class.
        /// </summary>
        public TagControl()
        {
            TagAdded += (s, e) => RaiseTagsChanged();
            TagRemoved += (s, e) => RaiseTagsChanged();
            TagEdited += (s, e) => RaiseTagsChanged();
            
        }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public IEnumerable Tags
        {
            get { return (IEnumerable)GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }


        /// <summary>
        /// Gets or sets the add new tag text.
        /// </summary>
        /// <value>
        /// The add new tag text.
        /// </value>
        public string AddNewTagText
        {
            get { return (string) GetValue(AddNewTagTextProperty); }
            set { SetValue(AddNewTagTextProperty, value); }
        }

        // IsEditing, readonly
        /// <summary>
        /// Gets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing
        {
            get { return (bool) GetValue(IsEditingProperty); }
            internal set { SetValue(IsEditingPropertyKey, value); }
        }

        private static void OnTagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (TagControl) d;

            c.ItemsSource = ((IEnumerable<object>) e.NewValue).Select(i => new TagItem(i, i.GetMemberValue<string>(c.DisplayMemberPath))).ToList();

            if (e.NewValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)e.NewValue).CollectionChanged -= c.OnTagsCollectionChanged;
                ((INotifyCollectionChanged)e.NewValue).CollectionChanged += c.OnTagsCollectionChanged;
            }
        }

        private void OnTagsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var c = this;                
                c.ItemsSource = ((IEnumerable<object>)Tags).Select(i => new TagItem(i, i.GetMemberValue<string>(c.DisplayMemberPath))).ToList();
            }
            

        }

        private void UpdateSelectedTagsOnRemove(TagItem removedTag)
        {
            if (removedTag == null)
                return;

            var its = Tags;

            if (its is IList)
            {
                ((IList)its).Remove(removedTag.Value);
            }

        }



        private void UpdateSelectedTagsOnAdd(TagItem addedTag)
        {
            var source = (IList<TagItem>) ItemsSource;
            //if (source.Count == SelectedTags.Count) //Update SelectedTags list if user edits tags
            //    SelectedTags.Where(i => source.All(s => !s.Text.Equals(i) || i.Equals(addedTag.Text)))
            //        .ToList()
            //        .ForEach(r => SelectedTags.Remove(r));
           // SelectedTags.Add(((TagObject)addedTag.Value));
        }

        /// <summary>
        /// Occurs when [tag click].
        /// </summary>
        public event EventHandler<TagEventArgs> TagClick;
        /// <summary>
        /// Occurs when [tag added].
        /// </summary>
        public event EventHandler<TagEventArgs> TagAdded;
        /// <summary>
        /// Occurs when [tag removed].
        /// </summary>
        public event EventHandler<TagEventArgs> TagRemoved;
        /// <summary>
        /// Occurs when [tag edited].
        /// </summary>
        public event EventHandler<TagEventArgs> TagEdited;
        /// <summary>
        /// Occurs when [tags changed].
        /// </summary>
        public event EventHandler<TagsChangedEventArgs> TagsChanged;


        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            ApplyTemplate();
        }

        /// <summary>
        /// Applies the template.
        /// </summary>
        /// <param name="appliedTag">The applied tag.</param>
        /// <param name="cancelEvent">if set to <c>true</c> [cancel event].</param>
        public void ApplyTemplate(TagItem appliedTag = null, bool cancelEvent = false)
        {
            var createBtn = GetTemplateChild("PART_CreateTagButton") as Button;
            if (createBtn != null)
            {
                createBtn.Click -= createBtn_Click;
                createBtn.Click += createBtn_Click;
            }

            var tagIcon = GetTemplateChild("PART_TagIcon") as Path;
            if (tagIcon != null)
            {
                //tagIcon.MouseUp -= createBtn_Click;
                //tagIcon.MouseUp += createBtn_Click;
            }

            base.OnApplyTemplate();

            if (appliedTag != null && !cancelEvent)
            {
                RaiseTagAdded(appliedTag);
            }
        }

        void createBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = InitializeNewTag();
        }

        internal TagItem InitializeNewTag(bool suppressEditing = false)
        {
            var newItem = new TagItem {IsEditing = !suppressEditing};
            AddTag(newItem);
            IsEditing = !suppressEditing;
            return newItem;
        }


        internal void AddTag(TagItem tag)
        {
            if (ItemsSource == null)
                ItemsSource = new List<TagItem>();

            ((IList) ItemsSource).Add(tag); // assume IList for convenience
            Items.Refresh();
        }

        internal void RemoveTag(TagItem tag, bool cancelEvent = false)
        {
            if (ItemsSource != null)
            {
                ((IList) ItemsSource).Remove(tag); // assume IList for convenience
                Items.Refresh();
                if (!cancelEvent)
                {
                   
                    RaiseTagRemoved(tag);
                }
            }
        }
        /// <summary>
        /// Raises the tag edited.
        /// </summary>
        /// <param name="tag">The tag.</param>
        public void RaiseTagEdited(TagItem tag)
        {
            UpdateSelectedTagsOnEdit();
            Debug.WriteLine($"RaiseTagEdited: {tag.Text}");
            TagEdited?.Invoke(this, new TagEventArgs(tag));
        }

        private void UpdateSelectedTagsOnEdit()
        {
            var source = (IList<TagItem>)ItemsSource; //ZUVIELE EVENTS WERDEN GESCHMISSEN
            //if (source.Count == SelectedTags.Count)
            //{
            //    for (int i = 0; i < source.Count; i++)
            //    {
            //        SelectedTags[i] = (TagObject)source[i].Value;
            //    }
            //}
        }

        private void RaiseTagRemoved(TagItem tag)
        {
            UpdateSelectedTagsOnRemove(tag);
            Debug.WriteLine($"RaiseTagRemoved: {tag.Text}");
            TagRemoved?.Invoke(this, new TagEventArgs(tag));
        }


        internal void RaiseTagClick(TagItem tag)
        {
            TagClick?.Invoke(this, new TagEventArgs(tag));
        }

        internal void RaiseTagsChanged()
        {
            var tokenizedTagItems = (IList<TagItem>) ItemsSource;
            Debug.WriteLine($"RaiseTagsChanged: {tokenizedTagItems.Aggregate("", (s, item) => $"{s} {item.Text}")}");
            TagsChanged?.Invoke(this, new TagsChangedEventArgs(tokenizedTagItems));
        }

        internal void RaiseTagAdded(TagItem tag)
        {
            UpdateSelectedTagsOnAdd(tag);
            Debug.WriteLine($"RaiseTagAdded: {tag.Text}");
            TagAdded?.Invoke(this, new TagEventArgs(tag));
        }

        internal void RaiseTagDoubleClick(TagItem tag)
        {
            tag.IsEditing = true;
        }


    }
}