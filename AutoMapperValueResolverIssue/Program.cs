using AutoMapper;
using System.Collections.Generic;

namespace AutoMapperDemo
{
    public class TrackedData<T>
    {
        public ICollection<T> Updated { get; set; }
        public ICollection<T> Created { get; set; }
        public ICollection<T> Deleted { get; set; }

        public TrackedData()
        {
            Updated = new List<T>();
            Created = new List<T>();
            Deleted = new List<T>();
        }
    }

    public class Foo
    {
        private List<Child> _childs;

        public string Name { get; set; }
        public ICollection<Child> Childs
        {
            get
            {
                if (_childs == null)
                    _childs = new List<Child>();
                return _childs;
            }
            set
            {
                _childs = (List<Child>)value;
            }
        }
    }

    public class Child
    {
        public string Description { get; set; }
    }

    public class FooDTO
    {
        public string Name { get; set; }
        public TrackedData<ChildDTO> Childs { get; set; }
    }

    public class ChildDTO
    {
        public string Description { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Mapper.CreateMap(typeof(TrackedData<>), typeof(TrackedData<>));
            Mapper.CreateMap<ChildDTO, Child>();
            Mapper.CreateMap<FooDTO, Foo>()
                .ForMember(d => d.Childs, opt => opt.ResolveUsing<TrackedDataResolver<ChildDTO, Child>>());

            FooDTO fooDTO = CreateFakeFooDTO();

            Foo foo = Mapper.Map<FooDTO, Foo>(fooDTO);
        }

        private static FooDTO CreateFakeFooDTO()
        {
            return new FooDTO()
            {
                Name = Faker.NameFaker.Name(),
                Childs = CreateFakeTrackedDataChildDTO()
            };
        }

        private static TrackedData<ChildDTO> CreateFakeTrackedDataChildDTO()
        {
            return new TrackedData<ChildDTO>()
            {
                Created = CreateFakeListChildDTO(),
                Updated = CreateFakeListChildDTO(),
                Deleted = CreateFakeListChildDTO()
            };
        }

        private static ICollection<ChildDTO> CreateFakeListChildDTO()
        {
            return new List<ChildDTO>()
            {
                CrateFakeChildDTO(),
                CrateFakeChildDTO()
            };
        }

        private static ChildDTO CrateFakeChildDTO()
        {
            return new ChildDTO()
            {
                Description = Faker.TextFaker.Sentence(),
            };
        }
    }

    public class TrackedDataResolver<TSource, TDestination> : ValueResolver<TrackedData<TSource>, ICollection<TDestination>>
    {
        protected override ICollection<TDestination> ResolveCore(TrackedData<TSource> source)
        {
            TrackedData<TDestination> trackedDestination = new TrackedData<TDestination>();
            ICollection<TDestination> destination = new List<TDestination>();

            trackedDestination = Mapper.Map<TrackedData<TSource>, TrackedData<TDestination>>(source);

            foreach (var o in trackedDestination.Created)
            {
                // Do stuff...
                destination.Add(o);
            }
            foreach (var o in trackedDestination.Updated)
            {
                // Do stuff...
                destination.Add(o);
            }
            foreach (var o in trackedDestination.Deleted)
            {
                // Do stuff...
                destination.Add(o);
            }

            return destination;
        }
    }


}
