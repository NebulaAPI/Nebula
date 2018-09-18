namespace Nebula.Renderers
{
    public class RendererFactory
    {
        public static AbstractRenderer Get(string language)
        {
            switch(language.ToLower())
            {
                case "c#": return new CSharpRenderer();
                default: return null;
            }    
        }
    }
}