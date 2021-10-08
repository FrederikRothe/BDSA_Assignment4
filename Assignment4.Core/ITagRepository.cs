using System;
using System.Collections.Generic;

namespace Assignment4.Core
{
    public interface ITagRepository : IDisposable {
        IReadOnlyCollection<TagDTO> All();

        TagDTO Create(TagCreateDTO tag);

        void Delete(int tagId);

        TagDTO FindById(int tagId);

        void Update(TagDTO tag);
    }
}