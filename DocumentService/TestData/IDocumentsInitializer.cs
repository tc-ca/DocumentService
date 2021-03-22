using DocumentService.Contexts;

namespace DocumentService.TestData
{
    public interface IDocumentsInitializer
    {
        DocumentContext context { get; set; }

        void Seed();
    }
}