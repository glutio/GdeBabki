﻿using System.Collections.Generic;
using System.Linq;

namespace GdeBabki.Client.ViewModel
{
    public class EditTagsViewModel : ViewModelBase
    {
        public List<string> Tags { get; set; }
        public string Tag { get; set; }
        public List<string> SuggestedTags { get; set; }
        public override void Initialize()
        {
            if (Tags == null)
            {
                Tags = new List<string>();
            }
            base.Initialize();
        }

        public void AddTag()
        {
            if (!Tags.Any(e => string.Equals(e, Tag, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                Tags.Add(Tag);
                Tag = null;
                RaisePropertyChanged(nameof(Tag));
            }
        }

        public void DeleteTag(string Tag)
        {
            Tags.Remove(Tag);
            RaisePropertyChanged(nameof(Tags));
        }
        
        public void DeleteLastTag()
        {
            if (string.IsNullOrEmpty(Tag) && Tags.Count > 0)
            {
                Tags.RemoveAt(Tags.Count - 1);
                RaisePropertyChanged(nameof(Tags));
            }
        }

        public void SuggestTags()
        {
            SuggestedTags = new List<string>() { "Abc", "Def", "Efg" };
            RaisePropertyChanged(nameof(SuggestedTags));
        }
    }
}
