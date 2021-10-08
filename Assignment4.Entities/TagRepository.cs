using Assignment4.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Assignment4.Entities
{
    public class TagRepository : ITagRepository
    {
        private readonly IKanbanContext _context;

        public TagRepository(IKanbanContext context)
        {
            _context = context;
        }

        public (Response, IReadOnlyCollection<TagDTO>) All()
        {
            var tags = from t in _context.Tags
                select new TagDTO(t.Id, t.Name);

            return (Response.Success, tags.ToList());
        }

        public (Response, TagDTO) Create(TagCreateDTO tag)
        {
            var entity = new Tag {
                Name = tag.Name
            };

            _context.Tags.Add(entity);
            _context.SaveChanges();

            return (Response.Created, new TagDTO(entity.Id, entity.Name));
        }

        public Response Delete(int tagId)
        {
            var entity = _context.Tags.Find(tagId);

            if (entity == null) {
                return Response.NotFound;
            }

            _context.Tags.Remove(entity);
            _context.SaveChanges();

            return Response.Deleted;
        }

        public (Response, TagDTO) FindById(int tagId)
        {
            var tag = _context.Tags.Find(tagId);

            if (tag == null) {
                return (Response.NotFound, null);
            }
            
            return (Response.Success, new TagDTO(tag.Id, tag.Name));
        }

        public Response Update(TagDTO tag)
        {
            var entity = _context.Tags.Find(tag.Id);

            if (entity == null) {
                return Response.NotFound;
            }

            entity.Name = tag.Name;

            _context.SaveChanges();

            return Response.Updated;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}