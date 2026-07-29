using Application.Commands;
using Application.Dtos;
using MapsterMapper;
using Domain.Specifications;
using Domain.Models;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.CommandHandlers
{
    public class GetCatalogHandler : AbstractHandler<GetCatalog>
    {
        private readonly ICatalogRepository catalogRepository;
        private readonly IMapper mapper;

        public GetCatalogHandler(ICatalogRepository catalogRepository, IMapper mapper) {
            this.catalogRepository = catalogRepository;
            this.mapper = mapper;
        }
        public override IResponse Handle(GetCatalog message)
        {
            IList<Catalog> listaCatalogo; 

            if (message.ParentId != 0) listaCatalogo = catalogRepository.Filter(new FindCatalogByTypeAndParent(message.Type, message.ParentId)).ToList();
            else listaCatalogo = catalogRepository.Filter(new FindCatalogByType(message.Type)).ToList();
            var listaDto = new List<CatalogDto>();
            foreach (var item in listaCatalogo) listaDto.Add(mapper.Map<CatalogDto>(item));
            return new CatalogListDto { Lista = listaDto };
        }

    }
}
