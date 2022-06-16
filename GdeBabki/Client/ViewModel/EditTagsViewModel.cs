using GdeBabki.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GdeBabki.Client.ViewModel
{
    public class EditTagsViewModel : ViewModelBase
    {
        private readonly TagsApi tagsApi;

        public List<string> Tags { get; set; }
        public string Tag { get; set; }
        public List<string> SuggestedTags { get; set; }
        public string LastTag => Tags == null || Tags.Count == 0 ? null : Tags[Tags.Count - 1];
        public bool HasNewTag => !string.IsNullOrWhiteSpace(Tag) && !Tags.Contains(Tag.ToUpper());
        
        public EditTagsViewModel(TagsApi tagsApi)
        {
            this.tagsApi = tagsApi;
        }

        public override void OnInitialize()
        {
            if (Tags == null)
            {
                Tags = new List<string>();
            }
            base.OnInitialize();
        }

        public void AddTag()
        {
            if (string.IsNullOrWhiteSpace(Tag))
            {
                return;
            }

            if (!Tags.Any(e => string.Equals(e, Tag, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                Tags.Add(Tag.ToUpper());
                RaisePropertyChanged(nameof(Tags));
            }
        }

        public void DeleteTag(string tag)
        {
            Tags.Remove(tag);
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

        public async Task SuggestTags()
        {
            var allSuggestedTags = await tagsApi.SuggestTagsAsync(Guid.Empty);
            SuggestedTags = allSuggestedTags.Except(Tags).ToList();
            RaisePropertyChanged(nameof(SuggestedTags));
        }
    }
}
