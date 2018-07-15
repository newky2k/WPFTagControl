using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using TagControl.Common;
using WPFTagControl;

namespace TestUI
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private List<TagObject> suggestedTags;
        private ObservableCollection<TagObject> selectedTags;

        private IEnumerable<Thing> tags;


        public MainWindowViewModel()
        {
            SuggestedTags = new List<TagObject> {
                new TagObject()
                {
                     Text = "Tag 1",
                        Value = 0,
                },
                new TagObject()
                {
                     Text = "Tag 2",
                        Value = 1,
                },
                new TagObject()
                {
                     Text = "Tag 3",
                        Value = 2,
                },
            };


            SelectedTags = new ObservableCollection<TagObject>(new List<TagObject> { new TagObject()
            {
                 Text = "TestTag",
                    Value = 0,
            } });

            Tags = new ObservableCollection<Thing>()
            {
                new Thing()
                {
                    Name = "Tag 2",
                    Value = 1,
                },
            };

            SelectedTags.CollectionChanged += (sender, args) => 
            Debug.WriteLine("SelectedTagsCollectionChanged: " + SelectedTags.Aggregate("", (s,i)=> $"{s} {i}"));
        }

        public List<TagObject> SuggestedTags
        {
            get { return suggestedTags; }
            set
            {
                if (suggestedTags == value) return;
                suggestedTags = value;
                OnPropertyChanged();
            }
        }


        public IEnumerable<Thing> Tags
        {
            get { return tags; }
            set
            {
                if (tags == value) return;
                tags = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<TagObject> SelectedTags
        {
            get { return selectedTags; }
            set
            {
                if (selectedTags == value) return;
                selectedTags = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void SelectedTagsChanged(IList<TagItem> items)
        {
            Debug.WriteLine("VM.TagItemsChanged: " + items.Aggregate("", (s, item) => $"{s} {item.Text}"));
            Debug.WriteLine("VM.SelectedTagsChanged: " + SelectedTags.Aggregate("", (s, item) => $"{s} {item}"));
        }

        public void SetTagsFromViewModel()
        {
            SelectedTags = new ObservableCollection<TagObject>(new List<TagObject>
            {
                new TagObject()
                {
                     Text = "Test from VM",
                        Value = 1,
                },
                new TagObject()
                {
                     Text = "Test from VM 2",
                        Value = 2,
                },
                new TagObject()
                {
                     Text = "Test from VM 3",
                        Value = 2,
                },
                new TagObject()
                {
                     Text = "Test from VM 3",
                        Value = 3,
                },
                                                new TagObject()
                {
                     Text = "Test from VM 3",
                        Value = 4,
                },


            }); //Workaround: SelectedTags.Add("Tag") does not work

            Tags = new ObservableCollection<Thing>()
            {
                new Thing()
                {
                     Name = "Test from VM",
                        Value = 1,
                },
                new Thing()
                {
                     Name = "Test from VM 2",
                        Value = 2,
                },
                new Thing()
                {
                     Name = "Test from VM 3",
                        Value = 2,
                },
                new Thing()
                {
                     Name = "Test from VM 3",
                        Value = 3,
                },
                new Thing()
                {
                     Name = "Test from VM 3",
                        Value = 4,
                },

            };
        }

        public void AddedNewItemToViewModel()
        {
            ((ObservableCollection<Thing>)Tags).Add(new Thing()
            {
                Name="New Thing",
                Value = 5,
            });
        }
    }
}