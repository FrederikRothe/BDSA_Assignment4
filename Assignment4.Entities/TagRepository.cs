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

        public IReadOnlyCollection<TagDTO> All()
        {
            var tags = from t in _context.Tags
                select new TagDTO(t.Id, t.Name);

            return tags.ToList();
        }

        public TagDTO Create(TagCreateDTO tag)
        {
            var entity = new Tag {
                Name = tag.Name
            };

            _context.Tags.Add(entity);
            _context.SaveChanges();

            return new TagDTO(entity.Id, entity.Name);
        }

        public void Delete(int tagId)
        {
            _context.Tags.Remove(_context.Tags.Single(t => t.Id == tagId));
            _context.SaveChanges();
        }

        public TagDTO FindById(int tagId)
        {
            var tags = from t in _context.Tags
                where t.Id == tagId
                select new TagDTO(t.Id, t.Name);

            return tags.FirstOrDefault();
        }

        public void Update(TagDTO tag)
        {
            var entity = _context.Tags.Find(tag.Id);

            entity.Name = tag.Name;

            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}