using System;
using System.Collections.Generic;

namespace Assignment4.Core
{
    public interface ITagRepository : IDisposable {
        (Response, IReadOnlyCollection<TagDTO>) All();

        (Response, TagDTO) Create(TagCreateDTO tag);

        Response Delete(int tagId);

        (Response, TagDTO) FindById(int tagId);

        Response Update(TagDTO tag);
    }
}